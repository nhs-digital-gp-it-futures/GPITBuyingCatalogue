using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Capabilities;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Capabilities
{
    public static class EpicsServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(EpicsService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static void GetActiveEpicsByCapabilityIds_NullCapabilityIds_ThrowsException(
            EpicsService service)
        {
            FluentActions
                .Awaiting(() => service.GetReferencedEpicsByCapabilityIds(null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [MockInMemoryDbAutoData]
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
                epics[i].Capabilities.Add(capabilities[i]);
                epics[i].IsActive = false;
            }

            epics[0].IsActive = true;

            dbContext.Epics.AddRange(epics);
            await dbContext.SaveChangesAsync();

            dbContext.CatalogueItemEpics.Add(new(catalogueItem.Id, epics[0].Capabilities.First().Id, epics[0].Id));
            await dbContext.SaveChangesAsync();

            var result = await service.GetReferencedEpicsByCapabilityIds(capabilities.Select(x => x.Id));

            result.Should().ContainSingle();
            result.First().Id.Should().Be(epics[0].Id);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData("0", 2)]
        [MockInMemoryDbInlineAutoData("1", 2)]
        [MockInMemoryDbInlineAutoData("2", 1)]
        [MockInMemoryDbInlineAutoData("3", 0)]
        public static async Task GetEpics_WithPageOptions_ReturnsExpectedPage(
            string page,
            int pageSize,
            List<Epic> epics,
            [Frozen] BuyingCatalogueDbContext dbContext,
            EpicsService service)
        {
            dbContext.Epics.AddRange(epics);
            await dbContext.SaveChangesAsync();

            var pageOptions = new PageOptions(page, 2);

            var result = await service.GetEpics(pageOptions);

            result.Items.Should().HaveCount(pageSize);
            result.Options.Should().BeSameAs(pageOptions);
            result.Options.TotalNumberOfItems.Should().Be(epics.Count);
            result.Options.NumberOfPages.Should().Be((int)Math.Ceiling((double)epics.Count / 2));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetEpics_NullPageOptions_ReturnsAllEpicsWithPageOptions(
            List<Epic> epics,
            [Frozen] BuyingCatalogueDbContext dbContext,
            EpicsService service)
        {
            epics[0].IsActive = true;
            epics.Skip(1).ToList().ForEach(x => x.IsActive = false);

            dbContext.Epics.AddRange(epics);
            await dbContext.SaveChangesAsync();

            var result = await service.GetEpics(null);

            result.Items.Should().BeEquivalentTo(epics);
            result.Options.PageNumber.Should().Be(1);
            result.Options.PageSize.Should().Be(epics.Count);
            result.Options.TotalNumberOfItems.Should().Be(epics.Count);
            result.Options.NumberOfPages.Should().Be(1);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetEpics_NoEpics_ReturnsEmptyPagedList(
            [Frozen] BuyingCatalogueDbContext dbContext,
            EpicsService service)
        {
            dbContext.Epics.Should().BeEmpty();

            var result = await service.GetEpics(null);

            result.Items.Should().BeEmpty();
            result.Options.PageNumber.Should().Be(1);
            result.Options.PageSize.Should().Be(0);
            result.Options.TotalNumberOfItems.Should().Be(0);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static void GetEpicsByIds_NullEpicIds_ThrowsException(
            EpicsService service)
        {
            FluentActions
                .Awaiting(() => service.GetEpicsByIds(null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetEpicsByIds_ExpectedResult(
            List<Epic> epics,
            [Frozen] BuyingCatalogueDbContext dbContext,
            EpicsService service)
        {
            epics.Skip(1).ToList().ForEach(x => x.IsActive = false);
            epics.Take(1).ToList().ForEach(x => x.IsActive = true);

            dbContext.Epics.AddRange(epics);
            await dbContext.SaveChangesAsync();

            dbContext.ChangeTracker.Clear();

            var result = await service.GetEpicsByIds(epics.Select(x => x.Id));

            result.Should().ContainSingle();
            result.First().Id.Should().Be(epics[0].Id);
        }
    }
}
