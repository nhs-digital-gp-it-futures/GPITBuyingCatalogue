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
        public DbRepository(TDbContext dbContext)
            : base(dbContext)
        {
            DbSet = dbContext.Set<T>();
        }

        protected DbSet<T> DbSet { get; }

        public override void Add(T item) => DbSet.Add(item);

        public override void AddAll(IList<T> items)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                DbSet.Add(item);
            }
        }

        public override async Task<T[]> GetAllAsync(Expression<Func<T, bool>> predicate) =>
            await DbSet.Where(predicate).ToArrayAsync();

        public override async Task<T> SingleAsync(Expression<Func<T, bool>> predicate) =>
            await DbSet.SingleAsync(predicate);

        public override void Remove(T item) => DbSet.Remove(item);
    }
}
