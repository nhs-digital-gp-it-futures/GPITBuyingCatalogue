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
using MoreLinq.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;
using Xunit;
using ServiceRecipient = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceRecipient;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection
{
    public static class ServiceRecipientsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(ServiceRecipientsController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(ServiceRecipientsController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ServiceRecipientsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData(SelectionMode.None)]
        [CommonInlineAutoData(SelectionMode.All)]
        public static async Task Get_AddServiceRecipients_ReturnsExpectedResult(
            SelectionMode? selectionMode,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOdsService> mockOdsService,
            ServiceRecipientsController controller)
        {
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockOdsService
                .Setup(x => x.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
                .ReturnsAsync(serviceRecipients);

            var result = await controller.AddServiceRecipients(internalOrgId, callOffId, solution.CatalogueItemId, selectionMode);

            mockOrderService.VerifyAll();
            mockOdsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var recipients = serviceRecipients
                .Select(x => new ServiceRecipientModel
                {
                    Name = x.Name,
                    OdsCode = x.OrgId,
                })
                .ToList();

            var expected = new SelectRecipientsModel(solution, null, recipients, selectionMode)
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = solution.CatalogueItemId,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddServiceRecipients_WithPreSelectedSolutionRecipients_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOdsService> mockOdsService,
            ServiceRecipientsController controller)
        {
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();
            var additionalService = order.OrderItems.ElementAt(1);

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            for (var i = 0; i < 3; i++)
            {
                solution.OrderItemRecipients.ElementAt(i).OdsCode = serviceRecipients[i].OrgId;
            }

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockOdsService
                .Setup(x => x.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
                .ReturnsAsync(serviceRecipients);

            var result = await controller.AddServiceRecipients(internalOrgId, callOffId, additionalService.CatalogueItemId);

            mockOrderService.VerifyAll();
            mockOdsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeAssignableTo<SelectRecipientsModel>().Subject;

            model.PreSelected.Should().BeTrue();
            model.ServiceRecipients.ForEach(x => x.Selected.Should().BeTrue());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddServiceRecipients_WithModelErrors_ReturnsExpectedResult(
            SelectRecipientsModel model,
            ServiceRecipientsController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.AddServiceRecipients(model.InternalOrgId, model.CallOffId, model.CatalogueItemId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddServiceRecipients_ReturnsExpectedResult(
            SelectRecipientsModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOrderItemRecipientService> mockOrderRecipientService,
            [Frozen] Mock<IRoutingService> mockRoutingService,
            ServiceRecipientsController controller)
        {
            IEnumerable<ServiceRecipientDto> serviceRecipients = null;

            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var orderItem = order.OrderItems.First().CatalogueItem;

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderWithCatalogueItemAndPrices(model.CallOffId, model.InternalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockOrderRecipientService
                .Setup(x => x.AddOrderItemRecipients(order.Id, orderItem.Id, It.IsAny<List<ServiceRecipientDto>>()))
                .Callback<int, CatalogueItemId, IEnumerable<ServiceRecipientDto>>((_, _, recipients) => serviceRecipients = recipients)
                .Returns(Task.CompletedTask);

            mockRoutingService
                .Setup(x => x.GetRoute(RoutingPoint.SelectServiceRecipients, order, It.IsAny<RouteValues>()))
                .Returns(Route(model.InternalOrgId, model.CallOffId, orderItem.Id));

            var result = await controller.AddServiceRecipients(model.InternalOrgId, model.CallOffId, orderItem.Id, model);

            mockOrderService.VerifyAll();
            mockOrderRecipientService.VerifyAll();
            mockRoutingService.VerifyAll();

            var expected = model.ServiceRecipients
                .Where(x => x.Selected)
                .Select(x => x.Dto);

            serviceRecipients.Should().BeEquivalentTo(expected);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(PricesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(PricesController.SelectPrice));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", model.InternalOrgId },
                { "callOffId", model.CallOffId },
                { "catalogueItemId", orderItem.Id },
            });
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData(SelectionMode.None)]
        [CommonInlineAutoData(SelectionMode.All)]
        public static async Task Get_EditServiceRecipients_ReturnsExpectedResult(
            SelectionMode? selectionMode,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOdsService> mockOdsService,
            ServiceRecipientsController controller)
        {
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockOdsService
                .Setup(x => x.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
                .ReturnsAsync(serviceRecipients);

            var result = await controller.EditServiceRecipients(internalOrgId, callOffId, solution.CatalogueItemId, selectionMode);

            mockOrderService.VerifyAll();
            mockOdsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var recipients = serviceRecipients
                .Select(x => new ServiceRecipientModel
                {
                    Name = x.Name,
                    OdsCode = x.OrgId,
                })
                .ToList();

            var expected = new SelectRecipientsModel(solution, null, recipients, selectionMode)
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = solution.CatalogueItemId,
                IsAdding = false,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditServiceRecipients_WithModelErrors_ReturnsExpectedResult(
            SelectRecipientsModel model,
            ServiceRecipientsController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.EditServiceRecipients(model.InternalOrgId, model.CallOffId, model.CatalogueItemId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditServiceRecipients_ReturnsExpectedResult(
            SelectRecipientsModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOrderItemRecipientService> mockOrderRecipientService,
            [Frozen] Mock<IRoutingService> mockRoutingService,
            ServiceRecipientsController controller)
        {
            IEnumerable<ServiceRecipientDto> serviceRecipients = null;

            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var orderItem = order.OrderItems.First().CatalogueItem;

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderWithCatalogueItemAndPrices(model.CallOffId, model.InternalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockOrderRecipientService
                .Setup(x => x.UpdateOrderItemRecipients(order.Id, orderItem.Id, It.IsAny<List<ServiceRecipientDto>>()))
                .Callback<int, CatalogueItemId, IEnumerable<ServiceRecipientDto>>((_, _, recipients) => serviceRecipients = recipients)
                .Returns(Task.CompletedTask);

            mockRoutingService
                .Setup(x => x.GetRoute(RoutingPoint.SelectServiceRecipients, order, It.IsAny<RouteValues>()))
                .Returns(Route(model.InternalOrgId, model.CallOffId, orderItem.Id));

            var result = await controller.EditServiceRecipients(model.InternalOrgId, model.CallOffId, orderItem.Id, model);

            mockOrderService.VerifyAll();
            mockOrderRecipientService.VerifyAll();
            mockRoutingService.VerifyAll();

            var expected = model.ServiceRecipients
                .Where(x => x.Selected)
                .Select(x => x.Dto);

            serviceRecipients.Should().BeEquivalentTo(expected);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(PricesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(PricesController.SelectPrice));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", model.InternalOrgId },
                { "callOffId", model.CallOffId },
                { "catalogueItemId", orderItem.Id },
            });
        }

        private static RoutingResult Route(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId) => new()
        {
            ActionName = Constants.Actions.SelectPrice,
            ControllerName = Constants.Controllers.Prices,
            RouteValues = new { internalOrgId, callOffId, catalogueItemId },
        };
    }
}
