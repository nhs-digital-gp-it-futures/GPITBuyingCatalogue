using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.ServiceRecipients
{
    public static class SelectRecipientsModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Constructor_BasicPropertiesCorrectlySet(
            OrderItem orderItem,
            OrderItem previousItem,
            List<ServiceRecipientModel> serviceRecipients)
        {
            var itemName = previousItem.CatalogueItem.Name;
            var itemType = previousItem.CatalogueItem.CatalogueItemType;

            var model = new SelectRecipientsModel(orderItem, previousItem, serviceRecipients, null);

            model.ItemName.Should().Be(itemName);
            model.ItemType.Should().Be(itemType);

            model.Title.Should().Be(string.Format(SelectRecipientsModel.TitleText, itemType.Name()));
            model.Caption.Should().Be(itemName);
            model.Advice.Should().Be(string.Format(SelectRecipientsModel.AdviceText, itemType.Name()));

            model.PreviouslySelected.Should().BeEquivalentTo(previousItem.OrderItemRecipients.Select(x => x.Recipient?.Name));
            model.GetServiceRecipients().Should().BeEquivalentTo(serviceRecipients);
            model.GetServiceRecipients().ForEach(x => x.Selected.Should().BeFalse());
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_WithNoServiceRecipients_BasicPropertiesCorrectlySet(
            OrderItem orderItem,
            OrderItem previousItem)
        {
            var itemName = previousItem.CatalogueItem.Name;
            var itemType = previousItem.CatalogueItem.CatalogueItemType;

            var model = new SelectRecipientsModel(orderItem, previousItem, new List<ServiceRecipientModel>(), null);

            model.ItemName.Should().Be(itemName);
            model.ItemType.Should().Be(itemType);

            model.Title.Should().Be(string.Format(SelectRecipientsModel.TitleText, itemType.Name()));
            model.Caption.Should().Be(itemName);
            model.Advice.Should().Be(SelectRecipientsModel.AdviceTextNoRecipientsAvailable);

            model.PreviouslySelected.Should().BeEquivalentTo(previousItem.OrderItemRecipients.Select(x => x.Recipient?.Name));
            model.GetServiceRecipients().Should().BeEmpty();
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData(SelectionMode.None)]
        public static void WithSelectionModeNone_PropertiesCorrectlySet(
            SelectionMode? selectionMode,
            OrderItem orderItem,
            List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(orderItem, null, serviceRecipients, selectionMode);

            model.HasImportedRecipients.Should().BeFalse();
            model.ItemName.Should().Be(orderItem.CatalogueItem.Name);
            model.ItemType.Should().Be(orderItem.CatalogueItem.CatalogueItemType);
            model.GetServiceRecipients().ForEach(x => x.Selected.Should().BeFalse());
            model.SelectionMode.Should().Be(SelectionMode.All);
            model.SelectionCaption.Should().Be(SelectRecipientsModel.SelectAll);
        }

        [Theory]
        [CommonAutoData]
        public static void WithSelectionModeAll_PropertiesCorrectlySet(
            OrderItem orderItem,
            List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(orderItem, null, serviceRecipients, SelectionMode.All);

            model.HasImportedRecipients.Should().BeFalse();
            model.GetServiceRecipients().ForEach(x => x.Selected.Should().BeTrue());
            model.SelectionMode.Should().Be(SelectionMode.None);
            model.SelectionCaption.Should().Be(SelectRecipientsModel.SelectNone);
        }

        [Theory]
        [CommonAutoData]
        public static void WithNullSelectionMode_AndImportedRecipients_PropertiesCorrectlySet(
            OrderItem orderItem,
            List<ServiceRecipientModel> serviceRecipients)
        {
            for (var i = 0; i < 3; i++)
            {
                serviceRecipients[i].OdsCode = orderItem.OrderItemRecipients.ElementAt(i).OdsCode.ToUpperInvariant();
            }

            var importedRecipients = serviceRecipients.Select(x => x.OdsCode).ToArray();

            var model = new SelectRecipientsModel(orderItem, null, serviceRecipients, null, importedRecipients);

            model.HasImportedRecipients.Should().BeTrue();
            model.GetServiceRecipients().ForEach(x => x.Selected.Should().BeTrue());
            model.SelectionMode.Should().Be(SelectionMode.None);
            model.SelectionCaption.Should().Be(SelectRecipientsModel.SelectNone);
        }

        [Theory]
        [CommonAutoData]
        public static void PreSelectRecipients_WithNullSolution_PropertiesCorrectlySet(
            OrderItem orderItem,
            List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(orderItem, null, serviceRecipients, null);

            model.PreSelectRecipients(null);

            model.PreSelected.Should().BeFalse();
            model.GetServiceRecipients().ForEach(x => x.Selected.Should().BeFalse());
        }

        [Theory]
        [CommonAutoData]
        public static void PreSelectRecipients_WithSameOrderItem_PropertiesCorrectlySet(
            OrderItem orderItem,
            List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(orderItem, null, serviceRecipients, null);

            model.PreSelectRecipients(orderItem);

            model.PreSelected.Should().BeFalse();
            model.GetServiceRecipients().ForEach(x => x.Selected.Should().BeFalse());
        }

        [Theory]
        [CommonAutoData]
        public static void PreSelectSolutionServiceRecipients_NoMatchForServiceId_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.Order order,
            CatalogueItemId catalogueItemId,
            List<ServiceRecipientModel> serviceRecipients)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var model = new SelectRecipientsModel(order.OrderItems.ElementAt(1), null, serviceRecipients, null);

            model.GetServiceRecipients()[0].Selected.Should().BeFalse();
            model.GetServiceRecipients()[1].Selected.Should().BeFalse();
            model.GetServiceRecipients()[2].Selected.Should().BeFalse();

            model.PreSelectSolutionServiceRecipients(order, catalogueItemId);

            model.PreSelected.Should().BeFalse();
            model.GetServiceRecipients()[0].Selected.Should().BeFalse();
            model.GetServiceRecipients()[1].Selected.Should().BeFalse();
            model.GetServiceRecipients()[2].Selected.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void PreSelectSolutionServiceRecipients_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipientModel> serviceRecipients)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();
            var service = order.OrderItems.ElementAt(1);

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            serviceRecipients.First().OdsCode = solution.OrderItemRecipients.First().OdsCode;

            serviceRecipients.ForEach(x => x.Selected = false);

            var model = new SelectRecipientsModel(service, null, serviceRecipients, null);

            model.PreSelectSolutionServiceRecipients(order, service.CatalogueItemId);

            model.PreSelected.Should().BeTrue();
            model.GetServiceRecipients().Where(x => x.Selected).Should().HaveCount(1);
        }

        [Theory]
        [CommonAutoData]
        public static void SelectRecipientIds_PropertiesCorrectlySet(
            OrderItem orderItem,
            List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(orderItem, null, serviceRecipients, null);

            model.GetServiceRecipients()[0].Selected.Should().BeFalse();
            model.GetServiceRecipients()[1].Selected.Should().BeFalse();
            model.GetServiceRecipients()[2].Selected.Should().BeFalse();

            var recipientIds = string.Join(
                SelectRecipientsModel.Separator,
                serviceRecipients.Select(x => x.OdsCode));

            model.SelectRecipientIds(recipientIds);

            model.GetServiceRecipients()[0].Selected.Should().BeTrue();
            model.GetServiceRecipients()[1].Selected.Should().BeTrue();
            model.GetServiceRecipients()[2].Selected.Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void PreSelectRecipients_WithDifferentOrderItem_PropertiesCorrectlySet(
            OrderItem orderItem,
            OrderItem baseOrderItem,
            List<ServiceRecipientModel> serviceRecipients)
        {
            serviceRecipients.ForEach(x => x.Selected = false);

            var model = new SelectRecipientsModel(orderItem, null, serviceRecipients, null);

            baseOrderItem.OrderItemRecipients.First().OdsCode = serviceRecipients.First().OdsCode;

            model.PreSelectRecipients(baseOrderItem);

            model.PreSelected.Should().BeTrue();
            model.GetServiceRecipients().Where(x => x.Selected).Should().HaveCount(1);
        }

        [Theory]
        [CommonAutoData]
        public static void GetSelectedItems_NoSelectionMade_ExpectedResult(
            OrderItem orderItem,
            List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(orderItem, null, serviceRecipients, SelectionMode.None);

            var result = model.GetSelectedItems();

            result.Should().BeEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static void GetSelectedItems_SelectionMade_ExpectedResult(
            OrderItem orderItem,
            List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(orderItem, null, serviceRecipients, SelectionMode.All);

            var result = model.GetSelectedItems();

            result.Count.Should().Be(serviceRecipients.Count);

            serviceRecipients.ForEach(x =>
            {
                var dto = result.First(s => s.OdsCode == x.OdsCode);

                dto.Name.Should().Be(x.Name);
            });
        }
    }
}
