using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.Contracts;

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
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
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
        List<EntityFramework.Ordering.Models.Order> previousOrders,
        [Frozen] IOrderService orderService,
        DeliveryDatesController controller)
    {
        var existingDeliveryDate = previousOrders.First().DeliveryDate;
        orderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order, previousOrders));

        var result = await controller.SelectDate(internalOrgId, callOffId, setAllPDD: setAllPDD);

        await orderService.Received().GetOrderThin(callOffId, internalOrgId);

        var expected = new SelectDateModel(internalOrgId, callOffId, order, setAllPDD, existingDeliveryDate);
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
        model.ApplyToAll = false;

        orderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

        var result = await controller.SelectDate(internalOrgId, callOffId, model);

        await deliveryDatesService.Received().SetAllDeliveryDates(internalOrgId, callOffId, model.Date.Value);
        await orderService.Received().GetOrderThin(callOffId, internalOrgId);

        var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

        actual.ActionName.Should().Be(nameof(DeliveryDatesController.SelectOrganisations));
        actual.ControllerName.Should().Be(typeof(DeliveryDatesController).ControllerName());
        actual.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", internalOrgId },
                    { "callOffId", callOffId },
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
        model.ApplyToAll = false;

        orderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

        var result = await controller.SelectDate(internalOrgId, callOffId, model);

        await orderService.Received().GetOrderThin(callOffId, internalOrgId);
        await deliveryDatesService.Received().SetAllDeliveryDates(internalOrgId, callOffId, model.Date.Value);

        var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

        actual.ActionName.Should().Be(nameof(DeliveryDatesController.SelectOrganisations));
        actual.ControllerName.Should().Be(typeof(DeliveryDatesController).ControllerName());
        actual.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", internalOrgId },
                    { "callOffId", callOffId },
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
    [MockAutoData]
    public static async Task Get_SelectOrganisations_ReturnsExpectedResult(
        string internalOrgId,
        CallOffId callOffId,
        EntityFramework.Ordering.Models.Order order,
        [Frozen] IOrderService orderService,
        [Frozen] IRoutingService routingService,
        DeliveryDatesController controller)
    {
        order.SetupCatalogueSolution();
        var orderItemId = order.OrderItems.First().Id;
        var orderWrapper = new OrderWrapper(order);

        orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(orderWrapper);
        routingService.GetRoute(
                RoutingPoint.EditDeliveryDatesBackLink,
                orderWrapper,
                Arg.Any<RouteValues>())
            .Returns(new RoutingResult
            {
                ActionName = "action",
                ControllerName = "controller",
                RouteValues = new { internalOrgId, callOffId, orderItemId },
            });

        var result = await controller.SelectOrganisations(internalOrgId, callOffId);

        await orderService.Received().GetOrderWithOrderItems(callOffId, internalOrgId);
        var expected = new SelectOrganisationsModel(orderWrapper, internalOrgId, callOffId, orderItemId);
        var actual = result.Should().BeOfType<ViewResult>().Subject;

        actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Get_SelectOrganisations_NoRecipients_ReturnsExpectedResult(
        string internalOrgId,
        CallOffId callOffId,
        EntityFramework.Ordering.Models.Order order,
        [Frozen] IOrderService orderService,
        DeliveryDatesController controller)
    {
        order.SetupCatalogueSolution();
        order.OrderSublocations.Clear();
        var orderItemId = order.OrderItems.First().Id;
        var orderWrapper = new OrderWrapper(order);

        orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(orderWrapper);
        var result = await controller.SelectOrganisations(internalOrgId, callOffId);

        var actual = result.Should().BeOfType<ViewResult>().Subject;
        actual.Model.Should().BeOfType<SelectOrganisationsModel>();
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_SelectOrganisations_WithModelErrors_ReturnsModel(
        string internalOrgId,
        CallOffId callOffId,
        SelectOrganisationsModel model,
        DeliveryDatesController controller)
    {
        controller.ModelState.AddModelError("key", "errorMessage");

        var result = await controller.SelectOrganisations(internalOrgId, callOffId, model);

        var actual = result.Should().BeOfType<ViewResult>().Subject;
        actual.Model.Should().BeSameAs(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_SelectOrganisations_RedirectsWithSelectedRecipientIds(
        string internalOrgId,
        CallOffId callOffId,
        SelectOrganisationsModel model,
        DeliveryDatesController controller)
    {
        model.Recipients =
        [
            CreateSelectRecipient("recipient-1", selected: true),
            CreateSelectRecipient("recipient-2", selected: false),
            CreateSelectRecipient("recipient-3", selected: true),
        ];

        var result = await controller.SelectOrganisations(internalOrgId, callOffId, model);

        var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;
        actual.ActionName.Should().Be(nameof(DeliveryDatesController.ChangeOrganisationsDates));
        actual.ControllerName.Should().Be(typeof(DeliveryDatesController).ControllerName());
        actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
        {
            { "internalOrgId", internalOrgId },
            { "callOffId", callOffId },
            { "selectedRecipients", "recipient-1,recipient-3" },
        });
    }

    [Theory]
    [MockAutoData]
    public static async Task Get_ChangeOrganisationsDates_ReturnsExpectedResult(
        string internalOrgId,
        CallOffId callOffId,
        string selectedRecipients,
        EntityFramework.Ordering.Models.Order order,
        [Frozen] IOrderService orderService,
        [Frozen] IDeliveryDateService deliveryDatesService,
        DeliveryDatesController controller)
    {
        order.SetupCatalogueSolution();
        order.DeliveryDate = DateTime.Today;
        var orderWrapper = new OrderWrapper(order);
        var recipient = order.FlattenedRecipients.First();
        var recipientOrganisation = recipient.RecipientOdsOrganisation;
        var recipientOdsCode = recipient.RecipientOdsCode;

        orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(orderWrapper);
        deliveryDatesService.GetSelectedRecipientsAsync(
                callOffId.OrderNumber,
                Arg.Any<IEnumerable<string>>())
            .Returns([recipient]);

        var result = await controller.ChangeOrganisationsDates(
            internalOrgId,
            callOffId,
            selectedRecipients);

        await orderService.Received().GetOrderWithOrderItems(callOffId, internalOrgId);
        await deliveryDatesService.Received().GetSelectedRecipientsAsync(
            callOffId.OrderNumber,
            Arg.Any<IEnumerable<string>>());

        var expected = new SelectOrganisationsModel
        {
            InternalOrgId = internalOrgId,
            CallOffId = callOffId,
            OrderItemId = order.OrderItems.First().Id,
            Recipients =
            [
                new SelectRecipientToEditModel
                {
                    RecipientOrganisationId = recipientOrganisation.Id,
                    RecipientName = recipientOrganisation.Name,
                    RecipientOdsCode = recipientOdsCode,
                    ParentSublocationName = recipient.ParentSublocation.SublocationOrganisation.Name,
                    ParentSublocationOdsCode = recipient.ParentSublocationOdsCode,
                    Selected = true,
                    Status = recipient.OrderItemSublocationRecipients.Any(x => x.DeliveryDate != order.DeliveryDate)
                        ? "Updated"
                        : string.Empty,
                },
            ],
        };

        var actual = result.Should().BeOfType<ViewResult>().Subject;
        actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_ChangeOrganisationsDates_RedirectsToReview(
        string internalOrgId,
        CallOffId callOffId,
        DeliveryDatesController controller)
    {
        var result = await controller.ChangeOrganisationsDates(internalOrgId, callOffId);

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
    public static async Task Get_SetOrganisationDeliveryDates_ReturnsExpectedResult(
        string internalOrgId,
        CallOffId callOffId,
        string recipientOrganisationId,
        string selectedRecipients,
        EntityFramework.Ordering.Models.Order order,
        [Frozen] IOrderService orderService,
        [Frozen] IDeliveryDateService deliveryDatesService,
        DeliveryDatesController controller)
    {
        order.SetupCatalogueSolution();
        var orderWrapper = new OrderWrapper(order);
        var recipientOrderItem = order.FlattenedRecipients
            .First()
            .OrderItemSublocationRecipients
            .First();

        orderService.GetOrderThin(callOffId, internalOrgId).Returns(orderWrapper);
        deliveryDatesService.GetRecipientOrderItemsDeliveryDatesAsync(
                callOffId.OrderNumber,
                recipientOrganisationId)
            .Returns([recipientOrderItem]);

        var result = await controller.SetOrganisationDeliveryDates(
            internalOrgId,
            callOffId,
            recipientOrganisationId,
            selectedRecipients);

        await orderService.Received().GetOrderThin(callOffId, internalOrgId);
        await deliveryDatesService.Received().GetRecipientOrderItemsDeliveryDatesAsync(
            callOffId.OrderNumber,
            recipientOrganisationId);

        var expected = new SetOrganisationDeliveryDatesModel(
            internalOrgId,
            callOffId,
            order.OrderItems.First().Id,
            recipientOrganisationId,
            [recipientOrderItem],
            order);
        var actual = result.Should().BeOfType<ViewResult>().Subject;

        actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Get_SetOrganisationDeliveryDates_NoOrderItems_ReturnsNotFound(
        string internalOrgId,
        CallOffId callOffId,
        string recipientOrganisationId,
        EntityFramework.Ordering.Models.Order order,
        [Frozen] IOrderService orderService,
        [Frozen] IDeliveryDateService deliveryDatesService,
        DeliveryDatesController controller)
    {
        order.OrderItems.Clear();
        orderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));
        deliveryDatesService.GetRecipientOrderItemsDeliveryDatesAsync(
                callOffId.OrderNumber,
                recipientOrganisationId)
            .Returns([]);

        var result = await controller.SetOrganisationDeliveryDates(
            internalOrgId,
            callOffId,
            recipientOrganisationId,
            (string)null);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_SetOrganisationDeliveryDates_WithModelErrors_ReturnsModel(
        string internalOrgId,
        CallOffId callOffId,
        string recipientOrganisationId,
        SetOrganisationDeliveryDatesModel model,
        DeliveryDatesController controller)
    {
        controller.ModelState.AddModelError("key", "errorMessage");

        var result = await controller.SetOrganisationDeliveryDates(
            internalOrgId,
            callOffId,
            recipientOrganisationId,
            model);

        var actual = result.Should().BeOfType<ViewResult>().Subject;
        actual.Model.Should().BeSameAs(model);
    }

    [Theory]
    [MockInlineAutoData(false)]
    [MockInlineAutoData(true)]
    public static async Task Post_SetOrganisationDeliveryDates_RedirectsToExpectedAction(
        bool hasSelectedRecipients,
        string internalOrgId,
        CallOffId callOffId,
        string recipientOrganisationId,
        SetOrganisationDeliveryDatesModel model,
        [Frozen] IDeliveryDateService deliveryDatesService,
        DeliveryDatesController controller)
    {
        var deliveryDate = new DateTime(2025, 1, 1);
        model.OrderItemsDeliveryDates =
        [
            new OrderItemDeliveryDate
            {
                OrderSublocationRecipientId = 123,
                Day = deliveryDate.Day.ToString(),
                Month = deliveryDate.Month.ToString(),
                Year = deliveryDate.Year.ToString(),
            },
        ];
        model.SelectedRecipients = hasSelectedRecipients
            ? ["recipient-1", "recipient-2"]
            : null;

        var result = await controller.SetOrganisationDeliveryDates(
            internalOrgId,
            callOffId,
            recipientOrganisationId,
            model);

        await deliveryDatesService.Received().SetRecipientOrderItemDeliveryDatesAsync(
            Arg.Is<IEnumerable<RecipientOrderItemDeliveryDateDto>>(dates =>
                dates.Single().OrderSublocationRecipientId == 123
                && dates.Single().DeliveryDate == deliveryDate));

        var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;
        actual.ActionName.Should().Be(hasSelectedRecipients
            ? nameof(DeliveryDatesController.ChangeOrganisationsDates)
            : nameof(DeliveryDatesController.Review));
        actual.ControllerName.Should().Be(typeof(DeliveryDatesController).ControllerName());

        var expectedRouteValues = hasSelectedRecipients
            ? new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "SelectedRecipients", model.SelectedRecipients },
            }
            : new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            };

        actual.RouteValues.Should().BeEquivalentTo(expectedRouteValues);
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
        order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
        orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

        var result = await controller.Review(internalOrgId, callOffId);

        await orderService.Received().GetOrderWithOrderItems(callOffId, internalOrgId);

        var expected = new ReviewModel(new OrderWrapper(order));
        var actual = result.Should().BeOfType<ViewResult>().Subject;

        actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
    }

    private static OrderSublocationRecipient CreateRecipient(OrderSublocation sublocation, string recipientOdsCode, DateTime solutionDeliveryDate, bool createRecipientForItem)
    {
        var recipient = new OrderSublocationRecipient(recipientOdsCode, sublocation.SublocationOdsCode)
        {
            Order = sublocation.Order,
            OrderId = sublocation.OrderId,
            ParentSublocation = sublocation,
        };

        var solution = sublocation.Order.OrderItems.First();
        var orderItem = sublocation.Order.OrderItems.ElementAt(1);

        recipient.OrderItemSublocationRecipients = new List<OrderItemSublocationRecipient>
        {
            CreateOrderItemRecipient(recipient, solution, solutionDeliveryDate),
            createRecipientForItem ? CreateOrderItemRecipient(recipient, orderItem, null) : null,
        };

        return recipient;
    }

    private static OrderItemSublocationRecipient CreateOrderItemRecipient(
        OrderSublocationRecipient recipient,
        OrderItem item,
        DateTime? deliveryDate)
    {
        return new OrderItemSublocationRecipient(recipient.OrderId, recipient.RecipientOdsCode, item)
        {
            DeliveryDate = deliveryDate,
            ParentSublocationOdsCode = recipient.ParentSublocationOdsCode,
            Recipient = recipient,
        };
    }

    private static SelectRecipientToEditModel CreateSelectRecipient(string organisationId, bool selected)
    {
        return new SelectRecipientToEditModel
        {
            Selected = selected,
            ParentSublocationName = "Sublocation",
            ParentSublocationOdsCode = "sublocation-ods",
            RecipientOrganisationId = organisationId,
            RecipientName = organisationId,
            RecipientOdsCode = organisationId,
        };
    }
}
