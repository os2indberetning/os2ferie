using System;
using System.Collections.Generic;
using Core.DomainModel;
using Presentation.Web.Test.Controllers;

namespace ApplicationServices.Test.DriveReportServiceTest
{
    public class SubstituteRepositoryMock : GenericRepositoryMock<Substitute>
    {
        private Substitute sub = new Substitute()
        {
            Id = 1,
            PersonId = 2,
            LeaderId = 1,
            SubId = 2,
            StartDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(-1))).TotalSeconds,
            EndDateTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1).AddDays(1))).TotalSeconds
        };

        protected override List<Substitute> Seed()
        {
            return new List<Substitute>()
            {
                sub
            };
        }
    };
}
