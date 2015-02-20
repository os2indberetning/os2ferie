using System;
using System.Data.Entity;
using System.Linq;
using Core.DomainServices;

namespace Infrastructure.DataAccess
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class 
    {
        private readonly DbSet<T> _dbSet;
        private readonly DataContext _context;

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
                var test = e.EntityValidationErrors;

                Console.WriteLine(e);
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
    }
}
