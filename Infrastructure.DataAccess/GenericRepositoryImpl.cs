using System.Data.Entity;
using System.Linq;
using Core.DomainServices;

namespace Infrastructure.DataAccess
{
    public class GenericRepositoryImpl<T> : IGenericRepository<T> where T : class 
    {
        private readonly DbSet<T> _dbSet;
        private readonly DataContext _context;

        public GenericRepositoryImpl(DataContext context)
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
            _context.SaveChanges();
        }
    }
}
