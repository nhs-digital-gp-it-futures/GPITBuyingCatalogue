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
        public static async Task GetAllSuppliersFromBuyingCatalogue_SuppliersNoPublishedSolutions_ReturnsEmptySet(
            [Frozen] BuyingCatalogueDbContext context,
            List<CatalogueItem> catalogueItems,
            List<Supplier> suppliers,
            SupplierService service)
        {
            catalogueItems.ForEach(x => x.CatalogueItemType = CatalogueItemType.Solution);
            catalogueItems.ForEach(x => x.PublishedStatus = PublicationStatus.Draft);
            context.CatalogueItems.AddRange(catalogueItems);

            for (var i = 0; i < suppliers.Count; i++)
            {
                suppliers[i].IsActive = true;
                suppliers[i].CatalogueItems.Add(catalogueItems[i]);
            }

            context.Suppliers.AddRange(suppliers);

            await context.SaveChangesAsync();

            var result = await service.GetAllSuppliersFromBuyingCatalogue();

            result.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetAllSuppliersFromBuyingCatalogue_MatchingSuppliers_ReturnsExpected(
            [Frozen] BuyingCatalogueDbContext context,
            List<Solution> solutions,
            List<Supplier> suppliers,
            SupplierService service)
        {
            solutions.ForEach(x => x.FrameworkSolutions.ToList().ForEach(y => y.Framework.IsExpired = false));
            solutions.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution);
            solutions.ForEach(x => x.CatalogueItem.PublishedStatus = PublicationStatus.Published);
            context.Solutions.AddRange(solutions);

            for (var i = 0; i < suppliers.Count; i++)
            {
                suppliers[i].IsActive = true;
                suppliers[i].CatalogueItems.Add(solutions[i].CatalogueItem);
            }

            context.Suppliers.AddRange(suppliers);

            await context.SaveChangesAsync();

            var result = await service.GetAllSuppliersFromBuyingCatalogue();

            result.Should().BeEquivalentTo(suppliers);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetAllSuppliersFromBuyingCatalogue_ExpiredFramework_ReturnsExpected(
            [Frozen] BuyingCatalogueDbContext context,
            List<Solution> solutions,
            List<Supplier> suppliers,
            SupplierService service)
        {
            solutions.Skip(1).ToList().ForEach(x => x.FrameworkSolutions.ToList().ForEach(y => y.Framework.IsExpired = false));
            solutions.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution);
            solutions.ForEach(x => x.CatalogueItem.PublishedStatus = PublicationStatus.Published);
            context.Solutions.AddRange(solutions);

            for (var i = 0; i < suppliers.Count; i++)
            {
                suppliers[i].IsActive = true;
                suppliers[i].CatalogueItems.Add(solutions[i].CatalogueItem);
            }

            context.Suppliers.AddRange(suppliers);

            await context.SaveChangesAsync();

            var result = await service.GetAllSuppliersFromBuyingCatalogue();

            result.Should().BeEquivalentTo(suppliers.Skip(1));
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
        public static async Task GetAllSuppliersWithAssociatedServices_NoServiceAssociations_ReturnsExpected(
            [Frozen] BuyingCatalogueDbContext context,
            List<Solution> solutions,
            List<Supplier> suppliers,
            SupplierService service)
        {
            solutions.ForEach(x => x.FrameworkSolutions.ToList().ForEach(y => y.Framework.IsExpired = false));
            solutions.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            solutions.ForEach(x => x.CatalogueItem.PublishedStatus = PublicationStatus.Published);
            solutions[1].CatalogueItem.PublishedStatus = PublicationStatus.Draft;

            context.Solutions.AddRange(solutions);

            suppliers[0].IsActive = true;
            suppliers[0].CatalogueItems.Add(solutions[0].CatalogueItem);

            suppliers[1].IsActive = true;
            suppliers[1].CatalogueItems.Add(solutions[1].CatalogueItem);

            suppliers[2].IsActive = false;
            suppliers[2].CatalogueItems.Add(solutions[2].CatalogueItem);

            context.Suppliers.AddRange(suppliers);

            await context.SaveChangesAsync();

            var results = await service.GetAllSuppliersWithAssociatedServices();

            results.Should().HaveCount(0);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetAllSuppliersWithAssociatedServices_WithServiceAssociations_ReturnsExpected(
            [Frozen] BuyingCatalogueDbContext dbContext,
            List<Solution> solutions,
            List<AssociatedService> associatedServices,
            List<Supplier> suppliers,
            SupplierService service)
        {
            solutions.ForEach(x => x.FrameworkSolutions.ToList().ForEach(y => y.Framework.IsExpired = false));
            solutions.ForEach(s => s.CatalogueItem.SupplierServiceAssociations = new List<SupplierServiceAssociation>());

            var solution = solutions.First();
            var associatedService = associatedServices.First();
            var supplier = suppliers.First();

            solution.CatalogueItem.SupplierServiceAssociations.Add(new(solution.CatalogueItemId, associatedService.CatalogueItemId));
            solution.CatalogueItem.Supplier = supplier;
            associatedService.CatalogueItem.Supplier = supplier;

            supplier.IsActive = true;
            supplier.CatalogueItems.Add(solution.CatalogueItem);

            dbContext.Solutions.AddRange(solutions);
            dbContext.AssociatedServices.AddRange(associatedService);
            dbContext.Suppliers.AddRange(suppliers);

            await dbContext.SaveChangesAsync();

            var results = await service.GetAllSuppliersWithAssociatedServices();

            results.Should().HaveCount(1);
            results[0].Should().BeEquivalentTo(supplier, opt => opt.Excluding(s => s.SupplierContacts).Excluding(s => s.CatalogueItems));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetAllSuppliersWithAssociatedServices_ExpiredFramework_ReturnsExpected(
            [Frozen] BuyingCatalogueDbContext dbContext,
            List<Solution> solutions,
            List<AssociatedService> associatedServices,
            List<Supplier> suppliers,
            SupplierService service)
        {
            solutions.Skip(1).ToList().ForEach(x => x.FrameworkSolutions.ToList().ForEach(y => y.Framework.IsExpired = false));
            solutions.ForEach(s => s.CatalogueItem.SupplierServiceAssociations = new List<SupplierServiceAssociation>());

            var solution = solutions.First();
            var associatedService = associatedServices.First();
            var supplier = suppliers.First();

            solution.CatalogueItem.SupplierServiceAssociations.Add(new(solution.CatalogueItemId, associatedService.CatalogueItemId));
            solution.CatalogueItem.Supplier = supplier;
            associatedService.CatalogueItem.Supplier = supplier;

            supplier.IsActive = true;
            supplier.CatalogueItems.Add(solution.CatalogueItem);

            dbContext.Solutions.AddRange(solutions);
            dbContext.AssociatedServices.AddRange(associatedService);
            dbContext.Suppliers.AddRange(suppliers);

            await dbContext.SaveChangesAsync();

            var results = await service.GetAllSuppliersWithAssociatedServices();

            results.Should().BeEmpty();
        }
    }
}
