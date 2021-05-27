using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public class SupplierService : ISupplierService
    {
        private readonly ILogWrapper<SupplierService> logger;
        private readonly OrderingDbContext oDbContext;
        private readonly BuyingCatalogueDbContext bcDbContext;
        private readonly IDbRepository<Order, OrderingDbContext> orderRepository;
        private readonly IDbRepository<EntityFramework.Models.BuyingCatalogue.Supplier, BuyingCatalogueDbContext> bcRepository;

        public SupplierService(
            ILogWrapper<SupplierService> logger,
            OrderingDbContext oDbContext,
            BuyingCatalogueDbContext bcDbContext,
            IDbRepository<Order, OrderingDbContext> orderRepository,
            IDbRepository<EntityFramework.Models.BuyingCatalogue.Supplier, BuyingCatalogueDbContext> bcRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.oDbContext = oDbContext ?? throw new ArgumentNullException(nameof(oDbContext));
            this.bcDbContext = bcDbContext ?? throw new ArgumentNullException(nameof(bcDbContext));
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.bcRepository = bcRepository ?? throw new ArgumentNullException(nameof(bcRepository));
        }

        public async Task<IList<EntityFramework.Models.BuyingCatalogue.Supplier>> GetList(
            string searchString,
            EntityFramework.Models.BuyingCatalogue.CatalogueItemType catalogueItemType,
            PublicationStatus publicationStatus = null)
        {
            return await bcDbContext.CatalogueItems
                .Where(ci => ci.Supplier.Name.Contains(searchString))
                .Where(ci => publicationStatus == null || ci.PublishedStatus == publicationStatus)
                .Where(ci => ci.CatalogueItemType == (catalogueItemType ?? EntityFramework.Models.BuyingCatalogue.CatalogueItemType.Solution))
                .Select(ci => ci.Supplier)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
