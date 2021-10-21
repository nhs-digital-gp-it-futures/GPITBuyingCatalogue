using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;

namespace NHSD.GPIT.BuyingCatalogue.Services.ServiceLevelAgreements
{
    public sealed class ServiceLevelAgreementsService : IServiceLevelAgreementsService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public ServiceLevelAgreementsService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddServiceLevelAsync(AddSlaModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var solution = await dbContext.Solutions.SingleAsync(s => s.CatalogueItemId == model.Solution.Id);
            solution.ServiceLevelAgreement = new EntityFramework.Catalogue.Models.ServiceLevelAgreements
            {
                SlaType = model.SlaLevel,
            };

            await dbContext.SaveChangesAsync();
        }

        public async Task<EntityFramework.Catalogue.Models.ServiceLevelAgreements> GetAllServiceLevelAgreementsForSolution(CatalogueItemId solutionId)
        {
            return await dbContext.ServiceLevelAgreements
                .Include(s => s.Contacts)
                .Include(s => s.ServiceHours)
                .Include(s => s.ServiceLevels)
                .SingleOrDefaultAsync(s => s.SolutionId == solutionId);
        }

        public async Task UpdateServiceLevelTypeAsync(CatalogueItem solution, SlaType slaLevel)
        {
            var sla = await dbContext.ServiceLevelAgreements.SingleAsync(s => s.SolutionId == solution.Id);

            sla.SlaType = slaLevel;

            await dbContext.SaveChangesAsync();
        }
    }
}
