using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class SupplierServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SupplierService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static Task AddOrUpdateOrderSupplierContact_NullContact_ThrowsException(
            SupplierService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(
                () => service.AddOrUpdateOrderSupplierContact(default, null, null));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetAllSuppliersFromBuyingCatalogue_NoMatchingSuppliers_ReturnsEmptySet(
            [Frozen] BuyingCatalogueDbContext context,
            List<Supplier> suppliers,
            SupplierService service)
        {
            suppliers.ForEach(x => x.IsActive = true);
            context.Suppliers.AddRange(suppliers);
            await context.SaveChangesAsync();

            var result = await service.GetAllSuppliersFromBuyingCatalogue();

            result.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetAllSuppliersFromBuyingCatalogue_MatchingSuppliers_ReturnsExpected(
            [Frozen] BuyingCatalogueDbContext context,
            List<CatalogueItem> catalogueItems,
            List<Supplier> suppliers,
            SupplierService service)
        {
            catalogueItems.ForEach(x => x.CatalogueItemType = CatalogueItemType.Solution);
            context.CatalogueItems.AddRange(catalogueItems);

            for (var i = 0; i < suppliers.Count; i++)
            {
                suppliers[i].IsActive = true;
                suppliers[i].CatalogueItems.Add(catalogueItems[i]);
            }

            context.Suppliers.AddRange(suppliers);

            await context.SaveChangesAsync();

            var result = await service.GetAllSuppliersFromBuyingCatalogue();

            result.Should().BeEquivalentTo(suppliers);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetAllSuppliersWithAssociatedServices_NoMatchingSuppliers_ReturnsEmptySet(
            [Frozen] BuyingCatalogueDbContext context,
            List<Supplier> suppliers,
            SupplierService service)
        {
            suppliers.ForEach(x => x.IsActive = true);

            context.Suppliers.AddRange(suppliers);

            await context.SaveChangesAsync();

            var result = await service.GetAllSuppliersWithAssociatedServices();

            result.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetAllSuppliersWithAssociatedServices_MatchingSuppliers_ReturnsExpected(
            [Frozen] BuyingCatalogueDbContext context,
            List<CatalogueItem> catalogueItems,
            List<Supplier> suppliers,
            SupplierService service)
        {
            catalogueItems.ForEach(x => x.CatalogueItemType = CatalogueItemType.AssociatedService);
            catalogueItems.ForEach(x => x.PublishedStatus = PublicationStatus.Published);
            catalogueItems[1].PublishedStatus = PublicationStatus.Draft;

            context.CatalogueItems.AddRange(catalogueItems);

            suppliers[0].IsActive = true;
            suppliers[0].CatalogueItems.Add(catalogueItems[0]);

            suppliers[1].IsActive = true;
            suppliers[1].CatalogueItems.Add(catalogueItems[1]);

            suppliers[2].IsActive = false;
            suppliers[2].CatalogueItems.Add(catalogueItems[2]);

            context.Suppliers.AddRange(suppliers);

            await context.SaveChangesAsync();

            var results = await service.GetAllSuppliersWithAssociatedServices();

            results.Should().HaveCount(1);
            results.First().Id.Should().Be(suppliers[0].Id);
        }
    }
}
