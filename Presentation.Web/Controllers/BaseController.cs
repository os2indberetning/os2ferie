using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.Routing;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainModel.Example;
using Core.DomainServices;
using log4net;
using Ninject;
using Expression = System.Linq.Expressions.Expression;

namespace OS2Indberetning.Controllers
{
    public class BaseController<T> : ODataController where T : class
    {
        protected ODataValidationSettings ValidationSettings = new ODataValidationSettings();
        protected IGenericRepository<T> Repo;
        private readonly IGenericRepository<Person> _personRepo;
        private readonly PropertyInfo _primaryKeyProp;

        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected Person CurrentUser;

        protected override void Initialize(HttpControllerContext requestContext)
        {
            base.Initialize(requestContext);

#if DEBUG
            string[] httpUser = @"syddjursnet\jbp".Split('\\'); // Fissirul Lehmann - administrator
#else
                string[] httpUser = User.Identity.Name.Split('\\');                
#endif

            if (httpUser.Length == 2 && String.Equals(httpUser[0], ConfigurationManager.AppSettings["PROTECTED_AD_DOMAIN"], StringComparison.CurrentCultureIgnoreCase))
            {
                var initials = httpUser[1].ToLower();
                // DEBUG ON PRODUCTION. Set petsoe = lky
                if (initials == "itmind" || initials == "jaoj" || initials == "mraitm") { initials = "jbp"; }
                // END DEBUG
                CurrentUser = _personRepo.AsQueryable().FirstOrDefault(p => p.Initials.ToLower().Equals(initials));
                if (CurrentUser == null)
                {
                    Logger.Error("AD-bruger ikke fundet i databasen (" + User.Identity.Name + ")");
                    throw new UnauthorizedAccessException("AD-bruger ikke fundet i databasen.");
                }
                if (!CurrentUser.IsActive)
                {
                    Logger.Error("Inaktiv bruger forsøgte at logge ind (" + User.Identity.Name + ")");
                    throw new UnauthorizedAccessException("Inaktiv bruger forsøgte at logge ind.");
                }
            }
            else
            {
                Logger.Info("Gyldig domænebruger ikke fundet (" + User.Identity.Name + ")");
                throw new UnauthorizedAccessException("Gyldig domænebruger ikke fundet.");
            }
        }

        public BaseController(IGenericRepository<T> repository, IGenericRepository<Person> personRepo)
        {
            _personRepo = personRepo;
            ValidationSettings.AllowedQueryOptions = AllowedQueryOptions.All;
            ValidationSettings.MaxExpansionDepth = 4;
            Repo = repository;
            _primaryKeyProp = Repo.GetPrimaryKeyProperty();
        }

        protected IQueryable<T> GetQueryable(ODataQueryOptions<T> queryOptions)
        {
            if (queryOptions.Filter != null) { 
                return (IQueryable<T>)queryOptions.Filter.ApplyTo(Repo.AsQueryable(), new ODataQuerySettings());
            }
            return Repo.AsQueryable();        
        }

        protected IQueryable<T> GetQueryable(int key, ODataQueryOptions<T> queryOptions)
        {
            var result = new List<T> { };
            var entity = Repo.AsQueryable().FirstOrDefault(PrimaryKeyEquals(_primaryKeyProp, key));
            if (entity != null)
            {
                result.Add(entity);
            }
            if (queryOptions.Filter != null)
            {
                return (IQueryable<T>) queryOptions.Filter.ApplyTo(result.AsQueryable(), new ODataQuerySettings());
            }
            return result.AsQueryable();        
        }

        protected IHttpActionResult Put(int key, Delta<T> delta)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        protected IHttpActionResult Post(T entity)
        {
            Validate(entity);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                entity = Repo.Insert(entity);
                Repo.Save();
                return Created(entity);
            }
            catch (Exception e)
            {
                Logger.Error("Exception doing post of type " + typeof(T), e);
                return InternalServerError(e);
            }
        }

        protected IHttpActionResult Patch(int key, Delta<T> delta)
        {
            //Validate(delta.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = Repo.AsQueryable().FirstOrDefault(PrimaryKeyEquals(_primaryKeyProp, key));
            if (entity == null)
            {
                return BadRequest("Unable to find entity with id " + key);
            }

            try
            {
                delta.Patch(entity);
                Repo.Save();
            }
            catch (Exception e)
            {
                Logger.Error("Exception doing patch of type " + typeof(T), e);
                return InternalServerError(e);
            }

            return Updated(entity);
        }

        protected IHttpActionResult Delete(int key)
        {
            var entity = Repo.AsQueryable().FirstOrDefault(PrimaryKeyEquals(_primaryKeyProp, key));
            if (entity == null)
            {
                return BadRequest("Unable to find entity with id " + key);
            }
            try
            {
                Repo.Delete(entity);
                Repo.Save();
            }
            catch (Exception e)
            {
                Logger.Error("Exception doing delete", e);
                return InternalServerError(e);
            }
            return Ok();
        }

        private static Expression<Func<T, bool>> PrimaryKeyEquals(PropertyInfo property, int value)
        {
            var param = Expression.Parameter(typeof(T));
            var body = Expression.Equal(Expression.Property(param, property), Expression.Constant(value));
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}