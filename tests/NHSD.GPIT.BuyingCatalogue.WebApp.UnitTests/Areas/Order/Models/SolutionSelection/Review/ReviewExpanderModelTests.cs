using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Review;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.Review
{
    public static class ReviewExpanderModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesSetCorrectly(
            EntityFramework.Ordering.Models.OrderItem orderItem)
        {
            var model = new ReviewExpanderModel(orderItem, null, isAmendment: false);

            model.IsAmendment.Should().BeFalse();
            model.IsOrderItemAdded.Should().BeTrue();
            model.OrderItem.Should().Be(orderItem);

            orderItem.OrderItemRecipients.ForEach(x => model.IsServiceRecipientAdded(x.OdsCode).Should().BeTrue());
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_AndPreviousVersion_PropertiesSetCorrectly(
            EntityFramework.Ordering.Models.OrderItem orderItem)
        {
            var model = new ReviewExpanderModel(orderItem, orderItem, isAmendment: true);

            model.IsAmendment.Should().BeTrue();
            model.IsOrderItemAdded.Should().BeFalse();
            model.OrderItem.Should().Be(orderItem);

            orderItem.OrderItemRecipients.ForEach(x => model.IsServiceRecipientAdded(x.OdsCode).Should().BeFalse());
        }
    }
}
