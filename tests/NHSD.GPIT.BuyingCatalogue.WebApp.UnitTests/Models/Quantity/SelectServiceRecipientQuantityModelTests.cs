using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Quantity
{
    public static class SelectServiceRecipientQuantityModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidOrderItem_PropertiesCorrectlySet(
            List<ServiceRecipientDto> serviceRecipients,
            OrderItem item)
        {
            item.OrderItemPrice.ProvisioningType = ProvisioningType.Patient;

            var model = new SelectServiceRecipientQuantityModel(item.CatalogueItem, item.OrderItemPrice, serviceRecipients);

            model.Title.Should().Be(string.Format(SelectServiceRecipientQuantityModel.TitleText, model.ItemType));
            model.Caption.Should().Be(model.ItemName);
            model.Advice.Should().Be(SelectServiceRecipientQuantityModel.AdviceTextPatient);
            model.ItemName.Should().Be(item.CatalogueItem.Name);
            model.ItemType.Should().Be(item.CatalogueItem.CatalogueItemType.Description());
            model.ServiceRecipients.Length.Should().Be(serviceRecipients.Count);

            foreach (var serviceRecipient in model.ServiceRecipients)
            {
                var recipient = serviceRecipients.First(x => x.OdsCode == serviceRecipient.OdsCode);

                serviceRecipient.Name.Should().Be(recipient.Name);
                serviceRecipient.InputQuantity.Should().Be($"{recipient.Quantity}");
                serviceRecipient.Quantity.Should().Be(recipient.Quantity);
            }
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidOrderItem_PerServiceRecipient_QuantityCorrectlySet(
            List<ServiceRecipientDto> serviceRecipients,
            OrderItem item)
        {
            item.OrderItemPrice.ProvisioningType = ProvisioningType.PerServiceRecipient;

            var model = new SelectServiceRecipientQuantityModel(item.CatalogueItem, item.OrderItemPrice, serviceRecipients);

            model.Title.Should().Be(string.Format(SelectServiceRecipientQuantityModel.TitleText, model.ItemType));
            model.Caption.Should().Be(model.ItemName);
            model.Advice.Should().Be(SelectServiceRecipientQuantityModel.AdviceTextServiceRecipient);
            model.ItemName.Should().Be(item.CatalogueItem.Name);
            model.ItemType.Should().Be(item.CatalogueItem.CatalogueItemType.Description());
            model.ServiceRecipients.Length.Should().Be(serviceRecipients.Count);

            foreach (var serviceRecipient in model.ServiceRecipients)
            {
                var recipient = serviceRecipients.First(x => x.OdsCode == serviceRecipient.OdsCode);

                serviceRecipient.Name.Should().Be(recipient.Name);
                serviceRecipient.InputQuantity.Should().Be("1");
                serviceRecipient.Quantity.Should().Be(1);
            }
        }
    }
}
