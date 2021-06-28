using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NHSD.GPIT.BuyingCatalogue.Services
{
    public class DbRepository<T, TDbContext> : DbRepositoryBase<T, TDbContext>
        where T : class
        where TDbContext : DbContext
    {
        private readonly DbSet<T> dbSet;

        public DbRepository(TDbContext dbContext)
            : base(dbContext)
        {
            dbSet = dbContext.Set<T>();
        }

        public override void Add(T item) => dbSet.Add(item);

        public override void AddAll(IList<T> items)
        {
            foreach (var item in items)
            {
                dbSet.Add(item);
            }
        }

        public override async Task<T[]> GetAllAsync(Expression<Func<T, bool>> predicate) =>
            await dbSet.Where(predicate).ToArrayAsync();

        public override async Task<T> SingleAsync(Expression<Func<T, bool>> predicate) =>
            await dbSet.SingleAsync(predicate);

        public override void Remove(T item) => dbSet.Remove(item);
    }
}
