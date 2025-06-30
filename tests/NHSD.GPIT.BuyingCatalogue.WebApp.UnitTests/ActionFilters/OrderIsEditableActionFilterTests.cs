using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ActionFilters;

public class OrderIsEditableActionFilterTests
{
    private const string OrgIntId = "AABB";

    [Theory]
    [MockInlineAutoData("sfgsdfg")]
    [MockInlineAutoData("C3321-91")]
    [MockInlineAutoData("C0-9")]
    public static async Task OnActionExecutionAsync_CallOffIdNotValid_LogsWarning(
        string invalidCallOffIds,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        [Frozen] ILogWrapper<OrderIsEditableActionFilterAttribute> logger,
        OrderIsEditableActionFilterAttribute filter)
    {
        var httpContextMock = Substitute.For<HttpContext>();
        var httpRequestMock = Substitute.For<HttpRequest>();

        httpRequestMock.Path.Returns(new PathString($"/orders/organisation/{OrgIntId}/{invalidCallOffIds}"));
        httpContextMock.Request.Returns(httpRequestMock);

        context.HttpContext = httpContextMock;
        context.Result = new OkResult();

        await filter.OnActionExecutionAsync(context, next);

        context.Result.Should().BeOfType<BadRequestResult>();

        logger.Received().LogWarning("Unable to retrieve CallOffId from route url");
    }

    public static IEnumerable<object[]> OrderIsNotEditableData()
    {
        return
        [
            // not editable because terminated
            [
                new Order
                {
                    Id = 55,
                    OrderNumber = 224876,
                    OrderingParty = new Organisation { InternalIdentifier = OrgIntId },
                    IsTerminated = true,
                },
            ],

            // not editable because deleted
            [
                new Order
                {
                    Id = 55,
                    OrderNumber = 224876,
                    OrderingParty = new Organisation { InternalIdentifier = OrgIntId },
                    IsDeleted = true,
                },
            ],

            // not editable because expired
            [
                new Order
                {
                    Id = 55,
                    OrderNumber = 224876,
                    OrderingParty = new Organisation { InternalIdentifier = OrgIntId },
                    CommencementDate = DateTime.UtcNow.AddMonths(-7),
                    MaximumTerm = 6,
                },
            ],

            // not editable because completed
            [
                new Order
                {
                    Id = 55,
                    OrderNumber = 224876,
                    OrderingParty = new Organisation { InternalIdentifier = OrgIntId },
                    CommencementDate = DateTime.UtcNow,
                    MaximumTerm = 6,
                    Completed = DateTime.UtcNow,
                },
            ],
        ];
    }

    [Theory]
    [MockMemberAutoData(nameof(OrderIsNotEditableData))]
    public static async Task OnActionExecutionAsync_OrderIsNotEditable_ReturnsError(
        Order order,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        [Frozen] IOrderService orderService,
        [Frozen] ILogWrapper<OrderIsEditableActionFilterAttribute> logger,
        OrderIsEditableActionFilterAttribute filter)
    {
        var httpContextMock = Substitute.For<HttpContext>();
        var httpRequestMock = Substitute.For<HttpRequest>();

        httpRequestMock.Path.Returns(new PathString($"/orders/organisation/{OrgIntId}/{order.CallOffId}"));
        httpContextMock.User.Returns(
            new ClaimsPrincipal(
                new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Role, "Buyer"),
                    new Claim("primaryOrganisationInternalIdentifier", OrgIntId),
                ])));

        httpContextMock.Request.Returns(httpRequestMock);

        context.HttpContext = httpContextMock;
        context.Result = new OkResult();

        orderService.GetOrderThin(order.CallOffId, order.OrderingParty.InternalIdentifier)
            .Returns(new OrderWrapper(order));

        await filter.OnActionExecutionAsync(context, next);

        context.Result.Should().BeOfType<BadRequestResult>();

        logger.Received().LogWarning("Attempt was made to edit non editable order {CallOffId}", order.CallOffId);
    }

    [Theory]
    [MockAutoData]
    public static async Task OnActionExecutionAsync_OrderIsEditable_ReturnsOk(
        Order order,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        [Frozen] IOrderService orderService,
        OrderIsEditableActionFilterAttribute filter)
    {
        var httpContextMock = Substitute.For<HttpContext>();
        var httpRequestMock = Substitute.For<HttpRequest>();

        httpRequestMock.Path.Returns(new PathString($"/orders/organisation/{OrgIntId}/{order.CallOffId}"));
        httpContextMock.User.Returns(
            new ClaimsPrincipal(
                new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Role, "Buyer"), new Claim("primaryOrganisationInternalIdentifier", OrgIntId),
                ])));

        httpContextMock.Request.Returns(httpRequestMock);

        context.HttpContext = httpContextMock;
        context.Result = new OkResult();

        order.IsTerminated = false;
        order.IsDeleted = false;
        order.CommencementDate = DateTime.UtcNow;
        order.MaximumTerm = 6;
        order.Completed = null;
        order.OrderingParty.InternalIdentifier = OrgIntId;

        orderService.GetOrderThin(order.CallOffId, order.OrderingParty.InternalIdentifier)
            .Returns(new OrderWrapper(order));

        await filter.OnActionExecutionAsync(context, next);

        context.Result.Should().BeOfType<OkResult>();
    }
}
