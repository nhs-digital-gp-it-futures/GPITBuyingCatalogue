using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Quantity;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.Quantity
{
    public static class SelectServiceRecipientQuantityModelTests
    {
        [Fact]
        public static void OrderItemIsNull_ThrowsException()
        {
            FluentActions
                .Invoking(() => new SelectServiceRecipientQuantityModel(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidOrderItem_PropertiesCorrectlySet(OrderItem item)
        {
            item.OrderItemPrice.ProvisioningType = ProvisioningType.Patient;

            var model = new SelectServiceRecipientQuantityModel(item);

            model.ItemName.Should().Be(item.CatalogueItem.Name);
            model.ItemType.Should().Be(item.CatalogueItem.CatalogueItemType.Description());
            model.ServiceRecipients.Length.Should().Be(item.OrderItemRecipients.Count);

            foreach (var serviceRecipient in model.ServiceRecipients)
            {
                var recipient = item.OrderItemRecipients.Single(x => x.OdsCode == serviceRecipient.OdsCode);

                serviceRecipient.Name.Should().Be(recipient.Recipient?.Name);
                serviceRecipient.InputQuantity.Should().Be($"{recipient.Quantity}");
                serviceRecipient.Quantity.Should().Be(recipient.Quantity);
            }
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidOrderItem_PerServiceRecipient_QuantityCorrectlySet(OrderItem item)
        {
            item.OrderItemPrice.ProvisioningType = ProvisioningType.PerServiceRecipient;

            var model = new SelectServiceRecipientQuantityModel(item);

            model.ItemName.Should().Be(item.CatalogueItem.Name);
            model.ItemType.Should().Be(item.CatalogueItem.CatalogueItemType.Description());
            model.ServiceRecipients.Length.Should().Be(item.OrderItemRecipients.Count);

            foreach (var serviceRecipient in model.ServiceRecipients)
            {
                var recipient = item.OrderItemRecipients.Single(x => x.OdsCode == serviceRecipient.OdsCode);

                serviceRecipient.Name.Should().Be(recipient.Recipient?.Name);
                serviceRecipient.InputQuantity.Should().BeNull();
                serviceRecipient.Quantity.Should().Be(1);
            }
        }
    }
}
