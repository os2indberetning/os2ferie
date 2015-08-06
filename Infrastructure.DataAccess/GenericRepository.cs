using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            //There is a problem that an unattached navigation property will
            //be created as a new entity, so we attach all non value types to the
            //contexts db sets.
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                if (propertyInfo.GetValue(entity) != null && propertyInfo.PropertyType == typeof(object))
                {
                    var navProperty = propertyInfo.GetValue(entity);
                    _context.Set(propertyInfo.GetType()).Attach(navProperty);
                }                
                else if (propertyInfo.PropertyType.IsGenericType)
                {
                    if (propertyInfo.GetValue(entity) == null)
                    {
                        continue;
                    }
                    var collection = propertyInfo.GetValue(entity) as ICollection;
                    if (collection == null)
                    {
                        continue;
                    }   
                    foreach (var obj in collection)
                    {
                        if (GetPrimaryKeyValue(obj) == 0) //If ID == 0; This is a new object and should be added
                        {
                            _context.Set(obj.GetType()).Add(obj);
                        }
                        else
                        {
                            _context.Set(obj.GetType()).Attach(obj);
                        }
                    }
                }
            }

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

                if (propertyInfo.GetValue(entity) != null && propertyInfo.PropertyType.IsValueType)
                    entry.Property(propertyInfo.Name).IsModified = true;
            }
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        private int GetPrimaryKeyValue(Object obj)
        {
            var t = obj.GetType();

            IEnumerable<dynamic> keyMembers = getKeyMenbers(t);

            var primaryKeyName = keyMembers.Select(k => (string)k.Name).First();
            
            return (int)t.GetProperty(primaryKeyName).GetValue(obj);
        }

        private IEnumerable<dynamic> getKeyMenbers(Type t)
        {

            while (t.BaseType != typeof(object))
                t = t.BaseType;

            var objectContext = ((IObjectContextAdapter)_context).ObjectContext;

            //create method CreateObjectSet with the generic parameter of the base-type
            var method = typeof(ObjectContext)
                                      .GetMethod("CreateObjectSet", Type.EmptyTypes)
                                      .MakeGenericMethod(t);
            dynamic objectSet = method.Invoke(objectContext, null);

            return objectSet.EntitySet.ElementType.KeyMembers;
        } 

        public PropertyInfo GetPrimaryKeyProperty()
        {
            var t = typeof(T);

            if (_primaryKeyName == null)
            {
                IEnumerable<dynamic> keyMembers = getKeyMenbers(t);

                _primaryKeyName = keyMembers.Select(k => (string)k.Name).First();
            }
            return t.GetProperty(_primaryKeyName);
        }
    }
}
