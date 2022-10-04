using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;

namespace NHSD.GPIT.BuyingCatalogue.Services.Framework
{
    public class FrameworkService : IFrameworkService
    {
        public const string CovidFrameworkId = "COVID";

        private readonly BuyingCatalogueDbContext dbContext;

        public FrameworkService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<EntityFramework.Catalogue.Models.Framework> GetFramework(string frameworkId) =>
            await dbContext.Frameworks.SingleOrDefaultAsync(f => f.Id == frameworkId);

        public async Task<EntityFramework.Catalogue.Models.Framework> GetFramework(int orderId)
        {
            var order = await dbContext.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.CatalogueItem)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == orderId);

            var solutionId = order?.GetSolutionId();

            if (solutionId == null)
            {
                return null;
            }

            var solution = await dbContext.FrameworkSolutions
                .Include(x => x.Framework)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SolutionId == solutionId
                    && x.FrameworkId != CovidFrameworkId);

            return solution?.Framework;
        }
    }
}
