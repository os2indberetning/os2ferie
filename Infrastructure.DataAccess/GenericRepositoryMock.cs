using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.DomainServices;

namespace Presentation.Web.Test.Controllers
{
    public abstract class GenericRepositoryMock<T> : IGenericRepository<T>
    {
        private static List<T> _list;
        private static bool isSeeded = false;

        public GenericRepositoryMock()
        {
            if (!isSeeded)
            {
                ReSeed();
                isSeeded = true;
            }
        }

        public void ReSeed()
        {
            _list = Seed();
        }

        protected abstract List<T> Seed();

        public virtual T Insert(T entity)
        {
            _list.Add(entity);
            return entity;
        }

        public IQueryable<T> AsQueryable()
        {
            return _list.AsQueryable();
        }

        public virtual void Save()
        {

        }

        public void Update(T entity)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                var item = _list[i];
                if (getId(item) == getId(entity))
                {
                    _list.Remove(item);
                    _list.Add(entity);
                    return;
                }
            }
        }

        public void Patch(T entity)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                var item = _list[i];
                if (getId(item) == getId(entity))
                {
                    _list.Remove(item);
                    _list.Add(entity);
                    return;
                }
            }
        }

        public void Delete(T entity)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                var item = _list[i];
                if (getId(item) == getId(entity))
                {
                    _list.Remove(item);
                    return;
                }
            }
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _list.Clear();
        }

        public PropertyInfo GetPrimaryKeyProperty()
        {
            return typeof(T).GetProperty("Id");
        }

        private int getId(T entity)
        {
            return (int)entity.GetType().GetProperty("Id").GetValue(entity);
        }
    }
}
