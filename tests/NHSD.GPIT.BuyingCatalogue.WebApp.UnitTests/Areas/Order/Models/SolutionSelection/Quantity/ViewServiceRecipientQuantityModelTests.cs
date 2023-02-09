using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
                .Invoking(() => new ViewServiceRecipientQuantityModel(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [CommonInlineAutoData(ProvisioningType.Declarative)]
        [CommonInlineAutoData(ProvisioningType.OnDemand)]
        [CommonInlineAutoData(ProvisioningType.PerServiceRecipient)]
        public static void WithValidOrderItem_PropertiesCorrectlySet(
            ProvisioningType provisioningType,
            OrderItem item)
        {
            item.OrderItemPrice.ProvisioningType = provisioningType;

            var model = new ViewServiceRecipientQuantityModel(item);

            model.Title.Should().Be(string.Format(ViewServiceRecipientQuantityModel.TitleText, model.ItemType));
            model.Caption.Should().Be(model.ItemName);
            model.Advice.Should().Be(ViewServiceRecipientQuantityModel.AdviceText);
            model.ItemName.Should().Be(item.CatalogueItem.Name);
            model.ItemType.Should().Be(item.CatalogueItem.CatalogueItemType.Description());
            model.QuantityColumnTitle.Should().Be(ViewServiceRecipientQuantityModel.QuantityColumnText);
            model.ServiceRecipients.Length.Should().Be(item.OrderItemRecipients.Count);

            foreach (var serviceRecipient in model.ServiceRecipients)
            {
                var recipient = item.OrderItemRecipients.First(x => x.OdsCode == serviceRecipient.OdsCode);

                serviceRecipient.Name.Should().Be(recipient.Recipient?.Name);
                serviceRecipient.Quantity.Should().Be(recipient.Quantity);
            }
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidOrderItem_Patient_PropertiesCorrectlySet(OrderItem item)
        {
            item.OrderItemPrice.ProvisioningType = ProvisioningType.Patient;

            var model = new ViewServiceRecipientQuantityModel(item);

            model.Title.Should().Be(string.Format(ViewServiceRecipientQuantityModel.TitleText, model.ItemType));
            model.Caption.Should().Be(model.ItemName);
            model.Advice.Should().Be(ViewServiceRecipientQuantityModel.AdviceText);
            model.ItemName.Should().Be(item.CatalogueItem.Name);
            model.ItemType.Should().Be(item.CatalogueItem.CatalogueItemType.Description());
            model.QuantityColumnTitle.Should().Be(ViewServiceRecipientQuantityModel.QuantityColumnPatientText);
            model.ServiceRecipients.Length.Should().Be(item.OrderItemRecipients.Count);

            foreach (var serviceRecipient in model.ServiceRecipients)
            {
                var recipient = item.OrderItemRecipients.First(x => x.OdsCode == serviceRecipient.OdsCode);

                serviceRecipient.Name.Should().Be(recipient.Recipient?.Name);
                serviceRecipient.Quantity.Should().Be(recipient.Quantity);
            }
        }
    }
}
