using System.Collections.Generic;
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
        public async Task GetFrameworksByCatalogueItems(
            FrameworkSolution frameworkSolutions,
            CatalogueItem catalogueItem,
            [Frozen] BuyingCatalogueDbContext dbContext,
            FrameworkService service)
        {
            dbContext.FrameworkSolutions.Add(frameworkSolutions);
            dbContext.CatalogueItems.Add(catalogueItem);
            await dbContext.SaveChangesAsync();

            var expectedFrameworks = await dbContext.FrameworkSolutions.AsNoTracking()
                .Where(fs => fs.SolutionId == catalogueItem.Id &&
                       fs.Solution.CatalogueItem.PublishedStatus == PublicationStatus.Published)
                .GroupBy(x => new { x.FrameworkId, x.Framework.ShortName })
                .Select(
                    x => new FrameworkFilterInfo
                    {
                        Id = x.Key.FrameworkId,
                        ShortName = x.Key.ShortName,
                        CountOfActiveSolutions = x.Count(),
                    })
                .ToListAsync();

            var result = await service.GetFrameworksByCatalogueItems(new List<CatalogueItem> { catalogueItem });

            result.Should().BeEquivalentTo(expectedFrameworks);
        }
    }
}
