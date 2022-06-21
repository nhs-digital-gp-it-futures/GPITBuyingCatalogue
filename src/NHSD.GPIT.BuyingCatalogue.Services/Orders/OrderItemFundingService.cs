using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public class OrderItemFundingService : IOrderItemFundingService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public OrderItemFundingService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<OrderItemFundingType> GetFundingType(OrderItem item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            if (item.OrderItemPrice.CalculateTotalCost(item.GetQuantity()) == 0)
                return OrderItemFundingType.NoFundingRequired;

            if (item.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService)
                return OrderItemFundingType.None;

            if (await IsLocallyFundedSolution(item)
                || await IsLocallyFundedAdditionalService(item))
                return OrderItemFundingType.LocalFundingOnly;

            return OrderItemFundingType.None;
        }

        private async Task<bool> IsLocallyFundedAdditionalService(OrderItem orderItem)
        {
            if (orderItem.CatalogueItem.CatalogueItemType != CatalogueItemType.AdditionalService)
                return false;

            return await dbContext.CatalogueItems.AnyAsync(ci => ci.Id == orderItem.CatalogueItemId
                && ci.CatalogueItemType == CatalogueItemType.AdditionalService
                && ci.AdditionalService.Solution.FrameworkSolutions.All(fs => fs.Framework.LocalFundingOnly));
        }

        private async Task<bool> IsLocallyFundedSolution(OrderItem orderItem)
        {
            if (orderItem.CatalogueItem.CatalogueItemType != CatalogueItemType.Solution)
                return false;

            return await dbContext.CatalogueItems.AnyAsync(ci => ci.Id == orderItem.CatalogueItemId
                && ci.CatalogueItemType == CatalogueItemType.Solution
                && ci.Solution.FrameworkSolutions.All(fs => fs.Framework.LocalFundingOnly));
        }
    }
}
