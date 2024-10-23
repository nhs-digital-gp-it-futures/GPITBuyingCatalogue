using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using LinqKit;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SolutionsFilterModels;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions
{
    public static class SolutionsFilterServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SolutionsFilterService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task NoFiltering_NothingToFind(SolutionsFilterService service)
        {
            var result = await service.GetFilteredAndNonFilteredQueryResults(null);
            result.CatalogueItems.Should().BeEmpty();
            result.CapabilitiesAndCount.Should().BeEmpty();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task NoFiltering(
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsFilterService service,
            Solution solution)
        {
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await service.GetFilteredAndNonFilteredQueryResults(null);
            result.CatalogueItems.Should().HaveCount(1);
            result.CapabilitiesAndCount.Should().BeEmpty();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Filtering_By_Capability_Finds_Solution(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsFilterService service)
        {
            solution.FrameworkSolutions.ForEach(f => f.Framework.IsExpired = false);
            context.Solutions.Add(solution);
            var capability = solution.CatalogueItem.CatalogueItemCapabilities.First().Capability;

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var filter = new Dictionary<int, string[]>() { { capability.Id, null } };
            var result = await service.GetFilteredAndNonFilteredQueryResults(filter);
            result.CatalogueItems.Should().HaveCount(1);
            result.CapabilitiesAndCount.Should().HaveCount(1);
            result.CapabilitiesAndCount.Should().BeEquivalentTo(new List<CapabilitiesAndCountModel>
            {
                new()
                {
                    CapabilityId = capability.Id,
                    CapabilityName = capability.Name,
                    CountOfEpics = 0,
                },
            });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Filtering_By_Capability_Does_Not_Find_Solution(
            Capability capability,
            Solution solution,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsFilterService service)
        {
            solution.FrameworkSolutions.ForEach(f => f.Framework.IsExpired = false);
            context.Solutions.Add(solution);
            context.Capabilities.Add(capability);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var filter = new Dictionary<int, string[]>() { { capability.Id, null } };
            var result = await service.GetFilteredAndNonFilteredQueryResults(filter);
            result.CatalogueItems.Should().HaveCount(0);
            result.CapabilitiesAndCount.Should().HaveCount(1);
            result.CapabilitiesAndCount.Should().BeEquivalentTo(new List<CapabilitiesAndCountModel>
            {
                new()
                {
                    CapabilityId = capability.Id,
                    CapabilityName = capability.Name,
                    CountOfEpics = 0,
                },
            });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Filtering_By_Capability_And_Epic_Finds_Solution(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsFilterService service)
        {
            solution.FrameworkSolutions.ForEach(f => f.Framework.IsExpired = false);
            var capability = solution.CatalogueItem.CatalogueItemCapabilities.First().Capability;
            var epic = capability.Epics.First();
            solution.CatalogueItem.CatalogueItemEpics.Add(new CatalogueItemEpic()
            {
                CapabilityId = capability.Id,
                EpicId = epic.Id,
            });
            context.Solutions.Add(solution);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var filter = new Dictionary<int, string[]>() { { capability.Id, new string[] { epic.Id } } };
            var result = await service.GetFilteredAndNonFilteredQueryResults(filter);
            result.CatalogueItems.Should().HaveCount(1);
            result.CapabilitiesAndCount.Should().HaveCount(1);
            result.CapabilitiesAndCount.Should().BeEquivalentTo(new List<CapabilitiesAndCountModel>
            {
                new()
                {
                    CapabilityId = capability.Id,
                    CapabilityName = capability.Name,
                    CountOfEpics = 1,
                },
            });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Filtering_By_Capability_And_Epic_Does_Not_Find_Solution(
            Solution solution,
            Epic epic,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsFilterService service)
        {
            solution.FrameworkSolutions.ForEach(f => f.Framework.IsExpired = false);
            var capability = solution.CatalogueItem.CatalogueItemCapabilities.First().Capability;
            capability.Epics.Add(epic);
            context.Solutions.Add(solution);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var filter = new Dictionary<int, string[]>() { { capability.Id, new string[] { epic.Id } } };
            var result = await service.GetFilteredAndNonFilteredQueryResults(filter);
            result.CatalogueItems.Should().HaveCount(0);
            result.CapabilitiesAndCount.Should().HaveCount(1);
            result.CapabilitiesAndCount.Should().BeEquivalentTo(new List<CapabilitiesAndCountModel>
            {
                new()
                {
                    CapabilityId = capability.Id,
                    CapabilityName = capability.Name,
                    CountOfEpics = 1,
                },
            });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Filtering_By_Capability_Finds_AdditionalService(
            Solution solution,
            AdditionalService additionalService,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsFilterService service)
        {
            additionalService.CatalogueItem.PublishedStatus = PublicationStatus.Published;
            additionalService.SolutionId = solution.CatalogueItemId;
            additionalService.Solution = solution;

            solution.AdditionalServices.Add(additionalService);
            context.Solutions.Add(solution);

            var capability = additionalService.CatalogueItem.CatalogueItemCapabilities.First().Capability;

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var filter = new Dictionary<int, string[]>() { { capability.Id, null } };
            var result = await service.GetFilteredAndNonFilteredQueryResults(filter);
            result.CatalogueItems.Should().HaveCount(1);
            result.CapabilitiesAndCount.Should().HaveCount(1);
            result.CapabilitiesAndCount.Should().BeEquivalentTo(new List<CapabilitiesAndCountModel>
            {
                new()
                {
                    CapabilityId = capability.Id,
                    CapabilityName = capability.Name,
                    CountOfEpics = 0,
                },
            });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Filtering_By_Capability_Does_Not_Find_AdditionalService(
            Capability capability,
            Solution solution,
            AdditionalService additionalService,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsFilterService service)
        {
            additionalService.CatalogueItem.PublishedStatus = PublicationStatus.Published;
            additionalService.SolutionId = solution.CatalogueItemId;
            additionalService.Solution = solution;

            solution.AdditionalServices.Add(additionalService);
            context.Solutions.Add(solution);
            context.Capabilities.Add(capability);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var filter = new Dictionary<int, string[]>() { { capability.Id, null } };
            var result = await service.GetFilteredAndNonFilteredQueryResults(filter);
            result.CatalogueItems.Should().HaveCount(0);
            result.CapabilitiesAndCount.Should().HaveCount(1);
            result.CapabilitiesAndCount.Should().BeEquivalentTo(new List<CapabilitiesAndCountModel>
            {
                new()
                {
                    CapabilityId = capability.Id,
                    CapabilityName = capability.Name,
                    CountOfEpics = 0,
                },
            });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Filtering_By_Capability_And_Epic_Finds_AdditionalService(
            Solution solution,
            AdditionalService additionalService,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsFilterService service)
        {
            additionalService.CatalogueItem.PublishedStatus = PublicationStatus.Published;
            additionalService.SolutionId = solution.CatalogueItemId;
            additionalService.Solution = solution;

            var capability = additionalService.CatalogueItem.CatalogueItemCapabilities.First().Capability;
            var epic = capability.Epics.First();

            additionalService.CatalogueItem.CatalogueItemEpics.Add(new CatalogueItemEpic()
            {
                CapabilityId = capability.Id,
                EpicId = epic.Id,
            });

            solution.AdditionalServices.Add(additionalService);
            context.Solutions.Add(solution);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var filter = new Dictionary<int, string[]>() { { capability.Id, new string[] { epic.Id } } };
            var result = await service.GetFilteredAndNonFilteredQueryResults(filter);
            result.CatalogueItems.Should().HaveCount(1);
            result.CapabilitiesAndCount.Should().HaveCount(1);
            result.CapabilitiesAndCount.Should().BeEquivalentTo(new List<CapabilitiesAndCountModel>
            {
                new()
                {
                    CapabilityId = capability.Id,
                    CapabilityName = capability.Name,
                    CountOfEpics = 1,
                },
            });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task Filtering_By_Capability_And_Epic_Does_Not_Find_AdditionalService(
            Solution solution,
            AdditionalService additionalService,
            Epic epic,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsFilterService service)
        {
            additionalService.CatalogueItem.PublishedStatus = PublicationStatus.Published;
            additionalService.SolutionId = solution.CatalogueItemId;
            additionalService.Solution = solution;

            var capability = solution.CatalogueItem.CatalogueItemCapabilities.First().Capability;

            capability.Epics.Add(epic);
            solution.AdditionalServices.Add(additionalService);
            context.Solutions.Add(solution);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var filter = new Dictionary<int, string[]>() { { capability.Id, new string[] { epic.Id } } };
            var result = await service.GetFilteredAndNonFilteredQueryResults(filter);
            result.CatalogueItems.Should().HaveCount(0);
            result.CapabilitiesAndCount.Should().HaveCount(1);
            result.CapabilitiesAndCount.Should().BeEquivalentTo(new List<CapabilitiesAndCountModel>
            {
                new()
                {
                    CapabilityId = capability.Id,
                    CapabilityName = capability.Name,
                    CountOfEpics = 1,
                },
            });
        }
    }
}
