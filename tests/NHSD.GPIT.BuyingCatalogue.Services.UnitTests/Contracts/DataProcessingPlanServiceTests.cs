using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Orders.Contracts;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Contracts;

public class DataProcessingPlanServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(DataProcessingPlanService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task CreateDataProcessingPlan_CreatesPlan(
        [Frozen] BuyingCatalogueDbContext dbContext,
        DataProcessingPlanService service)
    {
        var result = await service.CreateDataProcessingPlan();

        result.Should().NotBeNull();
        dbContext.DataProcessingPlans.Should().HaveCount(1);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetDefaultDataProcessingPlan_GetsDefaultPlan(
        DataProcessingPlan defaultPlan,
        DataProcessingPlan secondPlan,
        [Frozen] BuyingCatalogueDbContext dbContext,
        DataProcessingPlanService service)
    {
        defaultPlan.IsDefault = true;
        secondPlan.IsDefault = false;

        dbContext.DataProcessingPlans.AddRange(new[] { defaultPlan, secondPlan });
        await dbContext.SaveChangesAsync();

        var result = await service.GetDefaultDataProcessingPlan();

        result.Should().BeEquivalentTo(defaultPlan);
    }
}
