using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Routing.Providers
{
    public class SelectAssociatedServicesBackLinkProviderTests
    {
        [Theory]
        [CommonAutoData]
        public void Process_RouteValuesIsNull_ThrowsException(
            SelectAssociatedServicesBackLinkProvider provider)
        {
            FluentActions
                .Invoking(() => provider.Process(null, null))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("routeValues");
        }

        [Theory]
        [CommonAutoData]
        public void Process_FromAddAssociatedServices_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectAssociatedServicesBackLinkProvider provider)
        {
            var result = provider.Process(null, new RouteValues(internalOrgId, callOffId)
            {
                Source = RoutingSource.AddAssociatedServices,
            });

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                selected = true,
            };

            result.ActionName.Should().Be(Constants.Actions.AddAssociatedServices);
            result.ControllerName.Should().Be(Constants.Controllers.AssociatedServices);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_FromDashboard_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectAssociatedServicesBackLinkProvider provider)
        {
            var result = provider.Process(null, new RouteValues(internalOrgId, callOffId)
            {
                Source = RoutingSource.Dashboard,
            });

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            result.ActionName.Should().Be(Constants.Actions.OrderDashboard);
            result.ControllerName.Should().Be(Constants.Controllers.Orders);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_FromEditSolution_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectAssociatedServicesBackLinkProvider provider)
        {
            var result = provider.Process(null, new RouteValues(internalOrgId, callOffId)
            {
                Source = RoutingSource.EditSolution,
            });

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            result.ActionName.Should().Be(Constants.Actions.EditSolutionAssociatedServicesOnly);
            result.ControllerName.Should().Be(Constants.Controllers.CatalogueSolutions);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_FromSelectSolution_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectAssociatedServicesBackLinkProvider provider)
        {
            var result = provider.Process(null, new RouteValues(internalOrgId, callOffId)
            {
                Source = RoutingSource.SelectSolution,
            });

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                source = RoutingSource.SelectAssociatedServices,
            };

            result.ActionName.Should().Be(Constants.Actions.SelectSolutionAssociatedServicesOnly);
            result.ControllerName.Should().Be(Constants.Controllers.CatalogueSolutions);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_FromTaskList_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectAssociatedServicesBackLinkProvider provider)
        {
            var result = provider.Process(null, new RouteValues(internalOrgId, callOffId)
            {
                Source = RoutingSource.TaskList,
            });

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
