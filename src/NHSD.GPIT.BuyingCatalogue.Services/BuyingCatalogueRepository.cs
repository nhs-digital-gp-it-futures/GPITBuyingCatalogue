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
    public class BuyingCatalogueRepository<T> : BuyingCatalogueRepositoryBase<T> where T : class
    {
        private readonly ILogWrapper<SolutionsService> _logger;
        private readonly DbSet<T> _dbSet;

        public BuyingCatalogueRepository(ILogWrapper<SolutionsService> logger, BuyingCatalogueDbContext dbContext) : base(dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbSet = dbContext.Set<T>();
        }

        public override void Add(T item) => _dbSet.Add(item);

        public override void AddAll(IList<T> items)
        {
            foreach (var item in items)
            {
                _dbSet.Add(item);
            }
        }

        public override async Task<T[]> GetAllAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.Where(predicate).ToArrayAsync();

        public override async Task<T> SingleAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.SingleAsync(predicate);

        public override void Remove(T item) => _dbSet.Remove(item);
    }
}