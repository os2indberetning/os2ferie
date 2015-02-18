using System.Linq;
using System.Reflection;

namespace Core.DomainServices
{
    public interface IGenericRepository<T>
    {
        /**
         * Inserts an entity into the repository.
         * The entity is first persisted when a call
         * to save occurs.
         */
        T Insert(T entity);        

        /**
         * As we are using a data store with entity framework
         * support, or rather one that implements IQueryable
         * we are just returning that instead of specific methods
         * for querying.
         */
        IQueryable<T> AsQueryable();
        void Save();
        void Update(T entity);
        void Patch(T entity);

        void Delete(T entity);

        PropertyInfo GetPrimaryKeyProperty();
    }
}
