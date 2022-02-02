using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Capabilities;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Capabilities
{
    public static class CapabilitiesServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CapabilitiesService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetCapabilitiesByCategory_ReturnsCapabilityCategories(
            [Frozen] BuyingCatalogueDbContext context,
            CapabilitiesService capabilitiesService)
        {
            var supplierId = 10020;
            var supplierIdString = supplierId.ToString(CultureInfo.InvariantCulture);
            var supplierKey = $"S{supplierIdString[^3..]}";

            var expectedCapabilities = await context
                .CapabilityCategories
                .Include(c => c.Capabilities)
                .ThenInclude(c => c.Epics.Where(e => e.IsActive && e.CompliancyLevel == CompliancyLevel.May))
                .ToListAsync();

            var capabilityCategories = await capabilitiesService.GetCapabilitiesByCategory();

            capabilityCategories.Should().BeEquivalentTo(expectedCapabilities);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static Task AddCapabilitiesToCatalogueItem_NullModel_ThrowsArgumentNullException(
            CatalogueItemId catalogueItemId,
            CapabilitiesService capabilitiesService) => Assert.ThrowsAsync<ArgumentNullException>(() => capabilitiesService.AddCapabilitiesToCatalogueItem(catalogueItemId, null));

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddCapabilitiesToCatalogueItem_AddsCapabilitiesAndEpics(
            int userId,
            CatalogueItem catalogueItem,
            Epic epic,
            [Frozen] BuyingCatalogueDbContext context,
            CapabilitiesService capabilitiesService)
        {
            context.CatalogueItems.Add(catalogueItem);
            context.SaveChanges();

            var model = new SaveCatalogueItemCapabilitiesModel
            {
                UserId = userId,
                Capabilities = new Dictionary<int, string[]>
                {
                    [epic.CapabilityId] = new string[1] { epic.Id },
                },
            };

            await capabilitiesService.AddCapabilitiesToCatalogueItem(catalogueItem.Id, model);

            catalogueItem.CatalogueItemCapabilities.FirstOrDefault(c => c.CapabilityId == epic.CapabilityId).Should().NotBeNull();
            catalogueItem.CatalogueItemEpics.FirstOrDefault(e => e.CapabilityId == epic.CapabilityId && e.EpicId == epic.Id).Should().NotBeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddCapabilitiesToCatalogueItem_RemovesStaleCapabilities(
            int userId,
            CatalogueItem catalogueItem,
            Epic epic,
            [Frozen] BuyingCatalogueDbContext context,
            CapabilitiesService capabilitiesService)
        {
            context.CatalogueItems.Add(catalogueItem);
            context.SaveChanges();

            var originalNumberOfCapabilities = catalogueItem.CatalogueItemCapabilities.Count;

            var model = new SaveCatalogueItemCapabilitiesModel
            {
                UserId = userId,
                Capabilities = new Dictionary<int, string[]>
                {
                    [epic.CapabilityId] = new string[1] { epic.Id },
                },
            };

            await capabilitiesService.AddCapabilitiesToCatalogueItem(catalogueItem.Id, model);

            catalogueItem.CatalogueItemCapabilities.Should().ContainSingle();
            catalogueItem.CatalogueItemCapabilities.Count.Should().BeLessThan(originalNumberOfCapabilities);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddCapabilitiesToCatalogueItem_RemovesStaleEpics(
            int userId,
            CatalogueItem catalogueItem,
            Epic epic,
            List<CatalogueItemEpic> staleEpics,
            [Frozen] BuyingCatalogueDbContext context,
            CapabilitiesService capabilitiesService)
        {
            staleEpics.ForEach(catalogueItem.CatalogueItemEpics.Add);
            context.CatalogueItems.Add(catalogueItem);
            context.SaveChanges();

            var model = new SaveCatalogueItemCapabilitiesModel
            {
                UserId = userId,
                Capabilities = new Dictionary<int, string[]>
                {
                    [epic.CapabilityId] = new string[1] { epic.Id },
                },
            };

            await capabilitiesService.AddCapabilitiesToCatalogueItem(catalogueItem.Id, model);

            catalogueItem.CatalogueItemEpics.Should().ContainSingle();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddCapabilitiesToCatalogueItem_WithDuplicateCapability_DoesNotAdd(
            int userId,
            CatalogueItem catalogueItem,
            [Frozen] BuyingCatalogueDbContext context,
            CapabilitiesService capabilitiesService)
        {
            context.CatalogueItems.Add(catalogueItem);
            context.SaveChanges();

            var expectedNumberOfCapabilities = catalogueItem.CatalogueItemCapabilities.Count;

            var model = new SaveCatalogueItemCapabilitiesModel
            {
                UserId = userId,
                Capabilities = catalogueItem.CatalogueItemCapabilities.ToDictionary(c => c.CapabilityId, c => Array.Empty<string>()),
            };

            await capabilitiesService.AddCapabilitiesToCatalogueItem(catalogueItem.Id, model);

            catalogueItem.CatalogueItemCapabilities.Count.Should().Be(expectedNumberOfCapabilities);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddCapabilitiesToCatalogueItem_WithDuplicateEpicId_DoesNotAdd(
            int userId,
            CatalogueItem catalogueItem,
            CatalogueItemEpic epic,
            [Frozen] BuyingCatalogueDbContext context,
            CapabilitiesService capabilitiesService)
        {
            catalogueItem.CatalogueItemEpics.Add(epic);
            context.CatalogueItems.Add(catalogueItem);
            context.SaveChanges();

            var expectedNumberOfEpics = catalogueItem.CatalogueItemEpics.Count;

            var model = new SaveCatalogueItemCapabilitiesModel
            {
                UserId = userId,
                Capabilities = new Dictionary<int, string[]>
                {
                    [epic.CapabilityId] = new[] { epic.EpicId },
                },
            };

            await capabilitiesService.AddCapabilitiesToCatalogueItem(catalogueItem.Id, model);

            catalogueItem.CatalogueItemEpics.Count.Should().Be(expectedNumberOfEpics);
        }
    }
}
