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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ActionFilters;

public class OrderIsEditableActionFilterTests
{
    private const string orgIntId = "AABB";

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
                    OrderingParty = new Organisation { InternalIdentifier = orgIntId },
                    IsTerminated = true,
                },
            ],

            // not editable because deleted
            [
                new Order
                {
                    Id = 55,
                    OrderNumber = 224876,
                    OrderingParty = new Organisation { InternalIdentifier = orgIntId },
                    IsDeleted = true,
                },
            ],

            // not editable because expired
            [
                new Order
                {
                    Id = 55,
                    OrderNumber = 224876,
                    OrderingParty = new Organisation { InternalIdentifier = orgIntId },
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
                    OrderingParty = new Organisation { InternalIdentifier = orgIntId },
                    CommencementDate = DateTime.UtcNow,
                    MaximumTerm = 6,
                    Completed = DateTime.UtcNow,
                },
            ],
        ];
    }

    [Theory]
    [MockInlineAutoData(nameof(OrderIsNotEditableData))]
    public static async Task OnActionExecutionAsync_OrderIsNotEditable_ReturnsError(
        Order order,
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        [Frozen] IOrderService orderService,
        OrderIsEditableActionFilterAttribute filter)
    {
        var httpContextMock = Substitute.For<HttpContext>();
        var httpRequestMock = Substitute.For<HttpRequest>();

        httpRequestMock.Path.Returns(new PathString($"/orders/organisation/{orgIntId}/{order.CallOffId}"));
        httpContextMock.User.Returns(
            new ClaimsPrincipal(
                new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Role, "Buyer"),
                    new Claim("primaryOrganisationInternalIdentifier", orgIntId),
                ])));

        httpContextMock.Request.Returns(httpRequestMock);

        context.HttpContext = httpContextMock;

        orderService.GetOrderThin(order.CallOffId, order.OrderingParty.InternalIdentifier)
            .Returns(new OrderWrapper(order));

        await filter.OnActionExecutionAsync(context, next);

        context.Result.Should().BeOfType<BadRequestResult>();
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

        httpRequestMock.Path.Returns(new PathString($"/orders/organisation/{orgIntId}/{order.CallOffId}"));
        httpContextMock.User.Returns(
            new ClaimsPrincipal(
                new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Role, "Buyer"), new Claim("primaryOrganisationInternalIdentifier", orgIntId),
                ])));

        httpContextMock.Request.Returns(httpRequestMock);

        context.HttpContext = httpContextMock;

        order.IsTerminated = false;
        order.IsDeleted = false;
        order.CommencementDate = DateTime.UtcNow;
        order.MaximumTerm = 6;
        order.Completed = null;
        order.OrderingParty.InternalIdentifier = orgIntId;

        orderService.GetOrderThin(order.CallOffId, order.OrderingParty.InternalIdentifier)
            .Returns(new OrderWrapper(order));

        await filter.OnActionExecutionAsync(context, next);

        context.Result.Should().BeOfType<OkResult>();
    }
}
