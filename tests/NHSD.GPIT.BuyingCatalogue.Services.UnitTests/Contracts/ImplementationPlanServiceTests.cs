using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Contracts;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Contracts
{
    public class ImplementationPlanServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ImplementationPlanService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetDefaultImplementationPlan_NoPlanExists_ReturnsNull(
            ImplementationPlanService service)
        {
            var plan = await service.GetDefaultImplementationPlan();

            plan.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetDefaultImplementationPlan_PlanExists_ReturnsPlan(
            [Frozen] BuyingCatalogueDbContext dbContext,
            ImplementationPlanService service)
        {
            var plan = new ImplementationPlan { IsDefault = true };

            dbContext.ImplementationPlans.Add(plan);

            await dbContext.SaveChangesAsync();

            var output = await service.GetDefaultImplementationPlan();

            output.Should().Be(plan);
        }
    }
}
