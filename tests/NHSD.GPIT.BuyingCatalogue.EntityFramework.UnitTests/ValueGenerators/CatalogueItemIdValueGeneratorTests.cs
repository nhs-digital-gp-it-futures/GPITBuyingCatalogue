using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.ValueGenerators
{
    public static class CatalogueItemIdValueGeneratorTests
    {
        [Theory]
        [InMemoryDbAutoData]
        public static void GenerateId_Solution_ExpectedId(
            Supplier supplier,
            BuyingCatalogueDbContext context)
        {
            context.CatalogueItems.ToList().ForEach(ci => context.CatalogueItems.Remove(ci));
            context.SaveChanges();

            var catalogueItem = AddSolution(supplier, context);

            catalogueItem.Id.Should().Be(new CatalogueItemId(catalogueItem.SupplierId, "001"));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void GenerateId_SupplierWithExistingSolution_IncrementsAsExpected(
            Supplier supplier,
            BuyingCatalogueDbContext context)
        {
            context.CatalogueItems.ToList().ForEach(ci => context.CatalogueItems.Remove(ci));
            context.SaveChanges();

            var existingCatalogueItem = AddSolution(supplier, context);
            var newCatalogueItem = AddSolution(supplier, context);

            newCatalogueItem.Id.Should().Be(new CatalogueItemId(newCatalogueItem.SupplierId, "002"));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void GenerateId_SolutionsAcrossSuppliers_ExpectedIds(
            Supplier firstSupplier,
            Supplier secondSupplier,
            BuyingCatalogueDbContext context)
        {
            context.CatalogueItems.ToList().ForEach(ci => context.CatalogueItems.Remove(ci));
            context.SaveChanges();

            var firstSupplierSolution = AddSolution(firstSupplier, context);
            var secondSupplierSolution = AddSolution(secondSupplier, context);

            firstSupplierSolution.Id.Should().Be(new CatalogueItemId(firstSupplierSolution.SupplierId, "001"));
            secondSupplierSolution.Id.Should().Be(new CatalogueItemId(secondSupplierSolution.SupplierId, "001"));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void GenerateId_AdditionalService_ExpectedId(
            Supplier supplier,
            BuyingCatalogueDbContext context)
        {
            context.CatalogueItems.ToList().ForEach(ci => context.CatalogueItems.Remove(ci));
            context.SaveChanges();

            var solution = AddSolution(supplier, context);

            var additionalService = AddAdditionalService(supplier, solution, context);

            additionalService.Id.Should().Be(new CatalogueItemId(additionalService.SupplierId, $"{solution.Id.ItemId}A001"));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void GenerateId_AdditionalServicesAcrossSuppliers_ExpectedIds(
            Supplier firstSupplier,
            Supplier secondSupplier,
            BuyingCatalogueDbContext context)
        {
            context.CatalogueItems.ToList().ForEach(ci => context.CatalogueItems.Remove(ci));
            context.SaveChanges();

            var firstSolution = AddSolution(firstSupplier, context);
            var secondSolution = AddSolution(secondSupplier, context);

            var firstAdditionalService = AddAdditionalService(firstSupplier, firstSolution, context);
            var secondAdditionalService = AddAdditionalService(secondSupplier, secondSolution, context);

            firstAdditionalService.Id.Should().Be(new CatalogueItemId(firstAdditionalService.SupplierId, $"{firstSolution.Id.ItemId}A001"));
            secondAdditionalService.Id.Should().Be(new CatalogueItemId(secondAdditionalService.SupplierId, $"{secondSolution.Id.ItemId}A001"));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void GenerateId_TwoSolutions_ExistingAdditionalService_ExpectedIds(
            Supplier supplier,
            BuyingCatalogueDbContext context)
        {
            context.CatalogueItems.ToList().ForEach(ci => context.CatalogueItems.Remove(ci));
            context.SaveChanges();

            var firstSolution = AddSolution(supplier, context);
            var secondSolution = AddSolution(supplier, context);

            var firstAdditionalService = AddAdditionalService(supplier, firstSolution, context);
            var secondAdditionalService = AddAdditionalService(supplier, secondSolution, context);

            firstAdditionalService.Id.Should().Be(new CatalogueItemId(firstAdditionalService.SupplierId, $"{firstSolution.Id.ItemId}A001"));
            secondAdditionalService.Id.Should().Be(new CatalogueItemId(secondAdditionalService.SupplierId, $"{secondSolution.Id.ItemId}A001"));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void GenerateId_OneSolution_ExistingAdditionalService_ExpectedIds(
            Supplier supplier,
            BuyingCatalogueDbContext context)
        {
            context.CatalogueItems.ToList().ForEach(ci => context.CatalogueItems.Remove(ci));
            context.SaveChanges();

            var solution = AddSolution(supplier, context);

            var firstAdditionalService = AddAdditionalService(supplier, solution, context);
            var secondAdditionalService = AddAdditionalService(supplier, solution, context);

            firstAdditionalService.Id.Should().Be(new CatalogueItemId(firstAdditionalService.SupplierId, $"{solution.Id.ItemId}A001"));
            secondAdditionalService.Id.Should().Be(new CatalogueItemId(secondAdditionalService.SupplierId, $"{solution.Id.ItemId}A002"));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void GenerateId_AssociatedService_ExpectedId(
            Supplier supplier,
            BuyingCatalogueDbContext context)
        {
            context.CatalogueItems.ToList().ForEach(ci => context.CatalogueItems.Remove(ci));
            context.SaveChanges();

            var associatedService = AddAssociatedService(supplier, context);

            associatedService.Id.Should().Be(new CatalogueItemId(associatedService.SupplierId, "S-001"));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void GenerateId_ExistingAssociatedService_ExpectedId(
            Supplier supplier,
            BuyingCatalogueDbContext context)
        {
            context.CatalogueItems.ToList().ForEach(ci => context.CatalogueItems.Remove(ci));
            context.SaveChanges();

            _ = AddAssociatedService(supplier, context);
            var associatedService = AddAssociatedService(supplier, context);

            associatedService.Id.Should().Be(new CatalogueItemId(associatedService.SupplierId, "S-002"));
        }

        private static CatalogueItem AddSolution(Supplier supplier, BuyingCatalogueDbContext context)
        {
            var solution = new CatalogueItem
            {
                CatalogueItemType = CatalogueItemType.Solution,
                Name = "Solution",
                Supplier = supplier,
                SupplierId = supplier.Id,
                Solution = new Solution(),
            };

            context.CatalogueItems.Add(solution);
            context.SaveChanges();

            return solution;
        }

        private static CatalogueItem AddAdditionalService(Supplier supplier, CatalogueItem solution, BuyingCatalogueDbContext context)
        {
            var additionalService = new CatalogueItem
            {
                CatalogueItemType = CatalogueItemType.AdditionalService,
                AdditionalService = new AdditionalService
                {
                    Solution = solution.Solution,
                    SolutionId = solution.Id,
                },
                Supplier = supplier,
                SupplierId = supplier.Id,
                Name = "Additional Service",
            };

            context.CatalogueItems.Add(additionalService);
            context.SaveChanges();

            return additionalService;
        }

        private static CatalogueItem AddAssociatedService(Supplier supplier, BuyingCatalogueDbContext context)
        {
            var associatedService = new CatalogueItem
            {
                CatalogueItemType = CatalogueItemType.AssociatedService,
                AssociatedService = new AssociatedService { },
                Supplier = supplier,
                SupplierId = supplier.Id,
                Name = "Associated Service",
            };

            context.CatalogueItems.Add(associatedService);
            context.SaveChanges();

            return associatedService;
        }
    }
}
