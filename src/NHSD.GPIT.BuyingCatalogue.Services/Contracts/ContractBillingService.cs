using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Contracts
{
    public sealed class ContractBillingService : IContractBillingService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public ContractBillingService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Contract> AddContractBilling(int orderId, int contractId)
        {
            var contract = await dbContext.Contracts
                .Include(x => x.Order)
                    .ThenInclude(x => x.OrderItems)
                        .ThenInclude(x => x.CatalogueItem)
                .Include(x => x.ContractBilling)
                .FirstOrDefaultAsync(o => o.Id == contractId && o.OrderId == orderId) ?? throw new ArgumentException("Invalid contract", nameof(contractId));

            if (contract.ContractBilling is null)
            {
                contract.ContractBilling = new ContractBilling();
                await dbContext.SaveChangesAsync();
            }

            return contract;
        }

        public async Task AddBespokeContractBillingItem(int orderId, int contractId, CatalogueItemId catalogueItemId, string name, string paymentTrigger, int quantity)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(paymentTrigger))
                throw new ArgumentNullException(nameof(paymentTrigger));

            var contract = await AddContractBilling(orderId, contractId);

            var associatedService = contract.Order.GetAssociatedService(catalogueItemId) ?? throw new ArgumentException("Invalid associated service", nameof(catalogueItemId));

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
                .Include(x => x.Milestone)
                .Include(x => x.OrderItem)
                    .ThenInclude(x => x.CatalogueItem)
                .FirstOrDefaultAsync(x => x.Id == itemId && x.ContractBilling.Contract.OrderId == orderId);
        }

        public async Task EditContractBillingItem(int orderId, int itemId, CatalogueItemId catalogueItemId, string name, string paymentTrigger, int quantity)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(paymentTrigger))
                throw new ArgumentNullException(nameof(paymentTrigger));

            var item = await GetContractBillingItem(orderId, itemId);

            if (item == null)
            {
                return;
            }

            var order = await dbContext.Orders
                    .Include(x => x.OrderItems)
                    .ThenInclude(x => x.CatalogueItem)
                .FirstOrDefaultAsync(x => x.Id == orderId) ?? throw new ArgumentException("Invalid order", nameof(orderId));

            var associatedService = order.GetAssociatedService(catalogueItemId) ?? throw new ArgumentException("Invalid associated service", nameof(catalogueItemId));

            item.OrderItem = associatedService;
            item.Milestone.Title = name;
            item.Milestone.PaymentTrigger = paymentTrigger;
            item.Quantity = quantity;

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteContractBillingItem(int orderId, int itemId)
        {
            var item = await dbContext.ImplementationPlanMilestones
                .Include(x => x.ContractBillingItem)
                .FirstOrDefaultAsync(x => x.ContractBillingItem.Id == itemId && x.ContractBillingItem.ContractBilling.Contract.OrderId == orderId);

            if (item == null)
            {
                return;
            }

            dbContext.ImplementationPlanMilestones.Remove(item);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteContractBillingItems(int orderId, IEnumerable<CatalogueItemId> catalogueItemIds)
        {
            var items = dbContext.ContractBillingItems
                .Where(x => x.ContractBilling.Contract.OrderId == orderId && catalogueItemIds.Contains(x.CatalogueItemId));

            if (items.Any())
            {
                foreach (var item in items)
                {
                    dbContext.ContractBillingItems.Remove(item);
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
