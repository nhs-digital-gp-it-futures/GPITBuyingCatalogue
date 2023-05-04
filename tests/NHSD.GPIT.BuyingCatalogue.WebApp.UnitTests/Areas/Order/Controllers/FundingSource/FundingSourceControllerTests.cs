using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.FundingSource;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.FundingSource
{
    public static class FundingSourceControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(FundingSourceController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectFramework_OnlyOneFramework_Redirects(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Catalogue.Models.Framework framework,
            [Frozen] Mock<IOrderFrameworkService> orderFrameworkMock,
            [Frozen] Mock<IOrderService> orderServiceMock,
            FundingSourceController controller)
        {
            orderServiceMock
                .Setup(o => o.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            orderFrameworkMock.Setup(f => f.GetFrameworksForOrder(order.CallOffId, internalOrgId, order.AssociatedServicesOnly))
                .ReturnsAsync(new List<EntityFramework.Catalogue.Models.Framework>() { framework });

            var actual = await controller.SelectFramework(internalOrgId, order.CallOffId);

            var actualResult = actual.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(FundingSourceController.FundingSources));
            actualResult.ControllerName.Should().Be(typeof(FundingSourceController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
            });

            orderServiceMock.VerifyAll();
            orderFrameworkMock.VerifyAll();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectFramework_Multipleframeworks_Redirects(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<EntityFramework.Catalogue.Models.Framework> frameworks,
            [Frozen] Mock<IOrderFrameworkService> orderFrameworkMock,
            [Frozen] Mock<IOrderService> orderServiceMock,
            FundingSourceController controller)
        {
            orderServiceMock
                .Setup(o => o.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            orderFrameworkMock.Setup(f => f.GetFrameworksForOrder(order.CallOffId, internalOrgId, order.AssociatedServicesOnly))
                .ReturnsAsync(frameworks);

            var expectedModel = new SelectFrameworkModel(order, frameworks);

            var actual = await controller.SelectFramework(internalOrgId, order.CallOffId);

            orderServiceMock.VerifyAll();
            orderFrameworkMock.VerifyAll();

            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewData.Model
                .Should()
                .BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectFramework_NoExistingFramework_SetsAndRedirects(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<EntityFramework.Catalogue.Models.Framework> frameworks,
            [Frozen] Mock<IOrderFrameworkService> orderFrameworkMock,
            [Frozen] Mock<IOrderService> orderServiceMock,
            FundingSourceController controller)
        {
            order.SelectedFramework = null;
            order.SelectedFrameworkId = null;

            orderServiceMock
                .Setup(o => o.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var model = new SelectFrameworkModel(order, frameworks)
            {
                SelectedFramework = order.SelectedFrameworkId,
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

            orderServiceMock.VerifyAll();
            orderFrameworkMock.VerifyAll();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectFramework_FrameworkExists_RedirectsOnly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<EntityFramework.Catalogue.Models.Framework> frameworks,
            [Frozen] Mock<IOrderFrameworkService> orderFrameworkMock,
            [Frozen] Mock<IOrderService> orderServiceMock,
            FundingSourceController controller)
        {
            orderServiceMock
                .Setup(o => o.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var model = new SelectFrameworkModel(order, frameworks)
            {
                SelectedFramework = frameworks.FirstOrDefault().Id,
            };

            var actual = await controller.SelectFramework(model, internalOrgId, order.CallOffId);

            var actualResult = actual.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(FundingSourceController.ConfirmFrameworkChange));
            actualResult.ControllerName.Should().Be(typeof(FundingSourceController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
                { "selectedFrameworkId", frameworks.FirstOrDefault().Id },
            });

            orderServiceMock.VerifyAll();
            orderFrameworkMock.Verify(s => s.SetSelectedFrameworkForOrder(order.CallOffId, internalOrgId, order.SelectedFrameworkId), Times.Never());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectFramework_FrameworkExists_SelectedSameFramework_RedirectsOnly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<EntityFramework.Catalogue.Models.Framework> frameworks,
            [Frozen] Mock<IOrderFrameworkService> orderFrameworkMock,
            [Frozen] Mock<IOrderService> orderServiceMock,
            FundingSourceController controller)
        {
            orderServiceMock
                .Setup(o => o.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var model = new SelectFrameworkModel(order, frameworks)
            {
                SelectedFramework = order.SelectedFrameworkId,
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

            orderServiceMock.VerifyAll();
            orderFrameworkMock.Verify(s => s.SetSelectedFrameworkForOrder(order.CallOffId, internalOrgId, order.SelectedFrameworkId), Times.Never());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectFramework_ModelError_ReturnsView(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<EntityFramework.Catalogue.Models.Framework> frameworks,
            [Frozen] Mock<IOrderFrameworkService> orderFrameworkMock,
            [Frozen] Mock<IOrderService> orderServiceMock,
            FundingSourceController controller)
        {
            orderFrameworkMock.Setup(f => f.GetFrameworksForOrder(order.CallOffId, internalOrgId, order.AssociatedServicesOnly))
                .ReturnsAsync(frameworks);

            controller.ModelState.AddModelError("test", "test");

            var model = new SelectFrameworkModel(order, frameworks)
            {
                SelectedFramework = null,
            };

            var actual = await controller.SelectFramework(model, internalOrgId, order.CallOffId);

            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewData.ModelState.ValidationState.Should().Be(ModelValidationState.Invalid);

            orderServiceMock.VerifyAll();
            orderFrameworkMock.VerifyAll();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConfirmFrameworkChange_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Catalogue.Models.Framework framework,
            [Frozen] Mock<IFrameworkService> frameworkMock,
            [Frozen] Mock<IOrderService> orderServiceMock,
            FundingSourceController controller)
        {
            orderServiceMock
                .Setup(o => o.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            frameworkMock.Setup(f => f.GetFrameworksById(framework.Id))
                .ReturnsAsync(framework);

            var expectedModel = new ConfirmFrameworkChangeModel(order, framework);

            var actual = await controller.ConfirmFrameworkChange(internalOrgId, order.CallOffId, framework.Id);

            orderServiceMock.VerifyAll();
            frameworkMock.VerifyAll();

            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewData.Model
                .Should()
                .BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmFrameworkChange_ConfirmChangesSelectedNo_Redirects(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Catalogue.Models.Framework framework,
            [Frozen] Mock<IOrderFrameworkService> orderFrameworkMock,
            FundingSourceController controller)
        {
            var model = new ConfirmFrameworkChangeModel(order, framework)
            {
                ConfirmChanges = false,
            };

            var actual = await controller.ConfirmFrameworkChange(model, internalOrgId, order.CallOffId, framework.Id);

            var actualResult = actual.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(FundingSourceController.SelectFramework));
            actualResult.ControllerName.Should().Be(typeof(FundingSourceController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
            });

            orderFrameworkMock.Verify(o => o.UpdateFundingSourceAndSetSelectedFrameworkForOrder(order.CallOffId, internalOrgId, framework.Id), Times.Never());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmFrameworkChange_ConfirmChangesSelectedYes_ClearsDownAndRedirects(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Catalogue.Models.Framework framework,
            [Frozen] Mock<IOrderFrameworkService> orderFrameworkMock,
            FundingSourceController controller)
        {
            var model = new ConfirmFrameworkChangeModel(order, framework)
            {
                ConfirmChanges = true,
            };

            var actual = await controller.ConfirmFrameworkChange(model, internalOrgId, order.CallOffId, framework.Id);

            var actualResult = actual.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(FundingSourceController.FundingSources));
            actualResult.ControllerName.Should().Be(typeof(FundingSourceController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
            });

            orderFrameworkMock.VerifyAll();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_FundingSources_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            [Frozen] Mock<IOrderFrameworkService> frameworkServiceMock,
            EntityFramework.Catalogue.Models.Framework framework,
            FundingSourceController controller)
        {
            orderServiceMock
                .Setup(o => o.GetOrderWithOrderItemsForFunding(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            frameworkServiceMock.Setup(f => f.GetFrameworksForOrder(order.CallOffId, internalOrgId, order.AssociatedServicesOnly))
                .ReturnsAsync(new List<EntityFramework.Catalogue.Models.Framework>() { framework });

            var expectedViewData = new FundingSources(internalOrgId, order.CallOffId, order, 1);

            var actual = await controller.FundingSources(internalOrgId, order.CallOffId);

            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_FundingSource_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] OrderItem orderItem,
            [Frozen] Mock<IOrderItemService> orderItemServiceMock,
            [Frozen] Mock<IOrderService> orderService,
            FundingSourceController controller)
        {
            orderItemServiceMock.Setup(oi => oi.GetOrderItem(order.CallOffId, internalOrgId, orderItem.CatalogueItemId))
                .ReturnsAsync(orderItem);

            orderService
                .Setup(o => o.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var expectedViewData = new WebApp.Areas.Orders.Models.FundingSources.FundingSource(internalOrgId, order.CallOffId, order, orderItem);

            var actual = await controller.FundingSource(internalOrgId, order.CallOffId, orderItem.CatalogueItemId);

            orderItemServiceMock.VerifyAll();

            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_FundingSource_ReturnsExpectedRedirect(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] OrderItem orderItem,
            [Frozen] Mock<IOrderItemService> orderItemServiceMock,
            FundingSourceController controller)
        {
            var model = new WebApp.Areas.Orders.Models.FundingSources.FundingSource(internalOrgId, order.CallOffId, order, orderItem)
            {
                SelectedFundingType = OrderItemFundingType.CentralFunding,
            };

            orderItemServiceMock.Setup(oi => oi.UpdateOrderItemFunding(order.CallOffId, internalOrgId, orderItem.CatalogueItemId, model.SelectedFundingType))
                .Verifiable();

            var actual = await controller.FundingSource(internalOrgId, order.CallOffId, orderItem.CatalogueItemId, model);

            orderItemServiceMock.VerifyAll();

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
        [CommonAutoData]
        public static async Task Post_FundingSource_ModelError_ReturnsView(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] OrderItem orderItem,
            [Frozen] Mock<IOrderItemService> orderItemServiceMock,
            FundingSourceController controller)
        {
            var model = new WebApp.Areas.Orders.Models.FundingSources.FundingSource(internalOrgId, order.CallOffId, order, orderItem);

            controller.ModelState.AddModelError("test", "test");

            var actual = await controller.FundingSource(internalOrgId, order.CallOffId, orderItem.CatalogueItemId, model);

            orderItemServiceMock.VerifyAll();

            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewData.ModelState.ValidationState.Should().Be(ModelValidationState.Invalid);
        }
    }
}
