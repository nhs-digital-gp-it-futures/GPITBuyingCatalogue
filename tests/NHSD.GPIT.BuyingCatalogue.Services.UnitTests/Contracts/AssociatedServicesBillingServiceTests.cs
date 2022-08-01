using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Contracts;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Contracts
{
    public class AssociatedServicesBillingServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AssociatedServicesBillingService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetAssociatedServiceOrderItems_ReturnsExpected(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            AssociatedServicesBillingService service)
        {
            order.OrderItems.ForEach(oi => oi.CatalogueItem.CatalogueItemType = EntityFramework.Catalogue.Models.CatalogueItemType.Solution);
            order.OrderItems.FirstOrDefault().CatalogueItem.CatalogueItemType = EntityFramework.Catalogue.Models.CatalogueItemType.AssociatedService;

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var results = await service.GetAssociatedServiceOrderItems(order.OrderingParty.InternalIdentifier, order.CallOffId);

            order.OrderItems.Count.Should().BeGreaterThan(1);
            results.Should().HaveCount(1);
            results.First().Should().Be(order.OrderItems.First());
        }
    }
}
