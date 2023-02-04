using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Prices;
using NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.Prices.Base;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.Prices
{
    public class ConfirmPriceModelTests : PricingModelTests
    {
        protected override Type ModelType => typeof(ConfirmPriceModel);

        [Theory]
        [CommonAutoData]
        public static void CatalogueItemIsNull_ThrowsException(int priceId)
        {
            FluentActions
                .Invoking(() => new ConfirmPriceModel(null, priceId, null))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public static void OrderItemIsNull_ThrowsException()
        {
            FluentActions
                .Invoking(() => new ConfirmPriceModel(null))
                .Should().Throw<ArgumentNullException>();
        }

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
            CatalogueItem catalogueItem,
            OrderItem orderItem,
            RoutingSource routingSource)
        {
            var priceId = catalogueItem.CataloguePrices.First().CataloguePriceId;

            var model = new ConfirmPriceModel(catalogueItem, priceId, orderItem)
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
            var model = new ConfirmPriceModel(orderItem)
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
            var model = new ConfirmPriceModel(orderItem);

            var actual = model.AgreedPrices;
            var expected = model.Tiers.Select(x => x.AgreedPriceDto).ToList();

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
