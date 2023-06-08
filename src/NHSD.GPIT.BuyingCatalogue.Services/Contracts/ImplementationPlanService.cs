﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Contracts
{
    public class ImplementationPlanService : IImplementationPlanService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public ImplementationPlanService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<ImplementationPlan> GetDefaultImplementationPlan()
        {
            return await dbContext.ImplementationPlans
                .Include(x => x.Milestones.OrderBy(m => m.Order))
                .ThenInclude(x => x.AcceptanceCriteria)
                .FirstOrDefaultAsync(x => x.IsDefault == true);
        }

        public async Task<int> AddBespokeMilestone(int contractId, string name, string paymentTrigger)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(paymentTrigger))
                throw new ArgumentNullException(nameof(paymentTrigger));

            var contract = await dbContext.Contracts.Include(x => x.ImplementationPlan)
                    .FirstOrDefaultAsync(o => o.Id == contractId) ?? throw new ArgumentException("Invalid contract", nameof(contractId));

            if (contract.ImplementationPlan == null)
            {
                var implementationPlan = new ImplementationPlan() { IsDefault = false, };
                contract.ImplementationPlan = implementationPlan;
                await dbContext.SaveChangesAsync();
            }

            contract.ImplementationPlan.Milestones.Add(new ImplementationPlanMilestone() { Title = name, PaymentTrigger = paymentTrigger });
            await dbContext.SaveChangesAsync();

            return contract.ImplementationPlan.Id;
        }
    }
}
