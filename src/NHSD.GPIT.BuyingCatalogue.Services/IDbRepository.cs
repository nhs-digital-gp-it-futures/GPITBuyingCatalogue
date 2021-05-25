using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NHSD.GPIT.BuyingCatalogue.Services
{
    public interface IDbRepository<T, TDbContext>
        where T : class
        where TDbContext : DbContext
    {
        void Add(T item);

        void AddAll(IList<T> items);

        Task<T[]> GetAllAsync(Expression<Func<T, bool>> predicate);

        Task<T> SingleAsync(Expression<Func<T, bool>> predicate);

        void Remove(T item);

        Task SaveChangesAsync();
    }
}
