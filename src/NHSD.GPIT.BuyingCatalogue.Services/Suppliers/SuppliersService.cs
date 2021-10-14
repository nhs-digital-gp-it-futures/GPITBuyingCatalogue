using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;

namespace NHSD.GPIT.BuyingCatalogue.Services.Suppliers
{
    public sealed class SuppliersService : ISuppliersService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public SuppliersService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IReadOnlyList<Supplier>> GetAllSuppliers()
        {
            return await dbContext.Suppliers.OrderBy(s => s.Name).ToListAsync();
        }

        public async Task<IList<Supplier>> GetAllActiveSuppliers()
        {
            return await dbContext.Suppliers.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync();
        }

        public async Task<Supplier> GetSupplier(int supplierId)
        {
            return await dbContext.Suppliers
                .Include(s => s.SupplierContacts)
                .Where(s => s.Id == supplierId).SingleOrDefaultAsync();
        }

        public async Task<Supplier> GetSupplierByName(string supplierName)
        {
            return await dbContext.Suppliers
                .Where(s => s.Name == supplierName).SingleOrDefaultAsync();
        }

        public async Task<Supplier> GetSupplierByLegalName(string supplierLegalName)
        {
            return await dbContext.Suppliers
                .Where(s => s.LegalName == supplierLegalName).SingleOrDefaultAsync();
        }

        public async Task<List<CatalogueItem>> GetAllSolutionsForSupplier(int supplierId)
        {
            return await dbContext.CatalogueItems
                .Where(ci => ci.SupplierId == supplierId)
                .ToListAsync();
        }

        public async Task<Supplier> AddSupplier(EditSupplierModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var latestSupplier = await dbContext.Suppliers.OrderByDescending(s => s.Id).Take(1).SingleAsync();

            var supplier = new Supplier
            {
                Id = latestSupplier.Id + 1,
                Name = model.SupplierName,
                LegalName = model.SupplierLegalName,
                Summary = model.AboutSupplier,
                SupplierUrl = model.SupplierWebsite,
                IsActive = false,
            };

            dbContext.Suppliers.Add(supplier);

            await dbContext.SaveChangesAsync();

            return supplier;
        }

        public async Task<Supplier> UpdateSupplierActiveStatus(int supplierId, bool newStatus)
        {
            var supplier = await GetSupplier(supplierId);

            supplier.IsActive = newStatus;

            await dbContext.SaveChangesAsync();

            return supplier;
        }

        public async Task<Supplier> EditSupplierDetails(int supplierId, EditSupplierModel updatedSupplier)
        {
            if (updatedSupplier is null)
                throw new ArgumentNullException(nameof(updatedSupplier));

            var supplier = await GetSupplier(supplierId);

            supplier.Name = updatedSupplier.SupplierName;
            supplier.LegalName = updatedSupplier.SupplierLegalName;
            supplier.Summary = updatedSupplier.AboutSupplier;
            supplier.SupplierUrl = updatedSupplier.SupplierWebsite;

            await dbContext.SaveChangesAsync();

            return supplier;
        }

        public async Task<Supplier> EditSupplierAddress(int supplierId, Address newAddress)
        {
            var supplier = await GetSupplier(supplierId);

            supplier.Address = newAddress;

            await dbContext.SaveChangesAsync();

            return supplier;
        }

        public async Task<Supplier> AddSupplierContact(int supplierId, SupplierContact newContact)
        {
            var supplier = await GetSupplier(supplierId);

            supplier.SupplierContacts.Add(newContact);

            await dbContext.SaveChangesAsync();

            return supplier;
        }

        public async Task<Supplier> EditSupplierContact(int supplierId, int contactId, SupplierContact updatedContact)
        {
            if (updatedContact is null)
                throw new ArgumentNullException(nameof(updatedContact));

            var supplier = await GetSupplier(supplierId);

            var contact = supplier.SupplierContacts.Single(sc => sc.Id == contactId);

            contact.FirstName = updatedContact.FirstName;
            contact.LastName = updatedContact.LastName;
            contact.Email = updatedContact.Email;
            contact.Department = updatedContact.Department;
            contact.PhoneNumber = updatedContact.PhoneNumber;

            await dbContext.SaveChangesAsync();

            return supplier;
        }

        public async Task<Supplier> DeleteSupplierContact(int supplierId, int contactId)
        {
            var supplier = await GetSupplier(supplierId);

            var contact = supplier.SupplierContacts.Single(sc => sc.Id == contactId);

            supplier.SupplierContacts.Remove(contact);

            await dbContext.SaveChangesAsync();

            return supplier;
        }
    }
}
