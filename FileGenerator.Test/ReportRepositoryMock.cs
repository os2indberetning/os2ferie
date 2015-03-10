using System;
using System.Collections.Generic;
using Core.DomainModel;
using Presentation.Web.Test.Controllers;

namespace FileGenerator.Test
{
    public class ReportRepositoryMock : GenericRepositoryMock<DriveReport>
    {
        public ReportRepositoryMock()
        {
            ReSeed();
        }

        private static readonly Employment Employment = new Employment
        {
            EmploymentId = 2,
            EmploymentType = 1
        };

        private static readonly Person Person1 = new Person
        {
            CprNumber = "1111111111"
        };

        private static readonly Person Person2 = new Person
        {
            CprNumber = "2222222222"
        };

        public DriveReport Report1 = new DriveReport
        {
            DriveDateTimestamp = 1425982953,
            TFCode = "310-4",
            Distance = 100,
            Employment = Employment,
            Person = Person1,
            Status = ReportStatus.Accepted
        };

        public DriveReport Report2 = new DriveReport
        {
            DriveDateTimestamp = 1425982953,
            TFCode = "310-4",
            Distance = 200,
            Employment = Employment,
            Person = Person2,
            Status = ReportStatus.Accepted
        };

        public DriveReport Report3 = new DriveReport
        {
            DriveDateTimestamp = 1425982953,
            TFCode = "310-4",
            Distance = 300,
            Employment = Employment,
            Person = Person2,
            Status = ReportStatus.Rejected
        };

        public DriveReport Report4 = new DriveReport
        {
            DriveDateTimestamp = 1425982953,
            TFCode = "310-4",
            Distance = 400,
            Employment = Employment,
            Person = Person1,
            Status = ReportStatus.Invoiced
        };

        public DriveReport Report5 = new DriveReport
        {
            DriveDateTimestamp = 1425982953,
            TFCode = "310-4",
            Distance = 500,
            Employment = Employment,
            Person = Person1,
            Status = ReportStatus.Accepted
        };


        protected override List<DriveReport> Seed()
        {
            return new List<DriveReport>
            {
                Report1, Report2, Report3, Report4, Report5 
            };
        }
    }
}