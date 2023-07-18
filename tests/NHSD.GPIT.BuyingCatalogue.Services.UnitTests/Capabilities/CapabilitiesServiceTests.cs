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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Capabilities;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
        public static async Task GetCapabilitiesByIds_ReturnsExpectedResult(
            List<Capability> capabilities,
            [Frozen] BuyingCatalogueDbContext context,
            CapabilitiesService service)
        {
            context.Capabilities.AddRange(capabilities);
            context.SaveChanges();

            var capabilityIds = capabilities.Select(x => x.Id);
            var result = await service.GetCapabilitiesByIds(capabilityIds);

            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(capabilities.Count);
            result.ForEach(x => capabilities.Should().Contain(c => c.Id == x.Id));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetCapabilitiesByCategory_ReturnsCapabilityCategories(
            [Frozen] BuyingCatalogueDbContext context,
            CapabilityCategory capabilityCategory,
            Capability capability,
            Epic mustEpic,
            Epic mayEpic,
            Epic shouldEpic,
            CapabilitiesService capabilitiesService)
        {
            capability.Epics.Clear();
            capability.CapabilityEpics.Clear();
            capabilityCategory.Capabilities.Add(capability);
            capability.CapabilityEpics.Add(new CapabilityEpic()
            {
                EpicId = mustEpic.Id,
                CompliancyLevel = CompliancyLevel.Must,
            });
            capability.CapabilityEpics.Add(new CapabilityEpic()
            {
                EpicId = shouldEpic.Id,
                CompliancyLevel = CompliancyLevel.Should,
            });
            capability.CapabilityEpics.Add(new CapabilityEpic()
            {
                EpicId = mayEpic.Id,
                CompliancyLevel = CompliancyLevel.May,
            });
            mustEpic.IsActive = true;
            mayEpic.IsActive = true;
            shouldEpic.IsActive = true;

            context.Epics.AddRange(new[] { mustEpic, mayEpic, shouldEpic });
            context.CapabilityCategories.Add(capabilityCategory);
            context.SaveChanges();
            context.ChangeTracker.Clear();

            var capabilityCategories = await capabilitiesService.GetCapabilitiesByCategory();

            capabilityCategories.Count.Should().Be(1);

            var capabilityResult = capabilityCategories.First().Capabilities.First();
            capabilityResult.GetActiveMustEpics().Select(e => e.Id).Should().BeEquivalentTo(new[] { mustEpic.Id });
            capabilityResult.GetActiveMayEpics().Select(e => e.Id).Should().BeEquivalentTo(new[] { mayEpic.Id });
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
            Capability capability,
            [Frozen] BuyingCatalogueDbContext context,
            CapabilitiesService capabilitiesService)
        {
            epic.Capabilities.Add(capability);
            context.CatalogueItems.Add(catalogueItem);
            context.SaveChanges();

            var model = new SaveCatalogueItemCapabilitiesModel
            {
                UserId = userId,
                Capabilities = new Dictionary<int, string[]>
                {
                    [capability.Id] = new string[1] { epic.Id },
                },
            };

            await capabilitiesService.AddCapabilitiesToCatalogueItem(catalogueItem.Id, model);

            catalogueItem.CatalogueItemCapabilities.FirstOrDefault(c => c.CapabilityId == capability.Id).Should().NotBeNull();
            catalogueItem.CatalogueItemEpics.FirstOrDefault(e => e.CapabilityId == capability.Id && e.EpicId == epic.Id).Should().NotBeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddCapabilitiesToCatalogueItem_RemovesStaleCapabilities(
            int userId,
            CatalogueItem catalogueItem,
            Epic epic,
            Capability capability,
            [Frozen] BuyingCatalogueDbContext context,
            CapabilitiesService capabilitiesService)
        {
            epic.Capabilities.Add(capability);
            context.CatalogueItems.Add(catalogueItem);
            context.SaveChanges();

            var originalNumberOfCapabilities = catalogueItem.CatalogueItemCapabilities.Count;

            var model = new SaveCatalogueItemCapabilitiesModel
            {
                UserId = userId,
                Capabilities = new Dictionary<int, string[]>
                {
                    [capability.Id] = new string[1] { epic.Id },
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
            Capability capability,
            List<CatalogueItemEpic> staleEpics,
            [Frozen] BuyingCatalogueDbContext context,
            CapabilitiesService capabilitiesService)
        {
            epic.Capabilities.Add(capability);
            staleEpics.ForEach(catalogueItem.CatalogueItemEpics.Add);
            context.CatalogueItems.Add(catalogueItem);
            context.SaveChanges();

            var model = new SaveCatalogueItemCapabilitiesModel
            {
                UserId = userId,
                Capabilities = new Dictionary<int, string[]>
                {
                    [capability.Id] = new string[1] { epic.Id },
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

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetReferencedCapabilities_NoItemsReferencingCapabilities_ReturnsEmpty(
            List<Capability> capabilities,
            [Frozen] BuyingCatalogueDbContext dbContext,
            CapabilitiesService service)
        {
            dbContext.AddRange(capabilities);
            dbContext.CatalogueItemCapabilities.RemoveRange(dbContext.CatalogueItemCapabilities);
            await dbContext.SaveChangesAsync();

            var referencedCapabilities = await service.GetReferencedCapabilities();

            referencedCapabilities.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetReferencedCapabilities_WithItemsReferencingCapabilities_ReturnsCapabilities(
            List<Capability> capabilities,
            List<CatalogueItem> catalogueItems,
            [Frozen] BuyingCatalogueDbContext dbContext,
            CapabilitiesService service)
        {
            capabilities.ForEach(x => x.CatalogueItemCapabilities = null);
            catalogueItems.ForEach(
                x =>
                {
                    x.CatalogueItemCapabilities = null;
                    x.PublishedStatus = PublicationStatus.Published;
                });

            var catalogueItemCapabilities = capabilities.Zip(catalogueItems)
                .Select(x => new CatalogueItemCapability(x.Second.Id, x.First.Id))
                .ToList();

            dbContext.Capabilities.AddRange(capabilities);
            dbContext.CatalogueItems.AddRange(catalogueItems);
            dbContext.CatalogueItemCapabilities.AddRange(catalogueItemCapabilities);
            await dbContext.SaveChangesAsync();

            var referencedCapabilities = await service.GetReferencedCapabilities();

            capabilities.ForEach(x => referencedCapabilities.Should().Contain(y => x.Id == y.Id));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task
            GetReferencedCapabilities_WithUnpublishedItems_ReturnsReferencedCapabilitiesByPublishedItems(
                List<Capability> capabilities,
                List<CatalogueItem> catalogueItems,
                [Frozen] BuyingCatalogueDbContext dbContext,
                CapabilitiesService service)
        {
            capabilities.ForEach(x => x.CatalogueItemCapabilities = null);
            catalogueItems.ForEach(x => x.CatalogueItemCapabilities = null);

            catalogueItems.Take(2).ToList().ForEach(x => x.PublishedStatus = PublicationStatus.Unpublished);

            var catalogueItemCapabilities = capabilities.Zip(catalogueItems)
                .Select(x => new CatalogueItemCapability(x.Second.Id, x.First.Id))
                .ToList();

            dbContext.Capabilities.AddRange(capabilities);
            dbContext.CatalogueItems.AddRange(catalogueItems);
            dbContext.CatalogueItemCapabilities.AddRange(catalogueItemCapabilities);
            await dbContext.SaveChangesAsync();

            var unpublishedItemCapabilities = catalogueItemCapabilities
                .Where(x => x.CatalogueItem.PublishedStatus == PublicationStatus.Unpublished)
                .Select(x => x.Capability)
                .Distinct()
                .ToList();

            var referencedCapabilities = await service.GetReferencedCapabilities();

            unpublishedItemCapabilities.ForEach(x => referencedCapabilities.Should().NotContain(y => x.Id == y.Id));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetGroupedCapabilitiesAndEpics_Returns_Expected(
                List<Capability> capabilities,
                [Frozen] BuyingCatalogueDbContext dbContext,
                CapabilitiesService service)
        {
            dbContext.Capabilities.AddRange(capabilities);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var capabilitiesAndEpics = new Dictionary<int, string[]>(capabilities.Select(
                                y => new KeyValuePair<int, string[]>(
                                    y.Id,
                                    new string[] { y.Epics.First().Id })));

            var result = await service.GetGroupedCapabilitiesAndEpics(capabilitiesAndEpics);

            result.Count.Should().Be(capabilities.Count);
            foreach (var keyValue in capabilitiesAndEpics)
            {
                var capability = capabilities.First(c => c.Id == keyValue.Key);
                var epic = capability.Epics.First(e => e.Id == keyValue.Value[0]);
                result[capability.Name].Should().NotBeNull();
                result[capability.Name].Count().Should().Be(1);
                result[capability.Name].First().Id.Should().Be(epic.Id);
            }
        }
    }
}
