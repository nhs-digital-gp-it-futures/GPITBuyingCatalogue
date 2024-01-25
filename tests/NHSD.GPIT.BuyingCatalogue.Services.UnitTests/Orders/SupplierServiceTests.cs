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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
        [InMemoryDbInlineAutoData(OrderTypeEnum.Solution)]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static async Task SuppliersAvailableByOrderType_NoMatchingSuppliers_ReturnsFalse(
            OrderType orderType,
            [Frozen] BuyingCatalogueDbContext context,
            List<Supplier> suppliers,
            SupplierService service)
        {
            suppliers.ForEach(x => x.IsActive = true);

            context.Suppliers.AddRange(suppliers);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await service.HasActiveSuppliers(orderType);

            result.Should().BeFalse();
        }

        [Theory]
        [InMemoryDbInlineAutoData(OrderTypeEnum.Solution)]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static async Task GetAllSuppliersByOrderType_NoMatchingSuppliers_ReturnsEmptySet(
            OrderType orderType,
            [Frozen] BuyingCatalogueDbContext context,
            List<Supplier> suppliers,
            SupplierService service)
        {
            suppliers.ForEach(x => x.IsActive = true);

            context.Suppliers.AddRange(suppliers);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await service.GetActiveSuppliers(orderType);

            result.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbInlineAutoData(OrderTypeEnum.Solution)]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static async Task GetAllSuppliersByOrderType_SuppliersNoPublishedSolutions_ReturnsEmptySet(
            OrderType orderType,
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

            var result = await service.GetActiveSuppliers(orderType);

            result.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetAllSuppliersByOrderType_Solution_MatchingSuppliers_ReturnsExpected(
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

            var result = await service.GetActiveSuppliers(OrderTypeEnum.Solution);

            result.Should().BeEquivalentTo(suppliers);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetAllSuppliersByOrderType_Solution_ExpiredFramework_ReturnsExpected(
            List<Solution> solutions,
            List<Supplier> suppliers,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierService service)
        {
            solutions.Take(1).ToList().ForEach(x => x.FrameworkSolutions.ToList().ForEach(y => y.Framework.IsExpired = true));
            solutions.Skip(1).ToList().ForEach(x => x.FrameworkSolutions.ToList().ForEach(y => y.Framework.IsExpired = false));
            solutions.ForEach(
                x =>
                {
                    x.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
                    x.CatalogueItem.PublishedStatus = PublicationStatus.Published;
                    x.CatalogueItem.Supplier = null;
                    x.AdditionalServices = new List<AdditionalService>();
                });

            context.Solutions.AddRange(solutions);

            for (var i = 0; i < suppliers.Count; i++)
            {
                suppliers[i].IsActive = true;
                suppliers[i].CatalogueItems.Add(solutions[i].CatalogueItem);
            }

            context.Suppliers.AddRange(suppliers);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await service.GetActiveSuppliers(OrderTypeEnum.Solution);

            result.Should()
                .BeEquivalentTo(
                    suppliers.Skip(1),
                    opt => opt.Excluding(m => m.CatalogueItems).Excluding(m => m.SupplierContacts));
        }

        [Theory]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static async Task GetAllSuppliersByOrderType_NoServiceAssociations_ReturnsExpected(
            OrderType orderType,
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
            context.ChangeTracker.Clear();

            var results = await service.GetActiveSuppliers(orderType);

            results.Should().HaveCount(0);
        }

        [Theory]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static async Task GetAllSuppliersByOrderType_WithServiceAssociations_ReturnsExpected(
            OrderType orderType,
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
            associatedService.PracticeReorganisationType = orderType.ToPracticeReorganisationType;

            supplier.IsActive = true;
            supplier.CatalogueItems.Add(solution.CatalogueItem);

            dbContext.Solutions.AddRange(solutions);
            dbContext.AssociatedServices.AddRange(associatedService);
            dbContext.Suppliers.AddRange(suppliers);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var results = await service.GetActiveSuppliers(orderType);

            results.Should().HaveCount(1);
            results[0].Should().BeEquivalentTo(supplier, opt => opt.Excluding(s => s.SupplierContacts).Excluding(s => s.CatalogueItems));
        }

        [Theory]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [InMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static async Task GetAllSuppliersByOrderType_ExpiredFramework_ReturnsExpected(
            OrderType orderType,
            [Frozen] BuyingCatalogueDbContext dbContext,
            List<Solution> solutions,
            List<AssociatedService> associatedServices,
            List<Supplier> suppliers,
            SupplierService service)
        {
            solutions.Take(1).ToList().ForEach(x => x.FrameworkSolutions.ToList().ForEach(y => y.Framework.IsExpired = true));
            solutions.Skip(1).ToList().ForEach(x => x.FrameworkSolutions.ToList().ForEach(y => y.Framework.IsExpired = false));
            solutions.ForEach(s => s.CatalogueItem.SupplierServiceAssociations = new List<SupplierServiceAssociation>());

            var solution = solutions.First();
            var associatedService = associatedServices.First();
            var supplier = suppliers.First();

            solution.CatalogueItem.SupplierServiceAssociations.Add(new(solution.CatalogueItemId, associatedService.CatalogueItemId));
            solution.CatalogueItem.Supplier = supplier;
            associatedService.CatalogueItem.Supplier = supplier;
            associatedService.PracticeReorganisationType = orderType.ToPracticeReorganisationType;

            supplier.IsActive = true;
            supplier.CatalogueItems.Add(solution.CatalogueItem);

            dbContext.Solutions.AddRange(solutions);
            dbContext.AssociatedServices.AddRange(associatedService);
            dbContext.Suppliers.AddRange(suppliers);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var results = await service.GetActiveSuppliers(orderType);

            results.Should().BeEmpty();
        }
    }
}
