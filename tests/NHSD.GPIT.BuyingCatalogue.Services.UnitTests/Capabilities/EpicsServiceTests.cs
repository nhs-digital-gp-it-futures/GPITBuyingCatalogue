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
using NHSD.GPIT.BuyingCatalogue.Services.Capabilities;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Capabilities
{
    public static class EpicsServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(EpicsService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void GetActiveEpicsByCapabilityIds_NullCapabilityIds_ThrowsException(
            EpicsService service)
        {
            FluentActions
                .Awaiting(() => service.GetReferencedEpicsByCapabilityIds(null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetActiveEpicsByCapabilityIds_ExpectedResult(
            CatalogueItem catalogueItem,
            CapabilityCategory category,
            List<Capability> capabilities,
            List<Epic> epics,
            [Frozen] BuyingCatalogueDbContext dbContext,
            EpicsService service)
        {
            dbContext.CatalogueItems.Add(catalogueItem);
            dbContext.CapabilityCategories.Add(category);
            await dbContext.SaveChangesAsync();

            capabilities.ForEach(x =>
            {
                x.CategoryId = category.Id;
                x.Epics.Clear();
            });

            dbContext.Capabilities.AddRange(capabilities);
            await dbContext.SaveChangesAsync();

            for (var i = 0; i < capabilities.Count; i++)
            {
                epics[i].CapabilityId = capabilities[i].Id;
                epics[i].IsActive = false;
            }

            epics[0].IsActive = true;

            dbContext.Epics.AddRange(epics);
            await dbContext.SaveChangesAsync();

            dbContext.CatalogueItemEpics.Add(new(catalogueItem.Id, epics[0].CapabilityId, epics[0].Id));
            await dbContext.SaveChangesAsync();

            var result = await service.GetReferencedEpicsByCapabilityIds(capabilities.Select(x => x.Id));

            result.Should().ContainSingle();
            result.First().Id.Should().Be(epics[0].Id);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void GetEpicsByIds_NullEpicIds_ThrowsException(
            EpicsService service)
        {
            FluentActions
                .Awaiting(() => service.GetEpicsByIds(null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetEpicsByIds_ExpectedResult(
            List<Epic> epics,
            List<Capability> capabilities,
            [Frozen] BuyingCatalogueDbContext dbContext,
            EpicsService service)
        {
            capabilities.ForEach(x =>
            {
                x.Epics.Clear();
            });

            dbContext.Capabilities.AddRange(capabilities);
            await dbContext.SaveChangesAsync();

            for (var i = 0; i < capabilities.Count; i++)
            {
                epics[i].CapabilityId = capabilities[i].Id;
                epics[i].IsActive = false;
            }

            epics[0].IsActive = true;

            dbContext.Epics.AddRange(epics);
            await dbContext.SaveChangesAsync();

            var result = await service.GetEpicsByIds(epics.Select(x => x.Id));

            result.Should().ContainSingle();
            result.First().Id.Should().Be(epics[0].Id);
        }
    }
}
