using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Formatter;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;
using Core.DomainModel;
using Core.DomainModel.Example;
using OS2Indberetning.Controllers;

namespace OS2Indberetning
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            
            config.MapODataServiceRoute(
                routeName: "odata",
                routePrefix: "odata",
                model: GetModel()
                );
        }

        public static Microsoft.OData.Edm.IEdmModel GetModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

            //builder.EntitySet<TestReport>("TestReports");
            //var test = builder.EntityType<TestReport>();
            //test.Ignore(report => report.DateTimeTest);

            builder.EntitySet<Address>("Addresses");

            builder.EntitySet<DriveReport>("DriveReports");

            builder.EntitySet<DriveReportPoint>("DriveReportPoints");

            builder.EntitySet<Employment>("Employments");
            var eType = builder.EntityType<Employment>();
            eType.HasKey(e => e.Id);

            builder.EntitySet<FileGenerationSchedule>("FileGenerationSchedules");

            builder.EntitySet<LicensePlate>("LicensePlates");

            builder.EntitySet<MailNotificationSchedule>("MailNotificationSchedules");

            builder.EntitySet<MobileToken>("MobileTokens");

            builder.EntitySet<OrgUnit>("OrgUnits");

            builder.EntitySet<Person>("Persons");
            var pType = builder.EntityType<Person>();
            pType.HasKey(p => p.Id);

            builder.EntitySet<PersonalAddress>("PersonalAddresses");

            builder.EntitySet<PersonalRoute>("PersonalRoutes");

            builder.EntitySet<Point>("Points");

            builder.EntitySet<Rate>("Rates");

            builder.EntitySet<Report>("Reports");

            builder.EntitySet<Substitute>("Substitutes");

            return builder.GetEdmModel();
        }
    }
}
