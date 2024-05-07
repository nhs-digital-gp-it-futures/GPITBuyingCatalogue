using System;
using System.Collections.Generic;
using System.Linq;
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

        public Task<List<Supplier>> GetActiveSuppliers(OrderType orderType, string selectedFrameworkId)
        {
            ArgumentNullException.ThrowIfNull(orderType);

            return orderType.Value switch
            {
                OrderTypeEnum.Solution => GetActiveSuppliersForSolutions(selectedFrameworkId).ToListAsync(),
                _ => GetActiveSuppliersForSolutionsWithAssociatedServices(orderType.ToPracticeReorganisationType, selectedFrameworkId).ToListAsync(),
            };
        }

        public Task<bool> HasActiveSuppliers(OrderType orderType)
        {
            ArgumentNullException.ThrowIfNull(orderType);

            return orderType.Value switch
            {
                OrderTypeEnum.Solution => GetActiveSuppliersForSolutions().AnyAsync(),
                _ => GetActiveSuppliersForSolutionsWithAssociatedServices(orderType.ToPracticeReorganisationType).AnyAsync(),
            };
        }

        public Task<Supplier> GetSupplierFromBuyingCatalogue(int id)
        {
            return dbContext.Suppliers
                .Include(o => o.SupplierContacts)
                .Where(s => s.Id == id)
                .FirstAsync();
        }

        public async Task AddOrderSupplier(CallOffId callOffId, string internalOrgId, int supplierId)
        {
            var supplier = await GetSupplierFromBuyingCatalogue(supplierId);
            var order = (await orderService.GetOrderWithSupplier(callOffId, internalOrgId)).Order;

            order.Supplier = supplier;

            await dbContext.SaveChangesAsync();
        }

        public async Task AddOrUpdateOrderSupplierContact(
            CallOffId callOffId,
            string internalOrgId,
            SupplierContact contact)
        {
            ArgumentNullException.ThrowIfNull(contact);

            var order = (await orderService.GetOrderWithSupplier(callOffId, internalOrgId)).Order;

            order.SupplierContact ??= new Contact();

            order.SupplierContact.FirstName = contact.FirstName;
            order.SupplierContact.LastName = contact.LastName;
            order.SupplierContact.Department = contact.Department;
            order.SupplierContact.Email = contact.Email;
            order.SupplierContact.Phone = contact.PhoneNumber;

            await dbContext.SaveChangesAsync();
        }

        public async Task SetSupplierSection(Order order, Supplier supplier, Contact contact)
        {
            ArgumentNullException.ThrowIfNull(order);
            ArgumentNullException.ThrowIfNull(supplier);
            ArgumentNullException.ThrowIfNull(contact);

            order.Supplier ??= await dbContext.Suppliers.FindAsync(supplier.Id) ?? new Supplier { Id = supplier.Id, };

            order.Supplier.Name = supplier.Name;
            order.Supplier.Address = supplier.Address;
            order.SupplierContact = contact;

            await dbContext.SaveChangesAsync();
        }

        private IQueryable<Supplier> GetActiveSuppliersForSolutions(string selectedFrameworkId = null)
        {
            var query = dbContext.Suppliers
                .Include(x => x.CatalogueItems)
                .Where(
                    x => x.IsActive
                        && x.CatalogueItems.Any(
                            ci => ci.CatalogueItemType == CatalogueItemType.Solution
                                && ci.PublishedStatus == PublicationStatus.Published
                                && ci.Solution.FrameworkSolutions.Select(f => f.Framework).Distinct().Any(f => !f.IsExpired)));

            if (!string.IsNullOrEmpty(selectedFrameworkId))
            {
                query = query
                .Where(
                    x => x.CatalogueItems.Any(
                        ci => ci.Solution.FrameworkSolutions.Any(f => f.FrameworkId == selectedFrameworkId)));
            }

            return query
                .OrderBy(x => x.Name);
        }

        private IQueryable<Supplier> GetActiveSuppliersForSolutionsWithAssociatedServices(PracticeReorganisationTypeEnum practiceReorganisationType, string selectedFrameworkId = null)
        {
            var query = dbContext
                .CatalogueItems
                .AsNoTracking()
                .Include(ci => ci.SupplierServiceAssociations)
                .Include(ci => ci.Supplier)
                .Where(
                    ci =>
                        ci.CatalogueItemType == CatalogueItemType.Solution
                        && ci.PublishedStatus == PublicationStatus.Published
                        && ci.Solution.FrameworkSolutions.Select(f => f.Framework).Distinct().Any(f => !f.IsExpired));

            if (practiceReorganisationType != PracticeReorganisationTypeEnum.None)
            {
                query = query.Where(ci => ci.SupplierServiceAssociations.Any(ssa => (ssa.AssociatedService.PracticeReorganisationType & practiceReorganisationType) == practiceReorganisationType));
            }
            else
            {
                query = query.Where(ci => ci.SupplierServiceAssociations.Any());
            }

            if (!string.IsNullOrEmpty(selectedFrameworkId))
            {
                query = query
                .Where(
                    ci => ci.Solution.FrameworkSolutions.Any(f => f.FrameworkId == selectedFrameworkId));
            }

            return query
                .Select(ci => ci.Supplier)
                .Where(s => s.IsActive)
                .Distinct();
        }
    }
}
