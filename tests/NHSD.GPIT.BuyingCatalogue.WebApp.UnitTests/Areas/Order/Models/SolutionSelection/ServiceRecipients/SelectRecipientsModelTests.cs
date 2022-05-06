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
            List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(serviceRecipients, selectionMode);

            model.ServiceRecipients.ForEach(x => x.Selected.Should().BeFalse());
            model.SelectionMode.Should().Be(SelectionMode.All);
            model.SelectionCaption.Should().Be(SelectRecipientsModel.SelectAll);
        }

        [Theory]
        [CommonAutoData]
        public static void WithSelectionModeAll_PropertiesCorrectlySet(List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(serviceRecipients, SelectionMode.All);

            model.ServiceRecipients.ForEach(x => x.Selected.Should().BeTrue());
            model.SelectionMode.Should().Be(SelectionMode.None);
            model.SelectionCaption.Should().Be(SelectRecipientsModel.SelectNone);
        }

        [Theory]
        [CommonAutoData]
        public static void PreSelectRecipients_WithNullSolution_PropertiesCorrectlySet(
            List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(serviceRecipients, null);

            model.PreSelectRecipients(null);

            model.PreSelected.Should().BeFalse();
            model.ServiceRecipients.ForEach(x => x.Selected.Should().BeFalse());
        }

        [Theory]
        [CommonAutoData]
        public static void PreSelectRecipients_PropertiesCorrectlySet(
            OrderItem solution,
            List<ServiceRecipientModel> serviceRecipients)
        {
            var model = new SelectRecipientsModel(serviceRecipients, null);

            solution.OrderItemRecipients.First().OdsCode = serviceRecipients.First().OdsCode;

            model.PreSelectRecipients(solution);

            model.PreSelected.Should().BeTrue();
            model.ServiceRecipients[0].Selected.Should().BeTrue();
            model.ServiceRecipients[1].Selected.Should().BeFalse();
            model.ServiceRecipients[2].Selected.Should().BeFalse();
        }
    }
}
