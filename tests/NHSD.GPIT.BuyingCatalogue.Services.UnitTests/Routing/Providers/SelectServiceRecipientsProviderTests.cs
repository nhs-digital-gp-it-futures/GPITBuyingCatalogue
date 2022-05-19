﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Routing.Providers
{
    public class SelectServiceRecipientsProviderTests
    {
        [Theory]
        [CommonAutoData]
        public void Process_OrderIsNull_ThrowsException(
            RouteValues routeValues,
            SelectServiceRecipientsProvider provider)
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
            SelectServiceRecipientsProvider provider)
        {
            FluentActions
                .Invoking(() => provider.Process(order, null))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("routeValues");

            var routeValues = new RouteValues
            {
                CatalogueItemId = null,
            };

            FluentActions
                .Invoking(() => provider.Process(order, routeValues))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("routeValues");
        }

        [Theory]
        [CommonAutoData]
        public void Process_OrderItemHasAnExistingPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            SelectServiceRecipientsProvider provider)
        {
            order.OrderItems.ToList().ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, solution.CatalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                solution.CatalogueItemId,
            };

            result.ActionName.Should().Be(Constants.Actions.EditPrice);
            result.ControllerName.Should().Be(Constants.Controllers.Prices);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_OrderItemHasNoExistingPriceAndMultiplePublishedPrices_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            SelectServiceRecipientsProvider provider)
        {
            order.OrderItems.ToList().ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            solution.OrderItemPrice = null;

            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, solution.CatalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                solution.CatalogueItemId,
            };

            result.ActionName.Should().Be(Constants.Actions.SelectPrice);
            result.ControllerName.Should().Be(Constants.Controllers.Prices);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_OrderItemHasNoExistingPriceAndOnePublishedPrice_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            SelectServiceRecipientsProvider provider)
        {
            order.OrderItems.ToList().ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            solution.CatalogueItem.CataloguePrices = new List<CataloguePrice>
            {
                solution.CatalogueItem.CataloguePrices.First(),
            };
            solution.OrderItemPrice = null;

            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, solution.CatalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                solution.CatalogueItemId,
                priceId = solution.CatalogueItem.CataloguePrices.First().CataloguePriceId,
            };

            result.ActionName.Should().Be(Constants.Actions.ConfirmPrice);
            result.ControllerName.Should().Be(Constants.Controllers.Prices);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }
    }
}
