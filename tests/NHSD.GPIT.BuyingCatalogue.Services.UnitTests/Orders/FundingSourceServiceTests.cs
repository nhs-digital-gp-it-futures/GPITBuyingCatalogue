using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class FundingSourceServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(FundingSourceService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetFundingSource_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            bool fundingSource,
            FundingSourceService service)
        {
            order.FundingSourceOnlyGms = !fundingSource;
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            await service.SetFundingSource(order.CallOffId, order.OrderingParty.InternalIdentifier, fundingSource);

            var updatedOrder = await context.Orders.SingleAsync();
            updatedOrder.FundingSourceOnlyGms.Should().Be(fundingSource);
        }
    }
}
