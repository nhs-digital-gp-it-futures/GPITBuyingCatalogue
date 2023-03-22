using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Prices;
using NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.Prices.Base;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.Prices
{
    public class ViewPriceModelTests : PricingModelTests
    {
        protected override Type ModelType => typeof(ViewPriceModel);

        [Theory]
        [CommonAutoData]
        public static void CatalogueItemIsNull_ThrowsException(int priceId)
        {
            FluentActions
                .Invoking(() => new ViewPriceModel(null, priceId, null))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public static void OrderItemIsNull_ThrowsException()
        {
            FluentActions
                .Invoking(() => new ViewPriceModel(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public void WithValidOrderItem_SpecificPropertiesCorrectlySet(
            string internalOrgId,
            CallOffId callOffId,
            string onwardLink,
            OrderItem orderItem)
        {
            var model = new ViewPriceModel(orderItem)
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                OnwardLink = onwardLink,
            };

            model.Advice.Should().Be(ViewPriceModel.AdviceText);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.CallOffId.Should().Be(callOffId);
            model.OnwardLink.Should().Be(onwardLink);
        }
    }
}
