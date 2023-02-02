using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Routing.Providers
{
    public class ConfirmServiceRecipientsBackLinkProviderTests
    {
        [Theory]
        [CommonAutoData]
        public void Process_RouteValuesIsNull_ThrowsException(
            Order order,
            ConfirmServiceRecipientsBackLinkProvider provider)
        {
            FluentActions
                .Invoking(() => provider.Process(order, null))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("routeValues");
        }

        [Theory]
        [CommonAutoData]
        public void Process_JourneyIsNull_ThrowsException(
            Order order,
            RouteValues routeValues,
            ConfirmServiceRecipientsBackLinkProvider provider)
        {
            routeValues.Journey = null;

            FluentActions
                .Invoking(() => provider.Process(order, null))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("routeValues");
        }

        [Theory]
        [CommonAutoData]
        public void Process_AddJourney_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            RoutingSource routingSource,
            string recipientIds,
            Order order,
            ConfirmServiceRecipientsBackLinkProvider provider)
        {
            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, catalogueItemId)
            {
                Journey = JourneyType.Add,
                Source = routingSource,
                RecipientIds = recipientIds,
            });

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = catalogueItemId,
                Source = routingSource,
                RecipientIds = recipientIds,
            };

            result.ActionName.Should().Be(Constants.Actions.AddServiceRecipients);
            result.ControllerName.Should().Be(Constants.Controllers.ServiceRecipients);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_EditJourney_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            RoutingSource routingSource,
            string recipientIds,
            Order order,
            ConfirmServiceRecipientsBackLinkProvider provider)
        {
            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, catalogueItemId)
            {
                Journey = JourneyType.Edit,
                Source = routingSource,
                RecipientIds = recipientIds,
            });

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = catalogueItemId,
                Source = routingSource,
                RecipientIds = recipientIds,
            };

            result.ActionName.Should().Be(Constants.Actions.EditServiceRecipients);
            result.ControllerName.Should().Be(Constants.Controllers.ServiceRecipients);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }
    }
}
