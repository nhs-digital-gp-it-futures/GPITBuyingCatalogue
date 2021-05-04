using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.Services
{
    public interface IBuyingCatalogueRepository<T> where T : class
    {
        void Add(T item);

        void AddAll(IList<T> items);
        
        Task<T[]> GetAllAsync(Expression<Func<T, bool>> predicate);

        Task<T> SingleAsync(Expression<Func<T, bool>> predicate);

        void Remove(T item);
        
        Task SaveChangesAsync();
    }
}