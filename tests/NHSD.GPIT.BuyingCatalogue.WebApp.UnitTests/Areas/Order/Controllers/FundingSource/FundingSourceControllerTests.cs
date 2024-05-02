using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.FundingSource;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.FundingSource
{
    public static class FundingSourceControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(FundingSourceController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SelectFramework_OnlyOneFramework_Redirects(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Catalogue.Models.Framework framework,
            [Frozen] IOrderFrameworkService orderFrameworkMock,
            [Frozen] IOrderService orderServiceMock,
            FundingSourceController controller)
        {
            orderServiceMock
                .GetOrderThin(order.CallOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            orderFrameworkMock
                .GetFrameworksForOrder(order.CallOffId, internalOrgId, order.OrderType.AssociatedServicesOnly)
                .Returns(new List<EntityFramework.Catalogue.Models.Framework>() { framework });

            var actual = await controller.SelectFramework(internalOrgId, order.CallOffId);

            var actualResult = actual.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(FundingSourceController.FundingSources));
            actualResult.ControllerName.Should().Be(typeof(FundingSourceController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SelectFramework_Multipleframeworks_Redirects(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<EntityFramework.Catalogue.Models.Framework> frameworks,
            [Frozen] IOrderFrameworkService orderFrameworkMock,
            [Frozen] IOrderService orderServiceMock,
            FundingSourceController controller)
        {
            orderServiceMock
                .GetOrderThin(order.CallOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            orderFrameworkMock
                .GetFrameworksForOrder(order.CallOffId, internalOrgId, order.OrderType.AssociatedServicesOnly)
                .Returns(frameworks);

            var expectedModel = new SelectFrameworkModel(order, frameworks);

            var actual = await controller.SelectFramework(internalOrgId, order.CallOffId);

            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewData.Model
                .Should()
                .BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.BackLinkText));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectFramework_NoExistingFramework_SetsAndRedirects(
            string internalOrgId,
            string selectedFrameworkId,
            EntityFramework.Ordering.Models.Order order,
            List<EntityFramework.Catalogue.Models.Framework> frameworks,
            [Frozen] IOrderFrameworkService orderFrameworkMock,
            [Frozen] IOrderService orderServiceMock,
            FundingSourceController controller)
        {
            order.SelectedFramework = null;
            order.SelectedFrameworkId = null;

            orderServiceMock
                .GetOrderThin(order.CallOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            var model = new SelectFrameworkModel(order, frameworks)
            {
                SelectedFramework = selectedFrameworkId,
            };

            var actual = await controller.SelectFramework(model, internalOrgId, order.CallOffId);

            var actualResult = actual.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(FundingSourceController.FundingSources));
            actualResult.ControllerName.Should().Be(typeof(FundingSourceController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
            });

            await orderFrameworkMock
                .Received()
                .SetSelectedFrameworkForOrder(order.CallOffId, internalOrgId, model.SelectedFramework);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectFramework_ModelError_ReturnsView(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<EntityFramework.Catalogue.Models.Framework> frameworks,
            [Frozen] IOrderFrameworkService orderFrameworkMock,
            FundingSourceController controller)
        {
            orderFrameworkMock
                .GetFrameworksForOrder(order.CallOffId, internalOrgId, order.OrderType.AssociatedServicesOnly)
                .Returns(frameworks);

            controller.ModelState.AddModelError("test", "test");

            var model = new SelectFrameworkModel(order, frameworks)
            {
                SelectedFramework = null,
            };

            var actual = await controller.SelectFramework(model, internalOrgId, order.CallOffId);

            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewData.ModelState.ValidationState.Should().Be(ModelValidationState.Invalid);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_FundingSources_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderServiceMock,
            [Frozen] IOrderFrameworkService frameworkServiceMock,
            EntityFramework.Catalogue.Models.Framework framework,
            FundingSourceController controller)
        {
            var orderWrapper = new OrderWrapper(order);
            orderServiceMock
                .GetOrderWithOrderItemsForFunding(order.CallOffId, internalOrgId)
                .Returns(orderWrapper);

            frameworkServiceMock
                .GetFrameworksForOrder(order.CallOffId, internalOrgId, order.OrderType.AssociatedServicesOnly)
                .Returns(new List<EntityFramework.Catalogue.Models.Framework>() { framework });

            var expectedViewData = new FundingSources(internalOrgId, order.CallOffId, orderWrapper, 1);

            var actual = await controller.FundingSources(internalOrgId, order.CallOffId);

            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.BackLinkText));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_FundingSource_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] OrderItem orderItem,
            [Frozen] IOrderItemService orderItemServiceMock,
            [Frozen] IOrderService orderService,
            FundingSourceController controller)
        {
            orderItemServiceMock
                .GetOrderItem(order.CallOffId, internalOrgId, orderItem.CatalogueItemId)
                .Returns(orderItem);

            var orderWrapper = new OrderWrapper(order);
            orderService
                .GetOrderWithOrderItemsForFunding(order.CallOffId, internalOrgId)
                .Returns(orderWrapper);

            var expectedViewData = new WebApp.Areas.Orders.Models.FundingSources.FundingSource(internalOrgId, order.CallOffId, orderWrapper, orderItem);

            var actual = await controller.FundingSource(internalOrgId, order.CallOffId, orderItem.CatalogueItemId);

            await orderItemServiceMock
                .Received()
                .GetOrderItem(order.CallOffId, internalOrgId, orderItem.CatalogueItemId);

            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.BackLinkText));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_FundingSource_ReturnsExpectedRedirect(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] OrderItem orderItem,
            [Frozen] IOrderItemService orderItemServiceMock,
            FundingSourceController controller)
        {
            var orderWrapper = new OrderWrapper(order);
            var model = new WebApp.Areas.Orders.Models.FundingSources.FundingSource(internalOrgId, order.CallOffId, orderWrapper, orderItem)
            {
                SelectedFundingType = OrderItemFundingType.CentralFunding,
            };

            var actual = await controller.FundingSource(internalOrgId, order.CallOffId, orderItem.CatalogueItemId, model);

            await orderItemServiceMock
                .Received()
                .UpdateOrderItemFunding(order.CallOffId, internalOrgId, orderItem.CatalogueItemId, model.SelectedFundingType);

            var actualResult = actual.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(FundingSourceController.FundingSources));
            actualResult.ControllerName.Should().Be(typeof(FundingSourceController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_FundingSource_ModelError_ReturnsView(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] OrderItem orderItem,
            [Frozen] IOrderService orderServiceMock,
            FundingSourceController controller)
        {
            var orderWrapper = new OrderWrapper(order);
            var model = new WebApp.Areas.Orders.Models.FundingSources.FundingSource(internalOrgId, order.CallOffId, orderWrapper, orderItem);

            orderServiceMock
                .GetOrderWithOrderItemsForFunding(order.CallOffId, internalOrgId)
                .Returns(orderWrapper);

            controller.ModelState.AddModelError("test", "test");

            var actual = await controller.FundingSource(internalOrgId, order.CallOffId, orderItem.CatalogueItemId, model);

            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewData.ModelState.ValidationState.Should().Be(ModelValidationState.Invalid);
        }
    }
}
