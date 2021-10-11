using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Suppliers;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Suppliers
{
    public static class SuppliersServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SuppliersService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task AddSupplier_ValidatesSupplier_NotNull(
            SuppliersService service)
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddSupplier(null));

            actual.ParamName.Should().Be(nameof(EditSupplierModel));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddSupplier_AddsSupplier(
            [Frozen] BuyingCatalogueDbContext context,
            Supplier latestSupplier,
            EditSupplierModel newSupplier,
            SuppliersService service)
        {
            context.Suppliers.Add(latestSupplier);
            await context.SaveChangesAsync();

            await service.AddSupplier(newSupplier);

            var actual = await context.Suppliers.AsAsyncEnumerable().OrderByDescending(s => s.Id).Take(1).SingleAsync();

            actual.Id.Should().Be(latestSupplier.Id + 1);
            actual.Name.Should().Be(newSupplier.SupplierName);
            actual.LegalName.Should().Be(newSupplier.SupplierLegalName);
            actual.Summary.Should().Be(newSupplier.AboutSupplier);
            actual.SupplierUrl.Should().Be(newSupplier.SupplierWebsite);
            actual.IsActive.Should().BeFalse();
        }

        [Theory]
        [InMemoryDbAutoData]
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

            var actual = await context.Suppliers.AsAsyncEnumerable().SingleAsync(s => s.Id == supplier.Id);

            actual.IsActive.Should().Be(newStatus);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task EditSupplier_UpdatesSupplier(
           [Frozen] BuyingCatalogueDbContext context,
           Supplier supplier,
           EditSupplierModel model,
           SuppliersService service)
        {
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            await service.EditSupplierDetails(supplier.Id, model);

            var actual = await context.Suppliers.AsAsyncEnumerable().SingleAsync(s => s.Id == supplier.Id);

            actual.Name.Should().Be(model.SupplierName);
            actual.LegalName.Should().Be(model.SupplierLegalName);
            actual.Summary.Should().Be(model.AboutSupplier);
            actual.SupplierUrl.Should().Be(model.SupplierWebsite);
            actual.IsActive.Should().Be(supplier.IsActive);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task EditSupplierAddress_SavesAddress(
            [Frozen] BuyingCatalogueDbContext context,
            Supplier supplier,
            Address newAddress,
            SuppliersService service)
        {
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            await service.EditSupplierAddress(supplier.Id, newAddress);

            var actual = await context.Suppliers.AsAsyncEnumerable().SingleAsync(s => s.Id == supplier.Id);

            actual.Address.Should().BeEquivalentTo(newAddress);
        }

        [Theory]
        [InMemoryDbAutoData]
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

            newContact.Id = default;
            newContact.SupplierId = default;
            newContact.LastUpdatedBy = user.Id;
            newContact.LastUpdatedByUser = user;
            await service.AddSupplierContact(supplier.Id, newContact);

            var actual = await context.Suppliers.AsAsyncEnumerable().SingleAsync(s => s.Id == supplier.Id);

            actual.SupplierContacts.Single().Should().BeEquivalentTo(newContact);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task EditSupplierContact_UpdatesContact(
            [Frozen] BuyingCatalogueDbContext context,
            Supplier supplier,
            SupplierContact updatedContact,
            SuppliersService service)
        {
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            await service.EditSupplierContact(supplier.Id, supplier.SupplierContacts.First().Id, updatedContact);

            var actual = await context.Suppliers.AsAsyncEnumerable().SingleAsync(s => s.Id == supplier.Id);

            var savedContact = actual.SupplierContacts.First();

            savedContact.FirstName.Should().Be(updatedContact.FirstName);
            savedContact.LastName.Should().Be(updatedContact.LastName);
            savedContact.Email.Should().Be(updatedContact.Email);
            savedContact.Department.Should().Be(updatedContact.Department);
            savedContact.PhoneNumber.Should().Be(updatedContact.PhoneNumber);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeleteSupplierContact_DeletesContact(
            [Frozen] BuyingCatalogueDbContext context,
            Supplier supplier,
            SuppliersService service)
        {
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            var contactToDelete = supplier.SupplierContacts.First().Id;

            await service.DeleteSupplierContact(supplier.Id, contactToDelete);

            var updatedSupplier = await context.Suppliers.SingleAsync(s => s.Id == supplier.Id);

            updatedSupplier.SupplierContacts.ToList().ForEach(sc => sc.Id.Should().NotBe(contactToDelete));
        }
    }
}
