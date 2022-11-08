using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.ServiceRecipients;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.ServiceRecipients
{
    public static class SelectRecipientsModelTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData(SelectionMode.None)]
        public static void WithSelectionModeNone_PropertiesCorrectlySet(
            SelectionMode? selectionMode,
            OrderItem orderItem,
            List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(orderItem, serviceRecipients, selectionMode);

            model.ItemName.Should().Be(orderItem.CatalogueItem.Name);
            model.ItemType.Should().Be(orderItem.CatalogueItem.CatalogueItemType);
            model.ServiceRecipients.ForEach(x => x.Selected.Should().BeFalse());
            model.SelectionMode.Should().Be(SelectionMode.All);
            model.SelectionCaption.Should().Be(SelectRecipientsModel.SelectAll);
        }

        [Theory]
        [CommonAutoData]
        public static void WithSelectionModeAll_PropertiesCorrectlySet(
            OrderItem orderItem,
            List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(orderItem, serviceRecipients, SelectionMode.All);

            model.ServiceRecipients.ForEach(x => x.Selected.Should().BeTrue());
            model.SelectionMode.Should().Be(SelectionMode.None);
            model.SelectionCaption.Should().Be(SelectRecipientsModel.SelectNone);
        }

        [Theory]
        [CommonAutoData]
        public static void PreSelectRecipients_WithNullSolution_PropertiesCorrectlySet(
            OrderItem orderItem,
            List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(orderItem, serviceRecipients, null);

            model.PreSelectRecipients(null);

            model.PreSelected.Should().BeFalse();
            model.ServiceRecipients.ForEach(x => x.Selected.Should().BeFalse());
        }

        [Theory]
        [CommonAutoData]
        public static void PreSelectRecipients_WithSameOrderItem_PropertiesCorrectlySet(
            OrderItem orderItem,
            List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(orderItem, serviceRecipients, null);

            model.PreSelectRecipients(orderItem);

            model.PreSelected.Should().BeFalse();
            model.ServiceRecipients.ForEach(x => x.Selected.Should().BeFalse());
        }

        [Theory]
        [CommonAutoData]
        public static void PreSelectRecipients_WithDifferentOrderItem_PropertiesCorrectlySet(
            OrderItem orderItem,
            OrderItem baseOrderItem,
            List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(orderItem, serviceRecipients, null);

            baseOrderItem.OrderItemRecipients.First().OdsCode = serviceRecipients.First().OdsCode;

            model.PreSelectRecipients(baseOrderItem);

            model.PreSelected.Should().BeTrue();
            model.ServiceRecipients[0].Selected.Should().BeTrue();
            model.ServiceRecipients[1].Selected.Should().BeFalse();
            model.ServiceRecipients[2].Selected.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void GetSelectedItems_NoSelectionMade_ExpectedResult(
            OrderItem orderItem,
            List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(orderItem, serviceRecipients, SelectionMode.None);

            var result = model.GetSelectedItems();

            result.Should().BeEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static void GetSelectedItems_SelectionMade_ExpectedResult(
            OrderItem orderItem,
            List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(orderItem, serviceRecipients, SelectionMode.All);

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
