using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.Http.OData.Routing;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;
using Microsoft.Data.OData;

namespace OS2Indberetning.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using Core.DomainModel;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<MobileToken>("MobileToken");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class MobileTokenController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        private readonly IGenericRepository<MobileToken> _genericRepo;
        private readonly IMobileTokenService _service;

        public MobileTokenController()
        {
            _genericRepo = new GenericRepository<MobileToken>(new DataContext());
            _service = new MobileTokenService(_genericRepo);
        }

        // GET: odata/MobileToken
        [EnableQuery]
        public IQueryable<MobileToken> GetMobileToken(ODataQueryOptions<MobileToken> queryOptions)
        {
            throw new NotImplementedException();
        }

        // GET: odata/MobileToken(5)
        [EnableQuery]
        public IQueryable<MobileToken> GetMobileToken([FromODataUri] int key, ODataQueryOptions<MobileToken> queryOptions)
        {
            var result = _service.GetByPersonId(key);

            return result;
        }

        // PUT: odata/MobileToken(5)
        [EnableQuery]
        public IQueryable<MobileToken> Put([FromODataUri] int key, Delta<MobileToken> delta)
        {
            throw new NotImplementedException();
        }

        // POST: odata/MobileToken
        [EnableQuery]
        public IQueryable<MobileToken> Post(MobileToken mobileToken)
        {
            var result = _service.Create(mobileToken);

            return new List<MobileToken>() { result }.AsQueryable();
        }

        // PATCH: odata/MobileToken(5)
        [AcceptVerbs("PATCH", "MERGE")]
        [EnableQuery]
        public IQueryable<MobileToken> Patch([FromODataUri] int key, Delta<MobileToken> delta)
        {
            throw new NotImplementedException();
        }

        // DELETE: odata/MobileToken(5)
        [EnableQuery]
        public IQueryable<MobileToken> Delete([FromODataUri] int key)
        {
            var entity = _genericRepo.AsQueryable().Where(x => x.Id == key);

            if (entity.Any())
            {
                var result = _service.Delete(entity.First());

                if (result)
                {
                    return entity;
                }    
            }
            
            return new List<MobileToken>() { }.AsQueryable();
        }
    }
}
