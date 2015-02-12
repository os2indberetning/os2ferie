using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Formatter;
using System.Web.OData.Formatter.Deserialization;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;
using Core.DomainModel;
using Core.DomainModel.Example;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OS2Indberetning.Controllers;
using OS2Indberetning.Models;

namespace OS2Indberetning
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MapODataServiceRoute(
                routeName: "odata",
                routePrefix: "odata",
                model: GetModel()
                );

            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            
        }

        public static Microsoft.OData.Edm.IEdmModel GetModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

            builder.EntitySet<TestReport>("TestReports");
            //var test = builder.EntityType<TestReport>();
            //test.Ignore(report => report.DateTimeTest);

            builder.EntitySet<Address>("Addresses");

            builder.EntitySet<DriveReport>("DriveReports");

            builder.EntitySet<DriveReportPoint>("DriveReportPoints");

            builder.EntitySet<Employment>("Employments");
            var eType = builder.EntityType<Employment>();
            eType.HasKey(e => e.Id);

            builder.EntitySet<FileGenerationSchedule>("FileGenerationSchedules");
            
            var lType = builder.EntityType<LicensePlate>();
            lType.Ignore(l => l.Person);
            builder.EntitySet<LicensePlate>("LicensePlates");

            builder.EntitySet<MailNotificationSchedule>("MailNotificationSchedules");

            builder.EntitySet<MobileToken>("MobileTokens");

            builder.EntitySet<OrgUnit>("OrgUnits");

            builder.EntitySet<Person>("Person");
            var pType = builder.EntityType<Person>();
            pType.HasKey(p => p.Id);
            pType.Ignore(p => p.LicensePlates);            

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
