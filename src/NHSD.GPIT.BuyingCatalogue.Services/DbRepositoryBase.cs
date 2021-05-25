using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NHSD.GPIT.BuyingCatalogue.Services
{
    public abstract class DbRepositoryBase<T, TDbContext> : IDbRepository<T, TDbContext>
        where TDbContext : DbContext
        where T : class
    {
        private readonly TDbContext dbContext;

        protected DbRepositoryBase(TDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public abstract void Add(T item);

        public abstract void AddAll(IList<T> items);

        public abstract Task<T[]> GetAllAsync(Expression<Func<T, bool>> predicate);

        public abstract Task<T> SingleAsync(Expression<Func<T, bool>> predicate);

        public abstract void Remove(T item);

        public async Task SaveChangesAsync() => await dbContext.SaveChangesAsync();
    }
}
