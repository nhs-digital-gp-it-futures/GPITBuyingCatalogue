using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection
{
    public static class AdditionalServicesControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AdditionalServicesController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(AdditionalServicesController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AdditionalServicesController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAdditionalServices_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IAdditionalServicesService mockAdditionalServicesService,
            AdditionalServicesController controller)
        {
            var orderWrapper = new OrderWrapper(order);
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var solutionId = order.OrderItems.First().CatalogueItemId;

            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(orderWrapper);

            mockAdditionalServicesService.GetAdditionalServicesBySolutionId(solutionId, true).Returns(services);

            var result = await controller.SelectAdditionalServices(internalOrgId, order.CallOffId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var previousItems = orderWrapper.Previous?.GetAssociatedServices().Select(x => x.CatalogueItem)
                ?? Enumerable.Empty<CatalogueItem>();
            var currentItems = orderWrapper.Order?.GetAssociatedServices().Select(x => x.CatalogueItem)
                ?? Enumerable.Empty<CatalogueItem>();

            var expected = new SelectServicesModel(previousItems, currentItems, services)
            {
                InternalOrgId = internalOrgId,
                AssociatedServicesOnly = order.OrderType.AssociatedServicesOnly,
                SolutionName = order.OrderType.GetSolutionNameFromOrder(order),
                SolutionId = solutionId,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditAdditionalServices_NoChangesMade_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectServicesModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            AdditionalServicesController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            for (var i = 0; i < order.OrderItems.Count; i++)
            {
                model.Services[i].CatalogueItemId = order.OrderItems.ElementAt(i).CatalogueItemId;
                model.Services[i].IsSelected = true;
            }

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.SelectAdditionalServices(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(TaskListController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(TaskListController.TaskList));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [MockInlineAutoData(1)]
        [MockInlineAutoData(2)]
        public static async Task Post_EditAdditionalServices_ServicesAdded_ReturnsExpectedResult(
            int revision,
            string internalOrgId,
            CallOffId callOffId,
            SelectServicesModel model,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItemId newServiceId,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IOrderItemService mockOrderItemService,
            AdditionalServicesController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, revision);
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            for (var i = 0; i < order.OrderItems.Count; i++)
            {
                model.Services[i].CatalogueItemId = order.OrderItems.ElementAt(i).CatalogueItemId;
                model.Services[i].IsSelected = true;
            }

            model.Services.Add(new ServiceModel
            {
                CatalogueItemId = newServiceId,
                IsSelected = true,
            });

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            IEnumerable<CatalogueItemId> newServiceIds = null;

            mockOrderItemService
                .When(x => x.AddOrderItems(internalOrgId, callOffId, Arg.Any<IEnumerable<CatalogueItemId>>()))
                .Do(x => newServiceIds = x.Arg<IEnumerable<CatalogueItemId>>());

            var result = await controller.SelectAdditionalServices(internalOrgId, callOffId, model);

            newServiceIds.Should().BeEquivalentTo(new[] { newServiceId });

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(TaskListController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(TaskListController.TaskList));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }
    }
}
