using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;
using Presentation.Web.Test.Controllers;

namespace ApplicationServices.Test.MailerServiceTest
{
    class DriveRepoMock : GenericRepositoryMock<DriveReport>
    {
        public static Person p1;
        public static Person p2;
        public static Person p3;
        public static Person p4;

        public static DriveReport dr1;
        public static DriveReport dr2;
        public static DriveReport dr3;
        public static DriveReport dr4;

        public static Employment e1;
        public static Employment e2;
        public static Employment e3;
        public static Employment e4;


        protected override List<DriveReport> Seed()
        {
            p1 = new Person()
            {
                Id = 1,
                RecieveMail = true,
                Mail = "Lars@g.dk",
            };

            p2 = new Person()
            {
                Id = 2,
                RecieveMail = true,
                Mail = "Preben@g.dk",
            };

            p3 = new Person()
            {
                Id = 3,
                RecieveMail = true,
                Mail = "Ivan@g.dk",
            };

            p4 = new Person()
            {
                Id = 1,
                RecieveMail = true,
                Mail = "Frank@g.dk",
            };

            var orgUnit1 = new OrgUnit()
           {
               Id = 1,
               Level = 1,
           };
            var orgUnit2 = new OrgUnit()
            {
                Id = 2,
                Level = 1
            };

            e1 = new Employment()
            {
                Id = 1,
                IsLeader = true,
                OrgUnit = orgUnit1,
                Person = p1
            };

            e2 = new Employment()
            {
                Id = 2,
                IsLeader = false,
                OrgUnit = orgUnit1,
                Person = p2
            };

            e3 = new Employment()
            {
                Id = 3,
                IsLeader = true,
                OrgUnit = orgUnit2,
                Person = p3
            };

            e4 = new Employment()
            {
                Id = 4,
                IsLeader = false,
                OrgUnit = orgUnit2,
                Person = p4

            };

            orgUnit1.Employments = new List<Employment>()
            {
                e1,
                e2
            };

            orgUnit2.Employments = new List<Employment>()
            {
                e3,
                e4
            };

            dr1 = new DriveReport()
            {
                Id = 1,
                Status = ReportStatus.Pending,
                Employment = e1

            };

            dr2 = new DriveReport()
            {
                Id = 2,
                Status = ReportStatus.Pending,
                Employment = e2
            };

            dr3 = new DriveReport()
            {
                Id = 3,
                Status = ReportStatus.Pending,
                Employment = e3
            };

            dr4 = new DriveReport()
            {
                Id = 4,
                Status = ReportStatus.Pending,
                Employment = e4
            };





            return new List<DriveReport>()
            {
               dr1,dr2,dr3,dr4
            };
        }
    }

    public class SubRepoMock : GenericRepositoryMock<Core.DomainModel.Substitute>
    {
        public List<Substitute> Substitutes = new List<Substitute>();

        protected override List<Substitute> Seed()
        {
            return Substitutes;
        }
    }
}
