using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            var model = new ReviewExpanderModel(recipients, null, orderItem, null, isAmendment: false);

            model.IsAmendment.Should().BeFalse();
            model.IsOrderItemAdded.Should().BeTrue();
            model.OrderItemPrice.Should().Be(orderItem.OrderItemPrice);
            model.CatalogueItem.Should().Be(orderItem.CatalogueItem);
            model.RolledUpTotalQuantity.Should().Be(orderItem.TotalQuantity(null));
            model.PreviousTotalQuantity.Should().Be(0);
            recipients.ForEach(x => model.IsServiceRecipientAdded(x.OdsCode).Should().BeTrue());
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_AndPreviousVersion_PropertiesSetCorrectly(
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            var model = new ReviewExpanderModel(recipients, recipients, orderItem, orderItem, isAmendment: true);

            model.IsAmendment.Should().BeTrue();
            model.IsOrderItemAdded.Should().BeFalse();
            model.OrderItemPrice.Should().Be(orderItem.OrderItemPrice);
            model.CatalogueItem.Should().Be(orderItem.CatalogueItem);
            model.RolledUpTotalQuantity.Should().Be(orderItem.TotalQuantity(null));
            model.PreviousTotalQuantity.Should().Be(orderItem.TotalQuantity(null));
            recipients.ForEach(x => model.IsServiceRecipientAdded(x.OdsCode).Should().BeFalse());
        }
    }
}
