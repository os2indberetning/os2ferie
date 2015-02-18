using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using Core.DomainServices;

namespace Infrastructure.DataAccess
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class 
    {
        private readonly DbSet<T> _dbSet;
        private readonly DataContext _context;
        private string _primaryKeyName;

        public GenericRepository(DataContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        
        public T Insert(T entity)
        {
            return _dbSet.Add(entity);
        }

        public IQueryable<T> AsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                Console.WriteLine(e);
                throw e;
            }     
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Patch(T entity)
        {
            _context.Configuration.ValidateOnSaveEnabled = false;
            _dbSet.Attach(entity);
            var entry = _context.Entry(entity);
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                if (propertyInfo.Name == "Id")
                    continue; // skip primary key

                if (propertyInfo.GetValue(entity) != null)
                    entry.Property(propertyInfo.Name).IsModified = true;
            }
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public PropertyInfo GetPrimaryKeyProperty()
        {
            var t = typeof(T);

            if (_primaryKeyName == null)
            {
                //retrieve the base type
                while (t.BaseType != typeof(object))
                    t = t.BaseType;

                var objectContext = ((IObjectContextAdapter)_context).ObjectContext;

                //create method CreateObjectSet with the generic parameter of the base-type
                var method = typeof(ObjectContext)
                                          .GetMethod("CreateObjectSet", Type.EmptyTypes)
                                          .MakeGenericMethod(t);
                dynamic objectSet = method.Invoke(objectContext, null);

                IEnumerable<dynamic> keyMembers = objectSet.EntitySet.ElementType.KeyMembers;

                _primaryKeyName = keyMembers.Select(k => (string)k.Name).First();
            }
            return t.GetProperty(_primaryKeyName);
        }
    }
}
