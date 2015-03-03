﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;

namespace OS2Indberetning.Controllers
{
    public class SubstitutesController : BaseController<Substitute>
    {
        SubstituteService _sub = new SubstituteService();

          //GET: odata/Substitutes
        public SubstitutesController(IGenericRepository<Substitute> repository) : base(repository){}

        [EnableQuery]
        public IQueryable<Substitute> Get(ODataQueryOptions<Substitute> queryOptions)
        {
            var res = GetQueryable(queryOptions);
            _sub.AddFullName(res);
            _sub.ScrubCprFromPersons(res);
            return res;
        }

        //GET: odata/Substitutes(5)
        public IQueryable<Substitute> Get([FromODataUri] int key, ODataQueryOptions<Substitute> queryOptions)
        {
            var res = GetQueryable(key, queryOptions);
            _sub.AddFullName(res);
            _sub.ScrubCprFromPersons(res);
            return res;
        }

        //PUT: odata/Substitutes(5)
        public new IHttpActionResult Put([FromODataUri] int key, Delta<Substitute> delta)
        {
            return base.Put(key, delta);
        }

        //POST: odata/Substitutes
        [EnableQuery]
        public new IHttpActionResult Post(Substitute Substitute)
        {
            var targets = Substitute.Persons;

            return base.Post(Substitute);
        }

        //PATCH: odata/Substitutes(5)
        [EnableQuery]
        [AcceptVerbs("PATCH", "MERGE")]
        public new IHttpActionResult Patch([FromODataUri] int key, Delta<Substitute> delta)
        {
            return base.Patch(key, delta);
        }

        //DELETE: odata/Substitutes(5)
        public new IHttpActionResult Delete([FromODataUri] int key)
        {
            return base.Delete(key);
        }
    }
}
