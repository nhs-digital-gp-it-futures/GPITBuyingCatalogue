using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Routing.Providers
{
    public class EditPriceProviderTests
    {
        [Theory]
        [CommonAutoData]
        public void Process_RouteValuesIsNull_ThrowsException(
            Order order,
            EditPriceProvider provider)
        {
            FluentActions
                .Invoking(() => provider.Process(order, null))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public void Process_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            EditPriceProvider provider)
        {
            var result = provider.Process(null, new RouteValues(internalOrgId, callOffId, catalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = catalogueItemId,
            };

            result.ActionName.Should().Be(Constants.Actions.SelectQuantity);
            result.ControllerName.Should().Be(Constants.Controllers.Quantity);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_FromTaskList_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            EditPriceProvider provider)
        {
            var result = provider.Process(null, new RouteValues(internalOrgId, callOffId, catalogueItemId)
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
