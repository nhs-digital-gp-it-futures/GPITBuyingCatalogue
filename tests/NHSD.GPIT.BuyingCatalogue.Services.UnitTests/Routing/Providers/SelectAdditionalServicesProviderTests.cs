using System;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Routing.Providers
{
    public class SelectAdditionalServicesProviderTests
    {
        [Theory]
        [CommonAutoData]
        public void Process_OrderIsNull_ThrowsException(
            RouteValues routeValues,
            SelectAdditionalServicesProvider provider)
        {
            FluentActions
                .Invoking(() => provider.Process(null, routeValues))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("order");
        }

        [Theory]
        [CommonAutoData]
        public void Process_RouteValuesIsNull_ThrowsException(
            Order order,
            SelectAdditionalServicesProvider provider)
        {
            FluentActions
                .Invoking(() => provider.Process(order, null))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("routeValues");
        }

        [Theory]
        [CommonAutoData]
        public void Process_NoSolutionRecipients_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            SelectAdditionalServicesProvider provider)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            solution.OrderItemRecipients.Clear();

            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                solution.CatalogueItemId,
            };

            result.ActionName.Should().Be(Constants.Actions.AddServiceRecipients);
            result.ControllerName.Should().Be(Constants.Controllers.ServiceRecipients);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_NoAdditionalServiceRecipients_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            SelectAdditionalServicesProvider provider)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var service = order.OrderItems.ElementAt(1);

            service.OrderItemRecipients.Clear();

            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                service.CatalogueItemId,
            };

            result.ActionName.Should().Be(Constants.Actions.AddServiceRecipients);
            result.ControllerName.Should().Be(Constants.Controllers.ServiceRecipients);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            SelectAdditionalServicesProvider provider)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            result.ActionName.Should().Be(Constants.Actions.TaskList);
            result.ControllerName.Should().Be(Constants.Controllers.TaskList);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }
    }
}
