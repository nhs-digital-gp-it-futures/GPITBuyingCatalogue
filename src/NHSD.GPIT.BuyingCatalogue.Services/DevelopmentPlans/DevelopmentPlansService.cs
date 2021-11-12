using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.DevelopmentPlans;

namespace NHSD.GPIT.BuyingCatalogue.Services.DevelopmentPlans
{
    public sealed class DevelopmentPlansService : IDevelopmentPlansService
    {
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly IDbRepository<Solution, BuyingCatalogueDbContext> solutionRepository;

        public DevelopmentPlansService(
            BuyingCatalogueDbContext dbContext,
            IDbRepository<Solution, BuyingCatalogueDbContext> solutionRepository)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.solutionRepository = solutionRepository ?? throw new ArgumentNullException(nameof(solutionRepository));
        }

        public async Task SaveDevelopmentPlans(CatalogueItemId solutionId, string developmentPlan)
        {
            var solution = await solutionRepository.SingleAsync(s => s.CatalogueItemId == solutionId);
            solution.RoadMap = developmentPlan;
            await solutionRepository.SaveChangesAsync();
        }

        public async Task<List<WorkOffPlan>> GetWorkOffPlans(CatalogueItemId solutionId) =>
            await dbContext.WorkOffPlans.Where(wp => wp.SolutionId == solutionId).ToListAsync();

        public async Task<WorkOffPlan> GetWorkOffPlan(int id) =>
            await dbContext.WorkOffPlans.SingleOrDefaultAsync(wp => wp.Id == id);

        public async Task SaveWorkOffPlan(CatalogueItemId solutionId, SaveWorkOffPlanModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var solution = await dbContext.Solutions
                .Include(s => s.WorkOffPlans)
                .FirstOrDefaultAsync(s => s.CatalogueItemId == solutionId);

            var workOffPlan = new WorkOffPlan
            {
                SolutionId = solutionId,
                StandardId = model.StandardId,
                Details = model.Details,
                CompletionDate = model.CompletionDate,
            };

            solution.WorkOffPlans.Add(workOffPlan);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateWorkOffPlan(int id, SaveWorkOffPlanModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var existingPlan = await dbContext.WorkOffPlans.FirstOrDefaultAsync(wp => wp.Id == id);

            existingPlan.StandardId = model.StandardId;
            existingPlan.Details = model.Details;
            existingPlan.CompletionDate = model.CompletionDate;

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteWorkOffPlan(int id)
        {
            var existingPlan = await dbContext.WorkOffPlans.FirstOrDefaultAsync(wp => wp.Id == id);

            dbContext.WorkOffPlans.Remove(existingPlan);

            await dbContext.SaveChangesAsync();
        }
    }
}
