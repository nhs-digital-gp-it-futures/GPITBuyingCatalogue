using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contacts;
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
        private readonly IContactDetailsService contactDetailsService;
        private readonly IOrderService orderService;

        public SupplierService(
            ILogWrapper<SupplierService> logger,
            OrderingDbContext oDbContext,
            BuyingCatalogueDbContext bcDbContext,
            IDbRepository<Order, OrderingDbContext> orderRepository,
            IDbRepository<EntityFramework.Models.BuyingCatalogue.Supplier, BuyingCatalogueDbContext> bcRepository,
            IContactDetailsService contactDetailsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.oDbContext = oDbContext ?? throw new ArgumentNullException(nameof(oDbContext));
            this.bcDbContext = bcDbContext ?? throw new ArgumentNullException(nameof(bcDbContext));
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.bcRepository = bcRepository ?? throw new ArgumentNullException(nameof(bcRepository));
            this.contactDetailsService = contactDetailsService ?? throw new ArgumentNullException(nameof(contactDetailsService));
        }

        public async Task<List<EntityFramework.Models.BuyingCatalogue.Supplier>> GetListFromBuyingCatalogue(
            string searchString,
            EntityFramework.Models.BuyingCatalogue.CatalogueItemType catalogueItemType,
            PublicationStatus publicationStatus = null)
        {
            EntityFramework.Models.BuyingCatalogue.CatalogueItemType cIType =
                catalogueItemType ?? EntityFramework.Models.BuyingCatalogue.CatalogueItemType.Solution;

            IQueryable<EntityFramework.Models.BuyingCatalogue.CatalogueItem> query =
                bcDbContext.CatalogueItems.Where(ci => ci.Supplier.Name.Contains(searchString) && ci.CatalogueItemType == cIType);

            if (publicationStatus is not null)
                query.Where(ci => ci.PublishedStatus == publicationStatus);

            return await query.Select(ci => ci.Supplier)
                .Distinct()
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<EntityFramework.Models.BuyingCatalogue.Supplier> GetSupplierFromBuyingCatalogue(string id)
        {
            return await bcDbContext.Suppliers.Where(s => s.Id == id).SingleAsync();
        }

        public async Task AddOrderSupplier(string callOffId, string supplierId)
        {
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));
            supplierId.ValidateNotNullOrWhiteSpace(nameof(supplierId));

            var supplier = await GetSupplierFromBuyingCatalogue(supplierId);

            var order = await oDbContext.Orders.Where(o => o.CallOffId == new CallOffId(callOffId)).SingleAsync();

            var supplierModel = new EntityFramework.Models.Ordering.Supplier
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Address = contactDetailsService.AddOrUpdateAddress(order.Supplier?.Address, supplier.Address),
            };

            order.Supplier = supplierModel;

            await oDbContext.SaveChangesAsync();
        }

        public async Task AddOrUpdateOrderSupplierContact(string callOffId, Contact contact)
        {
            callOffId.ValidateNotNullOrWhiteSpace(nameof(callOffId));

            var order = await oDbContext.Orders.Where(o => o.CallOffId == new CallOffId(callOffId)).SingleAsync();

            switch (order.SupplierContact)
            {
                case null:
                    order.SupplierContact = contact;
                    break;
                default:
                    order.SupplierContact.FirstName = contact.FirstName;
                    order.SupplierContact.LastName = contact.LastName;
                    order.SupplierContact.Email = contact.Email;
                    order.SupplierContact.Phone = contact.Phone;
                    break;
            }

            await oDbContext.SaveChangesAsync();
        }

        public async Task SetSupplierSection(Order order, EntityFramework.Models.Ordering.Supplier supplier, Contact contact)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (supplier is null)
                throw new ArgumentNullException(nameof(supplier));

            if (contact is null)
                throw new ArgumentNullException(nameof(contact));

            order.Supplier ??= await oDbContext.Suppliers.FindAsync(supplier.Id) ?? new EntityFramework.Models.Ordering.Supplier
            {
                Id = supplier.Id,
            };

            order.Supplier.Name = supplier.Name;
            order.Supplier.Address = supplier.Address;
            order.SupplierContact = contact;

            await oDbContext.SaveChangesAsync();
        }
    }
}
