using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Quantity;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.Quantity
{
    public static class ViewServiceRecipientQuantityModelTests
    {
        [Fact]
        public static void OrderItemIsNull_ThrowsException()
        {
            FluentActions
                .Invoking(() => new ViewServiceRecipientQuantityModel(null, null))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MockInlineAutoData(ProvisioningType.Declarative, ViewServiceRecipientQuantityModel.QuantityColumnText)]
        [MockInlineAutoData(ProvisioningType.OnDemand, ViewServiceRecipientQuantityModel.QuantityColumnText)]
        [MockInlineAutoData(ProvisioningType.Patient, ViewServiceRecipientQuantityModel.QuantityColumnPatientText)]
        public static void WithValidOrderItem_PropertiesCorrectlySet(
            ProvisioningType provisioningType,
            string expectedQuantityColumnTitle,
            OrderItem item,
            OrderRecipient[] recipients)
        {
            item.OrderItemPrice.ProvisioningType = provisioningType;

            var model = new ViewServiceRecipientQuantityModel(item, recipients);

            model.Title.Should().Be(string.Format(ViewServiceRecipientQuantityModel.TitleText, model.ItemType));
            model.Caption.Should().Be(model.ItemName);
            model.Advice.Should().Be(ViewServiceRecipientQuantityModel.AdviceText);
            model.ItemName.Should().Be(item.CatalogueItem.Name);
            model.ItemType.Should().Be(item.CatalogueItem.CatalogueItemType.Description());
            model.QuantityColumnTitle.Should().Be(expectedQuantityColumnTitle);
            model.ServiceRecipients.Length.Should().Be(recipients.Length);
        }
    }
}
