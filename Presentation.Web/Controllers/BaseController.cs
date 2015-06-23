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
            string[] httpUser = @"syddjursnet\hshu".Split('\\'); // Fissirul Lehmann - administrator
#else
                string[] httpUser = User.Identity.Name.Split('\\');                
#endif

            if (httpUser.Length == 2 && String.Equals(httpUser[0], ConfigurationManager.AppSettings["PROTECTED_AD_DOMAIN"], StringComparison.CurrentCultureIgnoreCase))
            {
                var initials = httpUser[1].ToLower();
                // DEBUG ON PRODUCTION. Set petsoe = lky
                if (initials == "petsoe" || initials == "itmind" || initials == "jaoj" || initials == "mraitm") { initials = "hshu"; }
                // END DEBUG
                CurrentUser = _personRepo.AsQueryable().FirstOrDefault(p => p.Initials.ToLower().Equals(initials));
                if (CurrentUser == null)
                {
                    Logger.Error("AD-bruger ikke fundet i databasen (" + User.Identity.Name + ")");
                    throw new UnauthorizedAccessException("AD-bruger ikke fundet i databasen.");
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
            return _performODataQueryWithoutSelectAndExpand(Repo.AsQueryable(), queryOptions);
        }

        /// <summary>
        /// OData queries are only performed when our controllers have performed their tasks,
        /// meaning that they perform tasks on the entire dbset before it is filtered according
        /// to the odata query. We cannot just apply the odata query as select and expand will
        /// alter what it returns so it no longer matches the type T, as some fields will be missing
        /// or expanded.
        /// Therefore we construct a new odata query option that does not perform select and expand
        /// and filters the data according to that.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        private IQueryable<T> _performODataQueryWithoutSelectAndExpand(IQueryable<T> input,
            ODataQueryOptions<T> queryOptions)
        {
            var request = queryOptions.Request;
            var uri = request.RequestUri.AbsoluteUri;
            uri = Regex.Replace(uri, "\\$select=", ""); //Remove select query options
            uri = Regex.Replace(uri, "\\$expand=", ""); //Remove expand query options
            request.RequestUri = new Uri(uri);
            var newRequest = new HttpRequestMessage(request.Method, uri);
            var queryOptionsCopy = new ODataQueryOptions(queryOptions.Context, newRequest);

            var result = (IQueryable<T>)queryOptionsCopy.ApplyTo(input);
            result.Load(); //Make sure we close data readers as traversing the result will not work if we do not
            return result; 
        } 

        protected IQueryable<T> GetQueryable(int key, ODataQueryOptions<T> queryOptions)
        {
            var result = new List<T> { };
            var entity = Repo.AsQueryable().FirstOrDefault(PrimaryKeyEquals(_primaryKeyProp, key));
            if (entity != null)
            {
                result.Add(entity);
            }
            return _performODataQueryWithoutSelectAndExpand(result.AsQueryable(), queryOptions);
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