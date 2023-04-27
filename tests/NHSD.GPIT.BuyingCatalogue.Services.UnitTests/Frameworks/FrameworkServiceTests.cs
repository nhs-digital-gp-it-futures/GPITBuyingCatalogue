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
        public async Task GetFrameworksWithActiveAndPublishedSolutions_ReturnsExpected(
            EntityFramework.Catalogue.Models.Framework framework,
            EntityFramework.Catalogue.Models.FrameworkSolution frameworkSolutions,
            [Frozen] BuyingCatalogueDbContext dbContext,
            FrameworkService service)
        {
            dbContext.Frameworks.Add(framework);
            dbContext.FrameworkSolutions.Add(frameworkSolutions);
            await dbContext.SaveChangesAsync();

            var expectedFrameworks = dbContext.Frameworks
            .Join(dbContext.FrameworkSolutions, f => f.Id, fs => fs.FrameworkId, (f, fs) => new { Framework = f, FrameworkSolution = fs })
            .Join(dbContext.Solutions, fs => fs.FrameworkSolution.SolutionId, s => s.CatalogueItemId, (fs, s) => new { fs.Framework, Solution = s })
            .Join(dbContext.CatalogueItems, x => x.Solution.CatalogueItemId, ci => ci.Id, (x, ci) => new { x.Framework, x.Solution, CatalogueItem = ci })
            .Where(x => x.CatalogueItem.PublishedStatus == PublicationStatus.Published)
            .Select(x => x.Framework)
            .OrderBy(f => f.Id)
            .ThenBy(f => f.Name);

            var result = await service.GetFrameworksWithActiveAndPublishedSolutions();

            result.Should().BeEquivalentTo(expectedFrameworks);
        }
    }
}
