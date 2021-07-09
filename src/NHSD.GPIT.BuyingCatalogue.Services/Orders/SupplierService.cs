using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class SupplierService : ISupplierService
    {
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly IOrderService orderService;

        public SupplierService(
            BuyingCatalogueDbContext dbContext,
            IOrderService orderService)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public Task<List<Supplier>> GetListFromBuyingCatalogue(
            string searchString,
            CatalogueItemType? catalogueItemType,
            PublicationStatus? publicationStatus = null)
        {
            CatalogueItemType cIType =
                catalogueItemType ?? CatalogueItemType.Solution;

            IQueryable<CatalogueItem> query =
                dbContext.CatalogueItems.Where(ci => ci.Supplier.Name.Contains(searchString) && ci.CatalogueItemType == cIType);

            if (publicationStatus is not null)
                query.Where(ci => ci.PublishedStatus == publicationStatus);

            return query.Select(ci => ci.Supplier)
                .Distinct()
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<Supplier> GetSupplierFromBuyingCatalogue(string id)
        {
            return dbContext.Suppliers
                .Include(o => o.SupplierContacts)
                .Where(s => s.Id == id)
                .SingleAsync();
        }

        public async Task AddOrderSupplier(CallOffId callOffId, string supplierId)
        {
            supplierId.ValidateNotNullOrWhiteSpace(nameof(supplierId));

            var supplier = await GetSupplierFromBuyingCatalogue(supplierId);
            var order = await orderService.GetOrder(callOffId);

            order.Supplier = supplier;

            await dbContext.SaveChangesAsync();
        }

        public async Task AddOrUpdateOrderSupplierContact(CallOffId callOffId, Contact contact)
        {
            var order = await orderService.GetOrder(callOffId);

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

            await dbContext.SaveChangesAsync();
        }

        public async Task SetSupplierSection(Order order, Supplier supplier, Contact contact)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (supplier is null)
                throw new ArgumentNullException(nameof(supplier));

            if (contact is null)
                throw new ArgumentNullException(nameof(contact));

            order.Supplier ??= await dbContext.Suppliers.FindAsync(supplier.Id) ?? new Supplier
            {
                Id = supplier.Id,
            };

            order.Supplier.Name = supplier.Name;
            order.Supplier.Address = supplier.Address;
            order.SupplierContact = contact;

            await dbContext.SaveChangesAsync();
        }
    }
}
