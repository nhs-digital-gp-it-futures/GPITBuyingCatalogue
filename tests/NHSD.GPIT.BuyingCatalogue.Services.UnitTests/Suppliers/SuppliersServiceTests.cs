using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Suppliers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Suppliers
{
    public static class SuppliersServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SuppliersService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task AddSupplier_ValidatesSupplier_NotNull(
            SuppliersService service)
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddSupplier(null));

            actual.ParamName.Should().Be("model");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetAllSuppliers_RetrievesAllSuppliers(
            [Frozen] BuyingCatalogueDbContext context,
            Supplier supplier1,
            Supplier supplier2,
            Supplier supplier3,
            SuppliersService service)
        {
            context.Suppliers.Add(supplier1);
            context.Suppliers.Add(supplier2);
            context.Suppliers.Add(supplier3);
            await context.SaveChangesAsync();

            var actual = await service.GetAllSuppliers();

            actual.Count.Should().Be(3);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetAllSupplier_WithSearchTerm_ReturnsSupplier(
            List<Supplier> suppliers,
            [Frozen] BuyingCatalogueDbContext context,
            SuppliersService service)
        {
            context.Suppliers.AddRange(suppliers);
            await context.SaveChangesAsync();

            var expectedSupplier = suppliers.First();
            var searchTerm = expectedSupplier.Name;

            var result = await service.GetAllSuppliers(searchTerm);

            result.Should().NotBeEmpty();
            result.Should().Contain(expectedSupplier);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetAllActiveSuppliers_RetrievesAllActiveSuppliers(
            [Frozen] BuyingCatalogueDbContext context,
            Supplier supplier1,
            Supplier supplier2,
            Supplier supplier3,
            SuppliersService service)
        {
            supplier1.IsActive = false;
            context.Suppliers.Add(supplier1);
            supplier2.IsActive = true;
            context.Suppliers.Add(supplier2);
            supplier3.IsActive = true;
            context.Suppliers.Add(supplier3);
            await context.SaveChangesAsync();

            var actual = await service.GetAllActiveSuppliers();

            actual.Count.Should().Be(2);
            actual.Any(s => s.Id == supplier1.Id).Should().BeFalse();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSupplierByName_RetrievesCorrectSupplier(
            [Frozen] BuyingCatalogueDbContext context,
            Supplier supplier1,
            Supplier supplier2,
            Supplier supplier3,
            SuppliersService service)
        {
            context.Suppliers.Add(supplier1);
            context.Suppliers.Add(supplier2);
            context.Suppliers.Add(supplier3);
            await context.SaveChangesAsync();

            var actual = await service.GetSupplierByName(supplier2.Name);

            actual.Should().NotBeNull();
            actual.Name.Should().Be(supplier2.Name);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSupplierByLegalName_RetrievesCorrectSupplier(
            [Frozen] BuyingCatalogueDbContext context,
            Supplier supplier1,
            Supplier supplier2,
            Supplier supplier3,
            SuppliersService service)
        {
            context.Suppliers.Add(supplier1);
            context.Suppliers.Add(supplier2);
            context.Suppliers.Add(supplier3);
            await context.SaveChangesAsync();

            var actual = await service.GetSupplierByLegalName(supplier2.LegalName);

            actual.Should().NotBeNull();
            actual.Name.Should().Be(supplier2.Name);
            actual.LegalName.Should().Be(supplier2.LegalName);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetAllSolutionsForSupplier_RetrievesCorrectSolutions(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem item1,
            CatalogueItem item2,
            CatalogueItem item3,
            SuppliersService service)
        {
            context.CatalogueItems.Add(item1);
            context.CatalogueItems.Add(item2);
            context.CatalogueItems.Add(item3);
            await context.SaveChangesAsync();

            var actual = await service.GetAllSolutionsForSupplier(item2.SupplierId);

            actual.Count.Should().Be(1);
            actual.First().Id.Should().Be(item2.Id);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddSupplier_AddsSupplier(
            [Frozen] BuyingCatalogueDbContext context,
            Supplier latestSupplier,
            EditSupplierModel newSupplier,
            SuppliersService service)
        {
            context.Suppliers.Add(latestSupplier);
            await context.SaveChangesAsync();

            await service.AddSupplier(newSupplier);

            var actual = await context.Suppliers.AsAsyncEnumerable().OrderByDescending(s => s.Id).Take(1).FirstAsync();

            actual.Id.Should().Be(latestSupplier.Id + 1);
            actual.Name.Should().Be(newSupplier.SupplierName);
            actual.LegalName.Should().Be(newSupplier.SupplierLegalName);
            actual.Summary.Should().Be(newSupplier.AboutSupplier);
            actual.SupplierUrl.Should().Be(newSupplier.SupplierWebsite);
            actual.IsActive.Should().BeFalse();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task UpdateSupplierActiveStatus_UpdatesStatus(
            [Frozen] BuyingCatalogueDbContext context,
            Supplier supplier,
            bool newStatus,
            SuppliersService service)
        {
            supplier.IsActive = !newStatus;
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            await service.UpdateSupplierActiveStatus(supplier.Id, newStatus);

            var actual = await context.Suppliers.AsAsyncEnumerable().FirstAsync(s => s.Id == supplier.Id);

            actual.IsActive.Should().Be(newStatus);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task EditSupplier_UpdatesSupplier(
           [Frozen] BuyingCatalogueDbContext context,
           Supplier supplier,
           EditSupplierModel model,
           SuppliersService service)
        {
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            await service.EditSupplierDetails(supplier.Id, model);

            var actual = await context.Suppliers.AsAsyncEnumerable().FirstAsync(s => s.Id == supplier.Id);

            actual.Name.Should().Be(model.SupplierName);
            actual.LegalName.Should().Be(model.SupplierLegalName);
            actual.Summary.Should().Be(model.AboutSupplier);
            actual.SupplierUrl.Should().Be(model.SupplierWebsite);
            actual.IsActive.Should().Be(supplier.IsActive);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task EditSupplierAddress_SavesAddress(
            [Frozen] BuyingCatalogueDbContext context,
            Supplier supplier,
            Address newAddress,
            SuppliersService service)
        {
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            await service.EditSupplierAddress(supplier.Id, newAddress);

            var actual = await context.Suppliers.AsAsyncEnumerable().FirstAsync(s => s.Id == supplier.Id);

            actual.Address.Should().BeEquivalentTo(newAddress);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddSupplierContact_AddsContact(
            [Frozen] BuyingCatalogueDbContext context,
            Supplier supplier,
            AspNetUser user,
            SupplierContact newContact,
            SuppliersService service)
        {
            supplier.SupplierContacts.Clear();
            context.Suppliers.Add(supplier);
            context.AspNetUsers.Add(user);
            await context.SaveChangesAsync();

            newContact.AssignedCatalogueItems.Clear();
            newContact.Id = default;
            newContact.SupplierId = default;
            newContact.LastUpdatedBy = user.Id;
            newContact.LastUpdatedByUser = null;
            await service.AddSupplierContact(supplier.Id, newContact);

            var actual = await context.Suppliers.AsAsyncEnumerable().FirstAsync(s => s.Id == supplier.Id);

            actual.SupplierContacts.First().Should().BeEquivalentTo(newContact);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task EditSupplierContact_UpdatesContact(
            [Frozen] BuyingCatalogueDbContext context,
            Supplier supplier,
            SupplierContact updatedContact,
            SuppliersService service)
        {
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            await service.EditSupplierContact(supplier.Id, supplier.SupplierContacts.First().Id, updatedContact);

            var actual = await context.Suppliers.AsAsyncEnumerable().FirstAsync(s => s.Id == supplier.Id);

            var savedContact = actual.SupplierContacts.First();

            savedContact.FirstName.Should().Be(updatedContact.FirstName);
            savedContact.LastName.Should().Be(updatedContact.LastName);
            savedContact.Email.Should().Be(updatedContact.Email);
            savedContact.Department.Should().Be(updatedContact.Department);
            savedContact.PhoneNumber.Should().Be(updatedContact.PhoneNumber);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task DeleteSupplierContact_DeletesContact(
            [Frozen] BuyingCatalogueDbContext context,
            Supplier supplier,
            SuppliersService service)
        {
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            var contactToDelete = supplier.SupplierContacts.First().Id;

            await service.DeleteSupplierContact(supplier.Id, contactToDelete);

            var updatedSupplier = await context.Suppliers.FirstAsync(s => s.Id == supplier.Id);

            updatedSupplier.SupplierContacts.ToList().ForEach(sc => sc.Id.Should().NotBe(contactToDelete));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSuppliersBySearchTerm_ReturnsSupplier(
            List<Supplier> suppliers,
            [Frozen] BuyingCatalogueDbContext context,
            SuppliersService service)
        {
            context.Suppliers.AddRange(suppliers);
            await context.SaveChangesAsync();

            var expectedSupplier = suppliers.First();
            var searchTerm = expectedSupplier.Id.ToString();

            var result = await service.GetSuppliersBySearchTerm(searchTerm);

            result.Should().NotBeEmpty();
            result.Should().Contain(expectedSupplier);
        }
    }
}
