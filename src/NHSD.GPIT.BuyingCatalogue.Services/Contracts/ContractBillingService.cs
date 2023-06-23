using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;

namespace NHSD.GPIT.BuyingCatalogue.Services.Contracts
{
    public sealed class ContractBillingService : IContractBillingService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public ContractBillingService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddContractBilling(int orderId, int contractId)
        {
            var contract = await dbContext.Contracts.Include(x => x.ImplementationPlan)
                .FirstOrDefaultAsync(o => o.Id == contractId && o.OrderId == orderId) ?? throw new ArgumentException("Invalid contract", nameof(contractId));

            if (contract.ContractBilling != null)
            {
                return;
            }

            contract.ContractBilling = new ContractBilling();
            await dbContext.SaveChangesAsync();
        }

        public async Task AddBespokeContractBillingItem(int orderId, int contractId, CatalogueItemId catalogueItemId, string name, string paymentTrigger, int quantity)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(paymentTrigger))
                throw new ArgumentNullException(nameof(paymentTrigger));

            var contract = await dbContext.Contracts.Include(x => x.ContractBilling)
                    .FirstOrDefaultAsync(o => o.Id == contractId && o.OrderId == orderId) ?? throw new ArgumentException("Invalid contract", nameof(contractId));

            var associatedService = contract.Order.GetAssociatedService(catalogueItemId) ?? throw new ArgumentException("Invalid associated service", nameof(catalogueItemId));

            if (contract.ContractBilling == null)
            {
                contract.ContractBilling = new ContractBilling();
                await dbContext.SaveChangesAsync();
            }

            contract.ContractBilling.ContractBillingItems.Add(new ContractBillingItem()
            {
                OrderItem = associatedService,
                Milestone = new ImplementationPlanMilestone() { Title = name, PaymentTrigger = paymentTrigger, },
                Quantity = quantity,
            });
            await dbContext.SaveChangesAsync();
        }

        public async Task<ContractBillingItem> GetContractBillingItem(int orderId, int itemId)
        {
            return await dbContext.ContractBillingItems
                .FirstOrDefaultAsync(x => x.Id == itemId && x.ContractBilling.Contract.OrderId == orderId);
        }

        public async Task EditContractBillingItem(int orderId, int itemId, CatalogueItemId catalogueItemId, string name, string paymentTrigger, int quantity)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(paymentTrigger))
                throw new ArgumentNullException(nameof(paymentTrigger));

            var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId) ?? throw new ArgumentException("Invalid order", nameof(orderId));

            var associatedService = order.GetAssociatedService(catalogueItemId) ?? throw new ArgumentException("Invalid associated service", nameof(catalogueItemId));

            var item = await GetContractBillingItem(orderId, itemId);

            if (item == null)
            {
                return;
            }

            item.OrderItem = associatedService;
            item.Milestone.Title = name;
            item.Milestone.PaymentTrigger = paymentTrigger;
            item.Quantity = quantity;

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteContractBillingItem(int orderId, int itemId)
        {
            var item = await GetContractBillingItem(orderId, itemId);

            if (item == null)
            {
                return;
            }

            dbContext.ContractBillingItems.Remove(item);
            await dbContext.SaveChangesAsync();
        }
    }
}
