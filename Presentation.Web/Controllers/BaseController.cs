using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.DomainServices;
using Infrastructure.DataAccess;
using Expression = System.Linq.Expressions.Expression;

namespace OS2Indberetning.Controllers
{
    public class BaseController<T> : ODataController where T : class
    {
        protected static ODataValidationSettings _validationSettings = new ODataValidationSettings();
        protected IGenericRepository<T> _repo = new GenericRepository<T>(new DataContext());

        private readonly PropertyInfo _primaryKeyProp;



        protected BaseController()
        {
            _validationSettings.AllowedQueryOptions = AllowedQueryOptions.All;
            _primaryKeyProp = _repo.GetPrimaryKeyProperty();
        }

        protected IQueryable<T> GetQueryable(ODataQueryOptions<T> queryOptions)
        {
            return _repo.AsQueryable();
        }

        protected IQueryable<T> GetQueryable(int key, ODataQueryOptions<T> queryOptions)
        {
            var result = new List<T>() {_repo.AsQueryable().FirstOrDefault(BaseController<T>.PrimaryKeyEquals(_primaryKeyProp, key))}.AsQueryable();
            return result;
        }

        protected IHttpActionResult Put( int key, Delta<T> delta)
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
                entity = _repo.Insert(entity);
                _repo.Save();
                return Created(entity);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        protected IHttpActionResult Patch(int key, Delta<T> delta)
        {
            Validate(delta.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = _repo.AsQueryable().FirstOrDefault(BaseController<T>.PrimaryKeyEquals(_primaryKeyProp, key));
            if (entity == null)
            {
                return BadRequest("Unable to find entity with id " + key);
            }

            try
            {
                delta.Patch(entity);
                _repo.Save();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

            return Updated(entity);
        }

        protected IHttpActionResult Delete(int key)
        {
            var entity = _repo.AsQueryable().FirstOrDefault(BaseController<T>.PrimaryKeyEquals(_primaryKeyProp, key));
            if (entity == null)
            {
                return BadRequest("Unable to find entity with id " + key);
            }
            try
            {
                _repo.Delete(entity);
                _repo.Save();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            return Ok();
        }

        private static Expression<Func<T, bool>> PrimaryKeyEquals(PropertyInfo property, int value)
        {
            var param = Expression.Parameter(typeof (T));
            var body = Expression.Equal(Expression.Property(param, property), Expression.Constant(value));
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}