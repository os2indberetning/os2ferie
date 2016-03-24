using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.OData;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.MailerService.Impl;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Infrastructure.AddressServices;
using Infrastructure.AddressServices.Routing;
using Infrastructure.DataAccess;
using Ninject;
using OS2Indberetning;
using Core.ApplicationServices.Logger;


namespace Core.ApplicationServices
{
    public class VacationReportService : BaseReportService<VacationReport>
    {

        private readonly IGenericRepository<VacationReport> _reportRepo;

        public VacationReportService(IGenericRepository<VacationReport> reportRepo, IMailSender mailSender, IGenericRepository<OrgUnit> orgUnitRepository, IGenericRepository<Employment> employmentRepository, IGenericRepository<Substitute> substituteRepository) : base(mailSender, orgUnitRepository, employmentRepository, substituteRepository)
        {
            _reportRepo = reportRepo;
        }

        public override VacationReport Create(VacationReport report)
        {
            if (!Validate(report)) throw new Exception("Vacation report has invalid parameters");

            report.ResponsibleLeaderId = GetResponsibleLeaderForReport(report, SubstituteType.Vacation).Id;
            report.ActualLeaderId = GetActualLeaderForReport(report, SubstituteType.Vacation).Id;

            if (report.Comment == null) report.Comment = "";

            _reportRepo.Insert(report);

            _reportRepo.Save();

            return report;
        }

        public override bool Validate(VacationReport report)
        {

            if (report.PersonId == 0) return false;
            if (report.EndTimestamp < report.StartTimestamp) return false;
            

            return true;
        }

        public override void SendMailToUserAndApproverOfEditedReport(VacationReport report, string emailText, Person admin, string action)
        {
            throw new NotImplementedException();
        }
    }
}
