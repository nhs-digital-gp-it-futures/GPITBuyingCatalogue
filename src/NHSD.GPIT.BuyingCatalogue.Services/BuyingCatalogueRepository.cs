using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services
{
    [ExcludeFromCodeCoverage]
    public class BuyingCatalogueRepository<T> : BuyingCatalogueRepositoryBase<T>
        where T : class
    {
        private readonly ILogWrapper<SolutionsService> logger;
        private readonly DbSet<T> dbSet;

        public BuyingCatalogueRepository(ILogWrapper<SolutionsService> logger, BuyingCatalogueDbContext dbContext)
            : base(dbContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
