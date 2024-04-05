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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.Contracts;
using NHSD.GPIT.BuyingCatalogue.Services.Routing;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.Contracts
{
    public static class DeliveryDatesControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(DeliveryDatesController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(DeliveryDatesController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(DeliveryDatesController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData(true)]
        [MockInlineAutoData(false)]
        public static async Task Get_SelectDate_ReturnsExpectedResult(
            bool? setAllPDD,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderService,
            DeliveryDatesController controller)
        {
            orderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.SelectDate(internalOrgId, callOffId, setAllPDD: setAllPDD);

            await orderService.Received().GetOrderThin(callOffId, internalOrgId);

            var expected = new SelectDateModel(internalOrgId, callOffId, order, setAllPDD);
            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectDate_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectDateModel model,
            DeliveryDatesController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.SelectDate(internalOrgId, callOffId, model);
            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(model, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectDate_OrderDeliveryDateIsNull_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectDateModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IDeliveryDateService deliveryDatesService,
            [Frozen] IOrderService orderService,
            DeliveryDatesController controller)
        {
            order.SetupCatalogueSolution();
            order.DeliveryDate = null;

            orderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.SelectDate(internalOrgId, callOffId, model);

            await deliveryDatesService.Received().SetDeliveryDate(internalOrgId, callOffId, model.Date.Value);
            await orderService.Received().GetOrderThin(callOffId, internalOrgId);

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(DeliveryDatesController.EditDates));
            actual.ControllerName.Should().Be(typeof(DeliveryDatesController).ControllerName());
            actual.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary
                    {
                        { "internalOrgId", internalOrgId },
                        { "callOffId", callOffId },
                        { "catalogueItemId", order.OrderItems.First().CatalogueItemId },
                    });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectDate_OrderDeliveryDateIsNull_ApplyToAll_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectDateModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IDeliveryDateService deliveryDatesService,
            [Frozen] IOrderService orderService,
            DeliveryDatesController controller)
        {
            order.SetupCatalogueSolution();
            order.DeliveryDate = null;
            model.ApplyToAll = true;

            orderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.SelectDate(internalOrgId, callOffId, model);

            await deliveryDatesService.Received().SetAllDeliveryDates(internalOrgId, callOffId, model.Date.Value);
            await orderService.Received().GetOrderThin(callOffId, internalOrgId);

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(DeliveryDatesController.Review));
            actual.ControllerName.Should().Be(typeof(DeliveryDatesController).ControllerName());
            actual.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId }, });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectDate_NewDateSameAsOldDate_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectDateModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IDeliveryDateService deliveryDatesService,
            [Frozen] IOrderService orderService,
            DeliveryDatesController controller)
        {
            order.SetupCatalogueSolution();
            order.DeliveryDate = model.Date;

            orderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.SelectDate(internalOrgId, callOffId, model);

            await orderService.Received().GetOrderThin(callOffId, internalOrgId);
            await deliveryDatesService.Received().ResetRecipientDeliveryDates(order.Id);

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(DeliveryDatesController.EditDates));
            actual.ControllerName.Should().Be(typeof(DeliveryDatesController).ControllerName());
            actual.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary
                    {
                        { "InternalOrgId", internalOrgId },
                        { "callOffId", callOffId },
                        { "catalogueItemId", order.OrderItems.First().CatalogueItemId },
                    });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectDate_NewDateSameAsOldDate_ApplyToAll_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectDateModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IDeliveryDateService deliveryDatesService,
            [Frozen] IOrderService orderService,
            DeliveryDatesController controller)
        {
            order.SetupCatalogueSolution();
            order.DeliveryDate = model.Date;
            model.ApplyToAll = true;

            orderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.SelectDate(internalOrgId, callOffId, model);

            await orderService.Received().GetOrderThin(callOffId, internalOrgId);
            await deliveryDatesService.Received().SetAllDeliveryDates(internalOrgId, callOffId, model.Date.Value);

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(DeliveryDatesController.Review));
            actual.ControllerName.Should().Be(typeof(DeliveryDatesController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "InternalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [MockInlineAutoData(true)]
        [MockInlineAutoData(false)]
        public static async Task Post_SelectDate_NewDateDifferentFromOldDate_ReturnsExpectedResult(
            bool applyToAll,
            string internalOrgId,
            CallOffId callOffId,
            SelectDateModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderService,
            DeliveryDatesController controller)
        {
            model.ApplyToAll = applyToAll;
            order.SetupCatalogueSolution();
            order.DeliveryDate = model.Date!.Value.AddDays(1);

            orderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.SelectDate(internalOrgId, callOffId, model);

            await orderService.Received().GetOrderThin(callOffId, internalOrgId);

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(DeliveryDatesController.ConfirmChanges));
            actual.ControllerName.Should().Be(typeof(DeliveryDatesController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "deliveryDate", model.Date.Value.ToString(DeliveryDatesController.DateFormat) },
                { "applyToAll", model.ApplyToAll },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ConfirmChanges_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            bool applyToAll,
            [Frozen] IOrderService orderService,
            DeliveryDatesController controller)
        {
            orderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var newDate = order.DeliveryDate!.Value.Date.AddDays(1);

            var result = await controller.ConfirmChanges(
                internalOrgId,
                callOffId,
                newDate.ToString(DeliveryDatesController.DateFormat),
                applyToAll);

            await orderService.Received().GetOrderThin(callOffId, internalOrgId);

            var expected = new ConfirmChangesModel(internalOrgId, callOffId, order.DeliveryDate.Value, newDate, applyToAll);
            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ConfirmChanges_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmChangesModel model,
            DeliveryDatesController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.ConfirmChanges(internalOrgId, callOffId, model);
            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(model, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockInlineAutoData(true)]
        [MockInlineAutoData(false)]
        public static async Task Post_ConfirmChanges_NoSelected_ReturnsExpectedResult(
            bool applyToAll,
            string internalOrgId,
            CallOffId callOffId,
            ConfirmChangesModel model,
            DeliveryDatesController controller)
        {
            model.ConfirmChanges = false;
            model.ApplyToAll = applyToAll;

            var result = await controller.ConfirmChanges(internalOrgId, callOffId, model);
            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(DeliveryDatesController.Review));
            actual.ControllerName.Should().Be(typeof(DeliveryDatesController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ConfirmChanges_YesSelected_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmChangesModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IDeliveryDateService deliveryDatesService,
            [Frozen] IOrderService orderService,
            DeliveryDatesController controller)
        {
            order.SetupCatalogueSolution();
            model.ConfirmChanges = true;

            orderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.ConfirmChanges(internalOrgId, callOffId, model);

            await deliveryDatesService.Received().SetDeliveryDate(internalOrgId, callOffId, model.NewDeliveryDate);
            await deliveryDatesService.Received().ResetRecipientDeliveryDates(order.Id);
            await orderService.Received().GetOrderThin(callOffId, internalOrgId);

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(DeliveryDatesController.EditDates));
            actual.ControllerName.Should().Be(typeof(DeliveryDatesController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", model.InternalOrgId },
                { "callOffId", model.CallOffId },
                { "catalogueItemId", order.OrderItems.First().CatalogueItemId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ConfirmChanges_YesSelected_ApplyToAll_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmChangesModel model,
            [Frozen] IDeliveryDateService deliveryDatesService,
            DeliveryDatesController controller)
        {
            model.ConfirmChanges = true;
            model.ApplyToAll = true;

            var result = await controller.ConfirmChanges(internalOrgId, callOffId, model);

            await deliveryDatesService.Received().SetAllDeliveryDates(internalOrgId, callOffId, model.NewDeliveryDate);

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(DeliveryDatesController.Review));
            actual.ControllerName.Should().Be(typeof(DeliveryDatesController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditDates_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderService,
            [Frozen] IRoutingService routingService,
            DeliveryDatesController controller)
        {
            order.SetupCatalogueSolution();

            var catalogueItemId = order.OrderItems.First().CatalogueItemId;

            var orderWrapper = new OrderWrapper(order);
            orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(orderWrapper);

            var routeValues = new RouteValues();
            routingService.GetRoute(RoutingPoint.EditDeliveryDatesBackLink, orderWrapper, Arg.Any<RouteValues>())
                .Returns(new RoutingResult
                {
                    ActionName = "action",
                    ControllerName = "controller",
                    RouteValues = new { internalOrgId, callOffId, catalogueItemId },
                })
                .AndDoes(callInfo =>
                {
                    routeValues = callInfo.Arg<RouteValues>();
                });

            var result = await controller.EditDates(internalOrgId, callOffId, catalogueItemId);

            await orderService.Received().GetOrderWithOrderItems(callOffId, internalOrgId);
            routingService.Received().GetRoute(RoutingPoint.EditDeliveryDatesBackLink, orderWrapper, Arg.Any<RouteValues>());

            routeValues.InternalOrgId.Should().Be(internalOrgId);
            routeValues.CallOffId.Should().Be(callOffId);
            routeValues.CatalogueItemId.Should().Be(catalogueItemId);
            routeValues.Source.Should().BeNull();

            var expected = new EditDatesModel(orderWrapper, catalogueItemId);
            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditDates_NoServiceRecipients_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderService,
            [Frozen] IRoutingService routingService,
            DeliveryDatesController controller)
        {
            order.SetupCatalogueSolution();
            order.OrderRecipients = new List<OrderRecipient>();

            var catalogueItemId = order.OrderItems.First().CatalogueItemId;

            var orderWrapper = new OrderWrapper(order);
            orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(orderWrapper);

            var routeValues = new RouteValues();
            routingService.GetRoute(RoutingPoint.EditDeliveryDates, orderWrapper, Arg.Any<RouteValues>())
                .Returns(new RoutingResult
                {
                    ActionName = "action",
                    ControllerName = "controller",
                    RouteValues = new { internalOrgId, callOffId, catalogueItemId },
                })
                .AndDoes(callInfo =>
                {
                    routeValues = callInfo.Arg<RouteValues>();
                });

            var result = await controller.EditDates(internalOrgId, callOffId, catalogueItemId);

            await orderService.Received().GetOrderWithOrderItems(callOffId, internalOrgId);
            routingService.Received().GetRoute(RoutingPoint.EditDeliveryDates, orderWrapper, Arg.Any<RouteValues>());

            routeValues.InternalOrgId.Should().Be(internalOrgId);
            routeValues.CallOffId.Should().Be(callOffId);
            routeValues.CatalogueItemId.Should().Be(catalogueItemId);
            routeValues.Source.Should().BeNull();

            result.Should().BeOfType<RedirectToActionResult>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditDates_NullDeliveryDate_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderService,
            [Frozen] IRoutingService routingService,
            DeliveryDatesController controller)
        {
            order.SetupCatalogueSolution();

            order.OrderRecipients.ForEach(x => x.OrderItemRecipients.FirstOrDefault().DeliveryDate = null);

            var catalogueItemId = order.OrderItems.First().CatalogueItemId;

            var orderWrapper = new OrderWrapper(order);
            orderService.GetOrderWithOrderItems(callOffId, internalOrgId)
                .Returns(orderWrapper);

            var routeValues = new RouteValues();
            routingService.GetRoute(RoutingPoint.EditDeliveryDatesBackLink, orderWrapper, Arg.Any<RouteValues>())
                .Returns(new RoutingResult
                {
                    ActionName = "action",
                    ControllerName = "controller",
                    RouteValues = new { internalOrgId, callOffId, catalogueItemId },
                })
                .AndDoes(callInfo =>
                {
                    routeValues = callInfo.Arg<RouteValues>();
                });

            var result = await controller.EditDates(internalOrgId, callOffId, catalogueItemId);

            await orderService.Received().GetOrderWithOrderItems(callOffId, internalOrgId);
            routingService.Received().GetRoute(RoutingPoint.EditDeliveryDatesBackLink, orderWrapper, Arg.Any<RouteValues>());

            routeValues.InternalOrgId.Should().Be(internalOrgId);
            routeValues.CallOffId.Should().Be(callOffId);
            routeValues.CatalogueItemId.Should().Be(catalogueItemId);
            routeValues.Source.Should().BeNull();

            var expected = new EditDatesModel(new OrderWrapper(order), catalogueItemId);
            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditDates_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            EditDatesModel model,
            DeliveryDatesController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.EditDates(internalOrgId, callOffId, catalogueItemId, model);
            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(model, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditDates_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            EditDatesModel model,
            [Frozen] IDeliveryDateService deliveryDatesService,
            [Frozen] IOrderService orderService,
            [Frozen] IRoutingService routingService,
            DeliveryDatesController controller)
        {
            order.SetupCatalogueSolution();

            var catalogueItemId = order.OrderItems.First().CatalogueItemId;

            var orderWrapper = new OrderWrapper(order);
            orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(orderWrapper);

            var deliveryDates = new List<RecipientDeliveryDateDto>();
            var routeValues = new RouteValues();

            deliveryDatesService.SetDeliveryDates(order.Id, catalogueItemId, Arg.Any<List<RecipientDeliveryDateDto>>())
                .Returns(Task.CompletedTask)
                .AndDoes(callInfo =>
                {
                    deliveryDates = callInfo.Arg<List<RecipientDeliveryDateDto>>();
                });

            routingService.GetRoute(RoutingPoint.EditDeliveryDates, orderWrapper, Arg.Any<RouteValues>())
                .Returns(new RoutingResult
                {
                    ActionName = "action",
                    ControllerName = "controller",
                    RouteValues = new { internalOrgId, callOffId },
                })
                .AndDoes(callInfo =>
                {
                    routeValues = callInfo.Arg<RouteValues>();
                });

            var result = await controller.EditDates(internalOrgId, callOffId, catalogueItemId, model);

            await deliveryDatesService.Received().SetDeliveryDates(order.Id, catalogueItemId, Arg.Any<List<RecipientDeliveryDateDto>>());
            await orderService.Received().GetOrderWithOrderItems(callOffId, internalOrgId);
            routingService.Received().GetRoute(RoutingPoint.EditDeliveryDates, orderWrapper, Arg.Any<RouteValues>());

            deliveryDates.Count.Should().Be(model.Recipients.Length);

            foreach (var deliveryDate in deliveryDates)
            {
                var recipient = model.Recipients.First(x => x.OdsCode == deliveryDate.OdsCode);
                recipient.Date.Should().Be(deliveryDate.DeliveryDate);
            }

            routeValues.InternalOrgId.Should().Be(internalOrgId);
            routeValues.CallOffId.Should().Be(callOffId);
            routeValues.CatalogueItemId.Should().Be(catalogueItemId);
            routeValues.Source.Should().Be(model.Source);

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be("action");
            actual.ControllerName.Should().Be("controller");
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_MatchDates_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderService,
            DeliveryDatesController controller)
        {
            order.SetupCatalogueSolution();

            var catalogueItem = order.OrderItems.ElementAt(1).CatalogueItem;

            orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.MatchDates(internalOrgId, callOffId, catalogueItem.Id);

            await orderService.Received().GetOrderWithOrderItems(callOffId, internalOrgId);

            var expected = new MatchDatesModel(internalOrgId, callOffId, catalogueItem);
            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_MatchDates_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            MatchDatesModel model,
            DeliveryDatesController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.MatchDates(internalOrgId, callOffId, catalogueItemId, model);
            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(model, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_MatchDates_MatchDatesIsTrue_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            MatchDatesModel model,
            [Frozen] IOrderService orderService,
            [Frozen] IDeliveryDateService deliveryDateService,
            DeliveryDatesController controller)
        {
            model.MatchDates = true;
            order.SetupCatalogueSolution();

            var solutionId = order.OrderItems.ElementAt(0).CatalogueItemId;
            var catalogueItemId = order.OrderItems.ElementAt(1).CatalogueItemId;

            var recipients = order.DetermineOrderRecipients(null, catalogueItemId);

            var recipientDates = await VerifyMatching(catalogueItemId, internalOrgId, callOffId, order, model, orderService, deliveryDateService, controller, recipients);

            recipientDates.Select(x => x.DeliveryDate).Should().BeEquivalentTo(recipients.Select(x => x.GetDeliveryDateForItem(solutionId)!.Value));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_MatchDates_MatchDatesIsTrue_ServiceHasNoExistingOrderItemRecipients_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            MatchDatesModel model,
            [Frozen] IOrderService orderService,
            [Frozen] IDeliveryDateService deliveryDateService,
            DeliveryDatesController controller)
        {
            model.MatchDates = true;
            order.SetupCatalogueSolution();

            var solutionId = order.OrderItems.ElementAt(0).CatalogueItemId;

            order.OrderRecipients.ForEach(x => x.OrderItemRecipients
                    .Where(y => y.CatalogueItemId != solutionId)
                    .ForEach(z => x.OrderItemRecipients.Remove(z)));

            var catalogueItemId = order.OrderItems.ElementAt(1).CatalogueItemId;

            var recipients = order.DetermineOrderRecipients(null, catalogueItemId);

            var recipientDates = await VerifyMatching(catalogueItemId, internalOrgId, callOffId, order, model, orderService, deliveryDateService, controller, recipients);

            recipientDates.Select(x => x.DeliveryDate).Should().BeEquivalentTo(recipients.Select(x => x.GetDeliveryDateForItem(solutionId)!.Value));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_MatchDates_MatchDatesIsFalse_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            MatchDatesModel model,
            [Frozen] IOrderService orderService,
            [Frozen] IDeliveryDateService deliveryDateService,
            DeliveryDatesController controller)
        {
            model.MatchDates = false;
            order.SetupCatalogueSolution();

            var catalogueItemId = order.OrderItems.ElementAt(1).CatalogueItemId;

            var recipients = order.DetermineOrderRecipients(null, catalogueItemId);

            var recipientDates = await VerifyMatching(catalogueItemId, internalOrgId, callOffId, order, model, orderService, deliveryDateService, controller, recipients);

            recipientDates.ForEach(x => x.DeliveryDate.Should().Be(order.DeliveryDate));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_MatchDates_MatchDatesIsFalse_ServiceHasNoExistingOrderItemRecipients_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            MatchDatesModel model,
            [Frozen] IOrderService orderService,
            [Frozen] IDeliveryDateService deliveryDateService,
            DeliveryDatesController controller)
        {
            model.MatchDates = false;
            order.SetupCatalogueSolution();

            var solutionId = order.OrderItems.ElementAt(0).CatalogueItemId;

            order.OrderRecipients.ForEach(x => x.OrderItemRecipients
                    .Where(y => y.CatalogueItemId != solutionId)
                    .ForEach(z => x.OrderItemRecipients.Remove(z)));

            var catalogueItemId = order.OrderItems.ElementAt(1).CatalogueItemId;

            var recipients = order.DetermineOrderRecipients(null, catalogueItemId);

            var recipientDates = await VerifyMatching(catalogueItemId, internalOrgId, callOffId, order, model, orderService, deliveryDateService, controller, recipients);

            recipientDates.ForEach(x => x.DeliveryDate.Should().Be(order.DeliveryDate));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_MatchDates_MatchDatesIsTrue_SolutionIdIsNullReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            MatchDatesModel model,
            [Frozen] IOrderService orderService,
            [Frozen] IDeliveryDateService deliveryDateService,
            DeliveryDatesController controller)
        {
            model.MatchDates = true;
            order.SetupCatalogueSolution();

            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;

            order.GetSolutionId().Should().BeNull();

            var catalogueItemId = order.OrderItems.ElementAt(1).CatalogueItemId;

            var recipients = order.DetermineOrderRecipients(null, catalogueItemId);

            var recipientDates = await VerifyMatching(catalogueItemId, internalOrgId, callOffId, order, model, orderService, deliveryDateService, controller, recipients);

            recipientDates.ForEach(x => x.DeliveryDate.Should().Be(order.DeliveryDate));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Review_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderService,
            DeliveryDatesController controller)
        {
            orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.Review(internalOrgId, callOffId);

            await orderService.Received().GetOrderWithOrderItems(callOffId, internalOrgId);

            var expected = new ReviewModel(new OrderWrapper(order));
            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        private static async Task<List<RecipientDeliveryDateDto>> VerifyMatching(
            CatalogueItemId catalogueItemId,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            MatchDatesModel model,
            [Frozen] IOrderService orderService,
            [Frozen] IDeliveryDateService deliveryDateService,
            DeliveryDatesController controller,
            ICollection<OrderRecipient> recipients)
        {
            orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var recipientDates = new List<RecipientDeliveryDateDto>();

            deliveryDateService.SetDeliveryDates(order.Id, catalogueItemId, Arg.Any<List<RecipientDeliveryDateDto>>())
                .Returns(Task.CompletedTask)
                .AndDoes(callInfo =>
                {
                    recipientDates = callInfo.Arg<List<RecipientDeliveryDateDto>>();
                });

            var result = await controller.MatchDates(internalOrgId, callOffId, catalogueItemId, model);

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(DeliveryDatesController.EditDates));
            actual.ControllerName.Should().Be(typeof(DeliveryDatesController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", catalogueItemId },
            });

            await orderService.Received().GetOrderWithOrderItems(callOffId, internalOrgId);
            await deliveryDateService.Received().SetDeliveryDates(order.Id, catalogueItemId, Arg.Any<List<RecipientDeliveryDateDto>>());

            recipientDates.Count.Should().Be(recipients.Count);
            recipientDates.Select(x => x.OdsCode).Should().BeEquivalentTo(recipients.Select(x => x.OdsCode));

            return recipientDates;
        }
    }
}
