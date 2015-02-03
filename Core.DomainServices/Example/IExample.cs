using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel.Example;

namespace Core.DomainServices.Example
{
    public interface IExample<TEntity> where TEntity : Entity
    {
        TEntity Add(TEntity entity);
        TEntity Delete(Guid id);
        TEntity Get(Guid id);
        TEntity Update(TEntity entity);
        IQueryable<TEntity> Items { get; }
    }
}
