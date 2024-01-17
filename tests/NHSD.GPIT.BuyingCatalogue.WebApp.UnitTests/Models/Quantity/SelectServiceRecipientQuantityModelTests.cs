using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
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

            model.Title.Should().Be(string.Format(SelectServiceRecipientQuantityModel.TitleText, item.CatalogueItem.CatalogueItemType.Description()));
            model.Caption.Should().Be(item.CatalogueItem.Name);
            model.Advice.Should().Be(SelectServiceRecipientQuantityModel.AdviceTextPatient);
            model.OrderType.Should().BeNull();
            model.PracticeReorganisationRecipient.Should().BeNull();
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
        [CommonInlineAutoData(OrderTypeEnum.Solution)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        public static void WithValidOrderItem_Not_MergerSplit_PropertiesCorrectlySet(
            OrderType orderType,
            List<ServiceRecipientDto> serviceRecipients,
            OrderItem item)
        {
            item.OrderItemPrice.ProvisioningType = ProvisioningType.Patient;

            var model = new SelectServiceRecipientQuantityModel(orderType, null, item.CatalogueItem, item.OrderItemPrice, serviceRecipients, null);

            model.Title.Should().Be(string.Format(SelectServiceRecipientQuantityModel.TitleText, item.CatalogueItem.CatalogueItemType.Description()));
            model.Caption.Should().Be(item.CatalogueItem.Name);
            model.Advice.Should().Be(SelectServiceRecipientQuantityModel.AdviceTextPatient);
            model.OrderType.Should().Be(orderType);
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
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static void WithValidOrderItem_MergerSplit_PropertiesCorrectlySet(
            OrderType orderType,
            OdsOrganisation organisation,
            List<ServiceRecipientDto> serviceRecipients,
            OrderItem item)
        {
            item.OrderItemPrice.ProvisioningType = ProvisioningType.Patient;

            var model = new SelectServiceRecipientQuantityModel(orderType, organisation, item.CatalogueItem, item.OrderItemPrice, serviceRecipients, null);

            model.Title.Should().Be(string.Format(SelectServiceRecipientQuantityModel.TitleText, item.CatalogueItem.CatalogueItemType.Description()));
            model.Caption.Should().Be(item.CatalogueItem.Name);
            model.Advice.Should().Be(SelectServiceRecipientQuantityModel.AdviceTextMergerSplit);
            model.OrderType.Should().Be(orderType);
            model.PracticeReorganisationRecipient.Should().Be($"{organisation.Name} ({organisation.Id})");
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

            model.Title.Should().Be(string.Format(SelectServiceRecipientQuantityModel.TitleText, item.CatalogueItem.CatalogueItemType.Description()));
            model.Caption.Should().Be(item.CatalogueItem.Name);
            model.Advice.Should().Be(SelectServiceRecipientQuantityModel.AdviceTextServiceRecipient);
            model.OrderType.Should().BeNull();
            model.PracticeReorganisationRecipient.Should().BeNull();
            model.ServiceRecipients.Length.Should().Be(serviceRecipients.Count);

            foreach (var serviceRecipient in model.ServiceRecipients)
            {
                var recipient = serviceRecipients.First(x => x.OdsCode == serviceRecipient.OdsCode);

                serviceRecipient.Name.Should().Be(recipient.Name);
                serviceRecipient.InputQuantity.Should().Be("1");
                serviceRecipient.Quantity.Should().Be(1);
            }
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidOrderItem_ServiceRecipientsNull_EmptyServiceRecipients(
           OrderItem item)
        {
            item.OrderItemPrice.ProvisioningType = ProvisioningType.PerServiceRecipient;

            var model = new SelectServiceRecipientQuantityModel(item.CatalogueItem, item.OrderItemPrice, null);

            model.Title.Should().Be(string.Format(SelectServiceRecipientQuantityModel.TitleText, item.CatalogueItem.CatalogueItemType.Description()));
            model.Caption.Should().Be(item.CatalogueItem.Name);
            model.Advice.Should().Be(SelectServiceRecipientQuantityModel.AdviceTextServiceRecipient);
            model.OrderType.Should().BeNull();
            model.PracticeReorganisationRecipient.Should().BeNull();
            model.ServiceRecipients.Length.Should().Be(0);
        }
    }
}
