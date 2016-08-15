using System;
using System.Linq;
using System.Web.OData;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.Logger;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;

namespace Core.ApplicationServices
{
    public class ReportService<T> : IReportService<T> where T : Report
    {
        protected readonly IGenericRepository<OrgUnit> _orgUnitRepository;
        protected readonly IGenericRepository<Employment> _employmentRepository;
        protected readonly IGenericRepository<Substitute> _substituteRepository;
        protected readonly IMailSender _mailSender;
        protected readonly IGenericRepository<T> _reportRepo;
        protected readonly ILogger _logger;

        public ReportService(IMailSender mailSender, IGenericRepository<OrgUnit> orgUnitRepository,
            IGenericRepository<Employment> employmentRepository, IGenericRepository<Substitute> substituteRepository, ILogger logger, IGenericRepository<T> reportRepo)
        {
            _orgUnitRepository = orgUnitRepository;
            _employmentRepository = employmentRepository;
            _substituteRepository = substituteRepository;
            _mailSender = mailSender;
            _logger = logger;
            _reportRepo = reportRepo;
        }

        public T Create(T report)
        {
            if (!Validate(report)) throw new Exception("Vacation report has invalid parameters");
            report.ResponsibleLeaderId = GetResponsibleLeaderForReport(report).Id;
            report.ActualLeaderId = GetActualLeaderForReport(report).Id;
            _reportRepo.Insert(report);
            _reportRepo.Save();

            return report;
        }

        public bool Validate(T report)
        {
            if (report.PersonId == 0) return false;
            return true;
        }

        public void SendMailToUserAndApproverOfEditedReport(T report, string emailText, Person admin, string action)
        {
            var mailContent = "Hej," + Environment.NewLine + Environment.NewLine +
            "Jeg, " + admin.FullName + ", har pr. dags dato " + action + " den følgende godkendte indberetning:" + Environment.NewLine + Environment.NewLine;

            if (report.Comment != null)
                mailContent += Environment.NewLine + "Kommentar: " + report.Comment;

            mailContent += Environment.NewLine + Environment.NewLine
            + "Hvis du mener at dette er en fejl, så kontakt mig da venligst på " + admin.Mail + Environment.NewLine
            + "Med venlig hilsen " + admin.FullName + Environment.NewLine + Environment.NewLine
            + "Besked fra administrator: " + Environment.NewLine + emailText;

            _mailSender.SendMail(report.Person.Mail, "En administrator har ændret i din indberetning.", mailContent);

            _mailSender.SendMail(report.ApprovedBy.Mail, "En administrator har ændret i en indberetning du har godkendt.", mailContent);
        }

        public void SendMailIfRejectedReport(int key, Delta<T> delta, Person person)
        {
            object status;

            if (!delta.TryGetPropertyValue("Status", out status)) return;
            if (!status.ToString().Equals("Rejected")) return;
            if (string.IsNullOrEmpty(person.Mail))
            {
                _logger.Log(
                    "Forsøg på at sende mail om afvist indberetning til " + person.FullName +
                    ", men der findes ingen emailadresse. " + person.FullName +
                    " har derfor ikke modtaget en mailadvisering", "mail", 2);
                throw new Exception("Forsøg på at sende mail til person uden emailaddresse");
            }

            var recipient = person.Mail;
            object comment;
            if (delta.TryGetPropertyValue("Comment", out comment))
            {
                _mailSender.SendMail(recipient, "Afvist indberetning",
                    "Din indberetning er blevet afvist med kommentaren: \n \n" + comment);
            }
        }

        public Person GetResponsibleLeaderForReport(T report)
        {
            var currentDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            // Fix for bug that sometimes happens when drivereport is from app, where personid is set, but person is not.
            var person = _employmentRepository.AsQueryable().First(x => x.PersonId == report.PersonId).Person;


            // Fix for bug that sometimes happens when drivereport is from app, where personid is set, but person is not.
            var empl = _employmentRepository.AsQueryable().First(x => x.Id == report.EmploymentId);

            //Fetch personal approver for the person (Person and Leader of the substitute is the same)
            var personalApprover =
                _substituteRepository.AsQueryable()
                    .SingleOrDefault(
                        s =>
                            s.PersonId != s.LeaderId && s.PersonId == person.Id &&
                            s.StartDateTimestamp < currentDateTimestamp && s.EndDateTimestamp > currentDateTimestamp && s.Type == report.ReportType);
            if (personalApprover != null)
            {
                return personalApprover.Sub;
            }

            //Find an org unit where the person is not the leader, and then find the leader of that org unit to attach to the drive report
            var orgUnit = _orgUnitRepository.AsQueryable().SingleOrDefault(o => o.Id == empl.OrgUnitId);
            var leaderOfOrgUnit =
                _employmentRepository.AsQueryable().FirstOrDefault(e => e.OrgUnit.Id == orgUnit.Id && e.IsLeader && e.StartDateTimestamp < currentDateTimestamp && (e.EndDateTimestamp > currentDateTimestamp || e.EndDateTimestamp == 0));

            if (orgUnit == null)
            {
                return null;
            }

            var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            while ((leaderOfOrgUnit == null && orgUnit.Level > 0) || (leaderOfOrgUnit != null && leaderOfOrgUnit.PersonId == person.Id))
            {
                leaderOfOrgUnit = _employmentRepository.AsQueryable().SingleOrDefault(e => e.OrgUnit.Id == orgUnit.ParentId && e.IsLeader &&
                                                                                            e.StartDateTimestamp < currentTimestamp &&
                                                                                            (e.EndDateTimestamp == 0 || e.EndDateTimestamp > currentTimestamp));
                orgUnit = orgUnit.Parent;
            }


            if (orgUnit == null)
            {
                return null;
            }
            if (leaderOfOrgUnit == null)
            {
                return null;
            }

            var leader = leaderOfOrgUnit.Person;

            // Recursively look for substitutes in child orgs, up to the org of the actual leader.
            // Say the actual leader is leader of orgunit 1 with children 2 and 3. Child 2 has another child 4.
            // A report comes in for orgUnit 4. Check if leader has a substitute for that org.
            // If not then check if leader has a substitute for org 2.
            // If not then return the actual leader.
            var orgToCheck = empl.OrgUnit;
            Substitute sub = null;
            var loopHasFinished = false;
            while (!loopHasFinished)
            {
                sub = _substituteRepository.AsQueryable().SingleOrDefault(s => s.OrgUnitId == orgToCheck.Id && s.PersonId == leader.Id && s.StartDateTimestamp < currentDateTimestamp && s.EndDateTimestamp > currentDateTimestamp && s.PersonId.Equals(s.LeaderId) && s.Type == report.ReportType);
                if (sub != null)
                {
                    if (sub.Sub == null)
                    {
                        // This is a hack fix for a weird bug that happens, where sometimes the Sub navigation property on a Substitute is null, even though the SubId is not.
                        sub.Sub = _employmentRepository.AsQueryable().FirstOrDefault(x => x.PersonId == sub.SubId).Person;
                    }
                    loopHasFinished = true;
                }
                else
                {
                    orgToCheck = orgToCheck.Parent;
                    if (orgToCheck == null || orgToCheck.Id == orgUnit.Parent.Id)
                    {
                        loopHasFinished = true;
                    }
                }
            }
            return sub != null ? sub.Sub : leaderOfOrgUnit.Person;
        }

        public Person GetActualLeaderForReport(T report)
        {
            var currentDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            // Fix for bug that sometimes happens when drivereport is from app, where personid is set, but person is not.
            var person = _employmentRepository.AsQueryable().First(x => x.PersonId == report.PersonId).Person;

            // Fix for bug that sometimes happens when drivereport is from app, where personid is set, but person is not.
            var empl = _employmentRepository.AsQueryable().First(x => x.Id == report.EmploymentId);

            //Find an org unit where the person is not the leader, and then find the leader of that org unit to attach to the report
            var orgUnit = _orgUnitRepository.AsQueryable().SingleOrDefault(o => o.Id == empl.OrgUnitId);
            var leaderOfOrgUnit =
                _employmentRepository.AsQueryable().FirstOrDefault(e => e.OrgUnit.Id == orgUnit.Id && e.IsLeader && e.StartDateTimestamp < currentDateTimestamp && (e.EndDateTimestamp > currentDateTimestamp || e.EndDateTimestamp == 0));

            if (orgUnit == null)
            {
                return null;
            }

            var currentTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            while ((leaderOfOrgUnit == null && orgUnit.Level > 0) || (leaderOfOrgUnit != null && leaderOfOrgUnit.PersonId == person.Id))
            {
                leaderOfOrgUnit = _employmentRepository.AsQueryable().SingleOrDefault(e => e.OrgUnit.Id == orgUnit.ParentId && e.IsLeader &&
                                                                                            e.StartDateTimestamp < currentTimestamp &&
                                                                                            (e.EndDateTimestamp == 0 || e.EndDateTimestamp > currentTimestamp));
                orgUnit = orgUnit.Parent;
            }


            if (orgUnit == null)
            {
                return null;
            }
            if (leaderOfOrgUnit == null)
            {
                // This statement will be hit when all orgunits up to (not including) level 0 have been checked for a leader.
                // If no actual leader has been found then return the reponsibleleader.
                // This will happen when members of orgunit 0 try to create a report, as orgunit 0 has no leaders and they are all handled by a substitute.
                return GetResponsibleLeaderForReport(report);
            }

            return leaderOfOrgUnit.Person;
        }

    }
}
