using System;
using System.Collections.Concurrent;
using System.Linq;
using Core.DomainModel.Example;
using Core.DomainServices.Example;

namespace Infrastructure.DataAccess.Example
{
    public class Example<TEntity> : IExample<TEntity> where TEntity : Entity
    {
        private readonly ConcurrentDictionary<Guid, TEntity> _concurrentDictionary
            = new ConcurrentDictionary<Guid, TEntity>();

        public TEntity Add(TEntity entity)
        {
            if (entity == null)
            {
                //we dont want to store nulls in our collection
                throw new ArgumentNullException("entity");
            }

            if (entity.Id == Guid.Empty)
            {
                //we assume no Guid collisions will occur
                entity.Id = Guid.NewGuid();
            }

            if (_concurrentDictionary.ContainsKey(entity.Id))
            {
                return null;
            }

            bool result = _concurrentDictionary.TryAdd(entity.Id, entity);

            if (result == false)
            {
                return null;
            }
            return entity;
        }

        public TEntity Delete(Guid id)
        {
            TEntity removed;
            if (!_concurrentDictionary.ContainsKey(id))
            {
                return null;
            }
            bool result = _concurrentDictionary.TryRemove(id, out removed);
            if (!result)
            {
                return null;
            }
            return removed;
        }

        public TEntity Get(Guid id)
        {
            if (!_concurrentDictionary.ContainsKey(id))
            {
                return null;
            }
            TEntity entity;
            bool result = _concurrentDictionary.TryGetValue(id, out entity);
            if (!result)
            {
                return null;
            }
            return entity;
        }

        public TEntity Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (!_concurrentDictionary.ContainsKey(entity.Id))
            {
                return null;
            }
            _concurrentDictionary[entity.Id] = entity;
            return entity;
        }

        public IQueryable<TEntity> Items
        {
            get { return _concurrentDictionary.Values.AsQueryable(); }
        }
    }
}