﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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

            // EF Core cannot translate Contains(string, StringComparison). However, as this is executed by the DB the
            // DB collation rules will apply so a case-insensitive comparison will occur.
#pragma warning disable CA1307
            Expression<Func<CatalogueItem, bool>> searchPredicate = ci => ci.Supplier.Name.Contains(searchString) && ci.CatalogueItemType == cIType;
#pragma warning restore CA1307

            IQueryable<CatalogueItem> query = dbContext.CatalogueItems.Where(searchPredicate);

            if (publicationStatus is not null)
                query = query.Where(ci => ci.PublishedStatus == publicationStatus);

            return query.Select(ci => ci.Supplier)
                .Distinct()
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<Supplier>> GetAllSuppliersFromBuyingCatalogue()
        {
            return dbContext.Suppliers
                .Include(x => x.CatalogueItems)
                .Where(x => x.IsActive
                    && x.CatalogueItems.Any(ci => ci.CatalogueItemType == CatalogueItemType.Solution))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public Task<Supplier> GetSupplierFromBuyingCatalogue(int id)
        {
            return dbContext.Suppliers
                .Include(o => o.SupplierContacts)
                .Where(s => s.Id == id)
                .SingleAsync();
        }

        public async Task AddOrderSupplier(CallOffId callOffId, string internalOrgId, int supplierId)
        {
            var supplier = await GetSupplierFromBuyingCatalogue(supplierId);
            var order = await orderService.GetOrderWithSupplier(callOffId, internalOrgId);

            order.Supplier = supplier;

            await dbContext.SaveChangesAsync();
        }

        public async Task AddOrUpdateOrderSupplierContact(CallOffId callOffId, string internalOrgId, SupplierContact contact)
        {
            if (contact is null)
                throw new ArgumentNullException(nameof(contact));

            var order = await orderService.GetOrderWithSupplier(callOffId, internalOrgId);

            order.SupplierContact ??= new Contact();

            order.SupplierContact.SupplierContactId = contact.Id == SupplierContact.TemporaryContactId ? null : contact.Id;
            order.SupplierContact.FirstName = contact.FirstName;
            order.SupplierContact.LastName = contact.LastName;
            order.SupplierContact.Department = contact.Department;
            order.SupplierContact.Email = contact.Email;
            order.SupplierContact.Phone = contact.PhoneNumber;

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
