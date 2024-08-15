using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Order
{
    public static class CompletedModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order)
        {
            var model = new CompletedModel(internalOrgId, order);

            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Order.Should().Be(order);
        }

        [Theory]
        [MockAutoData]
        public static void NotAmendment_PropertiesCorrectlySet(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order)
        {
            order.Revision = 1;
            var model = new CompletedModel(internalOrgId, order);

            model.Title.Should().Be(string.Format(CompletedModel.TitleText, string.Empty));
            model.Advice.Should().Be(string.Format(CompletedModel.AdviceText, string.Empty));
        }

        [Theory]
        [MockAutoData]
        public static void Amendment_PropertiesCorrectlySet(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order)
        {
            order.Revision = 2;
            var model = new CompletedModel(internalOrgId, order);

            model.Title.Should().Be(string.Format(CompletedModel.TitleText, " amendment"));
            model.Advice.Should().Be(string.Format(CompletedModel.AdviceText, " amendments"));
        }
    }
}
