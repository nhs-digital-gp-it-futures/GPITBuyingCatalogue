using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.CatalogueSolutions
{
    public static class SelectSolutionServiceRecipientsModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments__NewOrder_PropertiesCorrectlySet(
            string odsCode,
            CallOffId callOffId,
            string solutionName,
            IEnumerable<OrderItemRecipientModel> serviceRecipients,
            string selectionMode,
            CatalogueItemId catalogueSolutionId
            )
        {
            var model = new SelectSolutionServiceRecipientsModel(odsCode, callOffId, solutionName, serviceRecipients, selectionMode, true, catalogueSolutionId);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Service Recipients for {solutionName} for {callOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.CallOffId.Should().Be(callOffId);

            // TODO: ServiceRecipients
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments__ExistingOrder_PropertiesCorrectlySet(
            string odsCode,
            CallOffId callOffId,
            string solutionName,
            IEnumerable<OrderItemRecipientModel> serviceRecipients,
            string selectionMode,
            CatalogueItemId catalogueSolutionId
        )
        {
            var model = new SelectSolutionServiceRecipientsModel(odsCode, callOffId, solutionName, serviceRecipients, selectionMode, false, catalogueSolutionId);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/{catalogueSolutionId}");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Service Recipients for {solutionName} for {callOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.CallOffId.Should().Be(callOffId);

            // TODO: ServiceRecipients
        }
    }
}
