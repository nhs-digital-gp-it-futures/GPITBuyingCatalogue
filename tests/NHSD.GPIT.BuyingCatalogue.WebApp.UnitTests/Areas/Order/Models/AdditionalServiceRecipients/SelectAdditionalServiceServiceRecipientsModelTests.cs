using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServiceRecipients;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AdditionalServiceRecipients
{
    public static class SelectAdditionalServiceRecipientsModelTests
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
            var model = new SelectAdditionalServiceRecipientsModel(odsCode, callOffId, solutionName, serviceRecipients, selectionMode, true, catalogueSolutionId);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{callOffId}/additional-services/select/additional-service");
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
            var model = new SelectAdditionalServiceRecipientsModel(odsCode, callOffId, solutionName, serviceRecipients, selectionMode, false, catalogueSolutionId);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{callOffId}/additional-services/{catalogueSolutionId}");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Service Recipients for {solutionName} for {callOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.CallOffId.Should().Be(callOffId);

            // TODO: ServiceRecipients
        }
    }
}
