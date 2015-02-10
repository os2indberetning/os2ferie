using System.Data.Entity;
using System.Linq;
using Core.DomainModel;
using Core.DomainServices;

namespace Infrastructure.DataAccess
{
    public class PersonRepository : IPersonRepository
    {
        private readonly DbSet<Person> _dbSet;
        private readonly DataContext _context;

        public PersonRepository(DataContext context)
        {
            _context = context;
            _dbSet = context.Set<Person>();
        }

        public Person Insert(Person entity)
        {
            return _dbSet.Add(entity);
        }

        public IQueryable<Person> AsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Person entity)
        {
            throw new System.NotImplementedException();
        }

        public void Patch(Person entity)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateWorkDistanceOverride(int id, float newValue)
        {
            var existing = _dbSet.First(x => x.Id == id);

            if (existing != null)
            {
                existing.WorkDistanceOverride = newValue;

                Save();
            }
        }
    }
}