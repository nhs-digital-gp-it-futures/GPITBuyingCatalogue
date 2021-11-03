using System;
using System.Linq;
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

        public async Task<EntityFramework.Catalogue.Models.ServiceLevelAgreements> GetServiceLevelAgreementForSolution(CatalogueItemId solutionId)
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

        public async Task<ServiceAvailabilityTimes> GetServiceAvailabilityTimes(CatalogueItemId solutionId, int serviceAvailabilityTimesId)
            => await dbContext.ServiceAvailabilityTimes.SingleOrDefaultAsync(s => s.SolutionId == solutionId && s.Id == serviceAvailabilityTimesId);

        public async Task SaveServiceAvailabilityTimes(CatalogueItem solution, ServiceAvailabilityTimesModel model)
        {
            if (solution is null)
                throw new ArgumentNullException(nameof(solution));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var serviceAvailabilityTimes = new ServiceAvailabilityTimes
            {
                ApplicableDays = model.ApplicableDays,
                Category = model.SupportType,
                TimeFrom = model.From,
                TimeUntil = model.Until,
                SolutionId = solution.Id,
                LastUpdatedBy = model.UserId,
                LastUpdated = DateTime.UtcNow,
            };

            dbContext.ServiceAvailabilityTimes.Add(serviceAvailabilityTimes);

            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateServiceAvailabilityTimes(CatalogueItem solution, int serviceAvailabilityTimesId, ServiceAvailabilityTimesModel model)
        {
            if (solution is null)
                throw new ArgumentNullException(nameof(solution));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var serviceAvailabilityTimes = await GetServiceAvailabilityTimes(solution.Id, serviceAvailabilityTimesId);

            serviceAvailabilityTimes.ApplicableDays = model.ApplicableDays;
            serviceAvailabilityTimes.Category = model.SupportType;
            serviceAvailabilityTimes.TimeFrom = model.From;
            serviceAvailabilityTimes.TimeUntil = model.Until;
            serviceAvailabilityTimes.LastUpdatedBy = model.UserId;
            serviceAvailabilityTimes.LastUpdated = DateTime.UtcNow;

            dbContext.ServiceAvailabilityTimes.Update(serviceAvailabilityTimes);

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteServiceAvailabilityTimes(CatalogueItemId solutionId, int serviceAvailabilityTimesId)
        {
            var serviceAvailabilityTimes = await GetServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId);

            dbContext.ServiceAvailabilityTimes.Remove(serviceAvailabilityTimes);

            await dbContext.SaveChangesAsync();
        }

        public async Task<int> GetCountOfServiceAvailabilityTimes(params int[] idsToExclude)
            => await dbContext.ServiceAvailabilityTimes.Where(s => !idsToExclude.Contains(s.Id)).CountAsync();
    }
}
