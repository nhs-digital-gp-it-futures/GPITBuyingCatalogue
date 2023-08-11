using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Prices
{
    public class ConfirmPriceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithDefaultConstructor_SpecificPropertiesCorrectlySet(
            RoutingSource routingSource)
        {
            var model = new ConfirmPriceModel
            {
                Source = routingSource,
            };

            model.Advice.Should().Be(ConfirmPriceModel.AdviceText);
            model.Hint.Should().Be(ConfirmPriceModel.HintText);
            model.Label.Should().Be(string.Format(ConfirmPriceModel.LabelText, model.Basis));
            model.Source.Should().Be(routingSource);
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidCatalogueItem_SpecificPropertiesCorrectlySet(
            OrderItem orderItem,
            RoutingSource routingSource)
        {
            var model = new ConfirmPriceModel(orderItem.OrderItemPrice, orderItem.CatalogueItem)
            {
                Source = routingSource,
            };

            model.Advice.Should().Be(ConfirmPriceModel.AdviceText);
            model.Hint.Should().Be(ConfirmPriceModel.HintText);
            model.Label.Should().Be(string.Format(ConfirmPriceModel.LabelText, model.Basis));
            model.Source.Should().Be(routingSource);
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidOrderItem_SpecificPropertiesCorrectlySet(
            OrderItem orderItem,
            RoutingSource routingSource)
        {
            var model = new ConfirmPriceModel(orderItem.OrderItemPrice, orderItem.CatalogueItem)
            {
                Source = routingSource,
            };

            model.Advice.Should().Be(ConfirmPriceModel.AdviceText);
            model.Hint.Should().Be(ConfirmPriceModel.HintText);
            model.Label.Should().Be(string.Format(ConfirmPriceModel.LabelText, model.Basis));
            model.Source.Should().Be(routingSource);
        }

        [Theory]
        [CommonAutoData]
        public static void AgreedPrices_ExpectedResult(OrderItem orderItem)
        {
            var model = new ConfirmPriceModel(orderItem.OrderItemPrice, orderItem.CatalogueItem);

            var actual = model.AgreedPrices;
            var expected = model.Tiers.Select(x => x.AgreedPriceDto).ToList();

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
