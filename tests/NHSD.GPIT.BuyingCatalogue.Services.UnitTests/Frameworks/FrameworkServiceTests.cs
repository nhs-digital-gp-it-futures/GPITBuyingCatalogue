using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.Services.Framework;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Frameworks
{
    public class FrameworkServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(FrameworkService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetFrameworksById_ReturnsExpected(
            EntityFramework.Catalogue.Models.Framework framework,
            [Frozen] BuyingCatalogueDbContext dbContext,
            FrameworkService service)
        {
            dbContext.Frameworks.Add(framework);
            await dbContext.SaveChangesAsync();

            var result = await service.GetFrameworksById(framework.Id);

            result.Should().BeEquivalentTo(framework);
        }

        [Theory]
        [InMemoryDbAutoData]
        public async Task GetFrameworksByCatalogueItems_PublishedItem_ReturnsExpected(
            EntityFramework.Catalogue.Models.Framework framework,
            FrameworkSolution frameworkSolution,
            CatalogueItem catalogueItem,
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            FrameworkService service)
        {
            dbContext.FrameworkSolutions.RemoveRange(dbContext.FrameworkSolutions);

            catalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            catalogueItem.PublishedStatus = PublicationStatus.Published;
            solution.FrameworkSolutions.Clear();

            solution.CatalogueItem = catalogueItem;
            frameworkSolution.Solution = solution;
            frameworkSolution.Framework = framework;

            dbContext.FrameworkSolutions.Add(frameworkSolution);
            dbContext.Frameworks.Add(framework);
            dbContext.CatalogueItems.Add(catalogueItem);
            dbContext.Solutions.Add(solution);

            await dbContext.SaveChangesAsync();

            var expectedFrameworks = new List<FrameworkFilterInfo>
            {
               new() { Id = framework.Id, ShortName = framework.ShortName, CountOfActiveSolutions = 1 },
            };

            var result = await service.GetFrameworksByCatalogueItems(new List<CatalogueItemId> { catalogueItem.Id });

            result.Should().HaveCount(1);
            result.Should().BeEquivalentTo(expectedFrameworks);
        }

        [Theory]
        [InMemoryDbAutoData]
        public async Task GetFrameworksByCatalogueItem_UnpublishedItem_ReturnsExpected(
            EntityFramework.Catalogue.Models.Framework framework,
            FrameworkSolution frameworkSolution,
            CatalogueItem catalogueItem,
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            FrameworkService service)
        {
            dbContext.FrameworkSolutions.RemoveRange(dbContext.FrameworkSolutions);

            catalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            catalogueItem.PublishedStatus = PublicationStatus.Unpublished;
            solution.FrameworkSolutions.Clear();

            solution.CatalogueItem = catalogueItem;
            frameworkSolution.Solution = solution;
            frameworkSolution.Framework = framework;

            dbContext.FrameworkSolutions.Add(frameworkSolution);
            dbContext.Frameworks.Add(framework);
            dbContext.CatalogueItems.Add(catalogueItem);
            dbContext.Solutions.Add(solution);

            await dbContext.SaveChangesAsync();

            var result = await service.GetFrameworksByCatalogueItems(new List<CatalogueItemId> { catalogueItem.Id });

            result.Should().BeEmpty();
        }
    }
}
