using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainModel.Example;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OS2Indberetning
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.DependencyResolver = new NinjectDependencyResolver(NinjectWebKernel.CreateKernel());

            config.MapHttpAttributeRoutes();

            config.AddODataQueryFilter();

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

            builder.EntityType<Address>().Collection
            .Action("SetCoordinatesOnAddress")
            .ReturnsFromEntitySet<Address>("Addresses");

            builder.EntityType<Address>().Collection
            .Action("SetCoordinatesOnAddressList")
            .ReturnsFromEntitySet<Address>("Addresses");

            

            builder.EntityType<Address>().Collection
            .Function("GetPersonalAndStandard")
            .ReturnsFromEntitySet<Address>("Addresses");

            builder.EntityType<Address>().Collection
            .Function("GetStandard")
            .ReturnsFromEntitySet<Address>("Addresses");

            builder.EntityType<Address>().Collection
            .Function("GetCachedAddresses")
            .ReturnsFromEntitySet<Address>("Addresses");

            builder.EntityType<Address>().Collection
           .Action("AttemptCleanCachedAddress")
           .ReturnsFromEntitySet<Address>("Addresses");





            builder.EntityType<Address>().Collection
                .Function("GetMapStart")
                .ReturnsFromEntitySet<Address>("Addresses");

            builder.EntitySet<DriveReport>("DriveReports");

            builder.EntityType<DriveReport>().Collection
            .Function("GetLatestReportForUser")
            .ReturnsFromEntitySet<DriveReport>("DriveReports");

            builder.EntitySet<DriveReportPoint>("DriveReportPoints");

            builder.EntitySet<Employment>("Employments");
            var eType = builder.EntityType<Employment>();
            eType.HasKey(e => e.Id);

            builder.EntitySet<FileGenerationSchedule>("FileGenerationSchedules");

            builder.EntitySet<LicensePlate>("LicensePlates");

            //var lType = builder.EntityType<LicensePlate>();
            //lType.Ignore(l => l.Person);


            builder.EntitySet<MailNotificationSchedule>("MailNotifications");

            builder.EntitySet<RateType>("RateTypes");

            builder.EntitySet<MobileToken>("MobileToken");

            builder.EntitySet<OrgUnit>("OrgUnits");

            builder.EntitySet<Person>("Person");
            var pType = builder.EntityType<Person>();
            pType.HasKey(p => p.Id);
            //pType.Ignore(p => p.LicensePlates);


            builder.EntityType<Person>().Collection
           .Function("GetCurrentUser")
           .ReturnsFromEntitySet<Person>("Person");

            builder.EntitySet<PersonalAddress>("PersonalAddresses");

            builder.EntityType<PersonalAddress>().Collection
            .Function("GetHome")
            .ReturnsFromEntitySet<PersonalAddress>("PersonalAddresses");

            builder.EntityType<PersonalAddress>().Collection
            .Function("GetRealHome")
            .ReturnsFromEntitySet<PersonalAddress>("PersonalAddresses");




            builder.EntityType<PersonalAddress>().Collection
            .Function("GetAlternativeHome")
            .ReturnsFromEntitySet<PersonalAddress>("PersonalAddresses");


            builder.EntityType<OrgUnit>().Collection
            .Function("GetLeaderOfOrg")
            .ReturnsFromEntitySet<Person>("Person");

            builder.EntityType<OrgUnit>().Collection
            .Function("GetWhereUserIsResponsible")
            .ReturnsFromEntitySet<OrgUnit>("OrgUnits");


            builder.EntitySet<PersonalRoute>("PersonalRoutes");

            builder.EntitySet<Point>("Points");

            builder.EntitySet<Report>("Reports");



            builder.EntitySet<BankAccount>("BankAccounts");

            builder.EntitySet<Person>("Person");
            builder.EntityType<Person>()
                .Action("HasLicensePlate");


            builder.EntitySet<Substitute>("Substitutes");
            builder.EntityType<Substitute>().Collection
                .Function("Personal")
                .ReturnsFromEntitySet<Substitute>("Substitutes");
            builder.EntityType<Substitute>().Collection
                .Function("Substitute")
                .ReturnsFromEntitySet<Substitute>("Substitutes");

            builder.EntitySet<Rate>("Rates");
            builder.EntityType<Rate>().Collection
                .Function("ThisYearsRates")
                .ReturnsFromEntitySet<Rate>("Rates");

            builder.Namespace = "Service";

            return builder.GetEdmModel();
        }
    }
}


