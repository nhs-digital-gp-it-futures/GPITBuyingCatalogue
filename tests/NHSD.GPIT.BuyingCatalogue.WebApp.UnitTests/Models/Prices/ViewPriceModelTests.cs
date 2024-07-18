using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Prices
{
    public class ViewPriceModelTests
    {
        [Theory]
        [MockAutoData]
        public void WithValidOrderItem_SpecificPropertiesCorrectlySet(
            string internalOrgId,
            CallOffId callOffId,
            string onwardLink,
            OrderItem orderItem)
        {
            var model = new ViewPriceModel(orderItem.OrderItemPrice, orderItem.CatalogueItem)
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
