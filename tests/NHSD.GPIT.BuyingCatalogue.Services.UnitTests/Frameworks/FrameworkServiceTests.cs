using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
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
        public async Task GetFrameworks_ReturnsExpected(
            List<EntityFramework.Catalogue.Models.Framework> frameworks,
            [Frozen] BuyingCatalogueDbContext dbContext,
            FrameworkService service)
        {
            dbContext.Frameworks.AddRange(frameworks);
            await dbContext.SaveChangesAsync();

            var expectedFrameworks = frameworks.OrderBy(f => f.Id).ThenBy(f => f.Name);

            var result = await service.GetFrameworks();

            result.Should().BeEquivalentTo(expectedFrameworks);
        }
    }
}
