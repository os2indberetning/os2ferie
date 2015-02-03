using System.Web.Http;
using System.Web.Http.OData.Extensions;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Formatter;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;
using Core.DomainModel.Example;
using OS2Indberetning.Controllers;

namespace OS2Indberetning
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.MapHttpAttributeRoutes();

            config.EnableQuerySupport();

            config.Formatters.InsertRange(0, ODataMediaTypeFormatters.Create());

            config.MapODataServiceRoute(
                routeName: "odata",
                routePrefix: "odata",
                model: GetModel()
                );
        }

        public static Microsoft.OData.Edm.IEdmModel GetModel()
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();

            builder.EntitySet<TestReport>("TestReports");

            //EntitySetConfiguration<Person> persons = builder.EntitySet<Person>("Person");

            //FunctionConfiguration myFirstFunction = persons.EntityType.Collection.Function("MyFirstFunction");
            //myFirstFunction.ReturnsCollectionFromEntitySet<Person>("Person");

            

            return builder.GetEdmModel();
        }
    }
}
