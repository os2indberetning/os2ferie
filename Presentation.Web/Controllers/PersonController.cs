using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.OData;
using System.Web.OData.Query;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;

namespace OS2Indberetning.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using Core.DomainModel;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Person>("Person");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */

    public class PersonController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        private readonly IGenericRepository<Person> _genericRepo;
        private readonly IPersonRepository _personRepo
;

        public PersonController()
        {
            _genericRepo = new GenericRepositoryImpl<Person>(new DataContext());
            _personRepo = new PersonRepository(new DataContext());
        }

        // GET: odata/Person
        [EnableQuery]
        public IQueryable<Person> GetPerson(ODataQueryOptions<Person> queryOptions)
        {
            return _genericRepo.AsQueryable();
        }

        // GET: odata/Person(5)
        [EnableQuery]
        public IQueryable<Person> GetPerson([FromODataUri] int key, ODataQueryOptions<Person> queryOptions)
        {
            return new List<Person>() { new Person() }.AsQueryable();
        }

        // PUT: odata/Person(5)
        [EnableQuery]
        public IQueryable<Person> Put([FromODataUri] int key, Delta<Person> delta)
        {
            throw new NotImplementedException();
        }

        // POST: odata/Person
        [EnableQuery]
        public IQueryable<Person> Post(Person person)
        {
            var result = _genericRepo.Insert(new Person()
            {
                CprNumber = "1234567890",
                FirstName = "Test",
                MiddleName = "Tester",
                LastName = "Testesen",
                Mail = "123@456.78",
                PersonId = 1234,
                WorkDistanceOverride = 0,
            });

            _genericRepo.Save();

            return new List<Person>() { result }.AsQueryable();
        }

        // PATCH: odata/Person(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public IQueryable<Person> Patch([FromODataUri] int key, Delta<Person> delta)
        {
            var existing = _genericRepo.AsQueryable().First(x => x.Id == key);

            var temp = delta.GetEntity();

            foreach (var propertyInfo in typeof(Person).GetProperties())
            {
                var itemType = existing.GetType();

                PropertyInfo prop;

                if (propertyInfo.Name == "Id")
                    continue; // skip primary key

                if (propertyInfo.GetValue(temp) != null)
                {
                    prop = itemType.GetProperty(propertyInfo.Name);

                    prop.SetValue(existing, propertyInfo.GetValue(temp));
                }
            }

            _genericRepo.Update(existing);
            _genericRepo.Save();            

            return new List<Person>() { existing }.AsQueryable();
        }

        // DELETE: odata/Person(5)
        [EnableQuery]
        public IQueryable<Person> Delete([FromODataUri] int key)
        {
            throw new NotImplementedException();
        }
    }
}
