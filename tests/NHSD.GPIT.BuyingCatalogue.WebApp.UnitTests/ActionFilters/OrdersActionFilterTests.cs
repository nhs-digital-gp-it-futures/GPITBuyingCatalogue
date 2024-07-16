using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ActionFilters
{
    public class OrdersActionFilterTests
    {
        private readonly HttpRequest httpRequestMock;
        private readonly HttpContext httpContextMock;
        private readonly ActionExecutingContext actionExecutingContext;
        private readonly ActionExecutedContext actionExecutedContext;

        public OrdersActionFilterTests()
        {
            httpRequestMock = Substitute.For<HttpRequest>();
            httpContextMock = Substitute.For<HttpContext>();
            httpContextMock.Request.Returns(httpRequestMock);

            var actionContext = new ActionContext(
                httpContextMock,
                Substitute.For<Microsoft.AspNetCore.Routing.RouteData>(),
                Substitute.For<ActionDescriptor>(),
                new ModelStateDictionary());

            actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Substitute.For<Controller>())
            {
                Result = new OkResult(),
            };

            actionExecutedContext = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Substitute.For<Controller>());
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrdersActionFilter).GetConstructors();

            assertion.Verify(constructors);
        }

        [Fact]
        public async Task RequestNotOrderRelated_Returns_Ok()
        {
            httpRequestMock.Path.Returns(new PathString("/admin"));

            var mockLogger = Substitute.For<ILogWrapper<OrdersActionFilter>>();
            var ordersActionFilter = new OrdersActionFilter(mockLogger);

            await ordersActionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task OrderMatchesUsersPrimaryInternalOrgId_ReturnsOk()
        {
            httpRequestMock.Path.Returns(new PathString("/order/organisation/ABC/edit"));

            httpContextMock.User.Returns(new ClaimsPrincipal(
                     new ClaimsIdentity(
                         new Claim[]
                         {
                             new(ClaimTypes.Role, "Buyer"),
                             new("primaryOrganisationInternalIdentifier", "ABC"),
                         })));

            var mockLogger = Substitute.For<ILogWrapper<OrdersActionFilter>>();
            var ordersActionFilter = new OrdersActionFilter(mockLogger);

            await ordersActionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task OrderMatchesUsersSecondaryInternalOrgId_ReturnsOk()
        {
            httpRequestMock.Path.Returns(new PathString("/order/organisation/GHI/edit"));

            httpContextMock.User.Returns(new ClaimsPrincipal(
                     new ClaimsIdentity(
                         new Claim[]
                         {
                             new(ClaimTypes.Role, "Buyer"),
                             new("primaryOrganisationInternalIdentifier", "ABC"),
                             new("secondaryOrganisationInternalIdentifier", "DEF"),
                             new("secondaryOrganisationInternalIdentifier", "GHI"),
                             new("secondaryOrganisationInternalIdentifier", "JKL"),
                         })));

            var mockLogger = Substitute.For<ILogWrapper<OrdersActionFilter>>();
            var ordersActionFilter = new OrdersActionFilter(mockLogger);

            await ordersActionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task OrderDoesntMatch_UsersPrimaryOrSecondaryInternalOrgIds_ReturnsNotFoundResult()
        {
            const string internalOrgId = "MNO";
            var requestPath = $"/order/organisation/{internalOrgId}/edit";

            httpRequestMock.Path.Returns(new PathString(requestPath));

            httpContextMock.User.Returns(new ClaimsPrincipal(
                     new ClaimsIdentity(
                         new Claim[]
                         {
                             new(ClaimTypes.Role, "Buyer"),
                             new("primaryOrganisationInternalIdentifier", "ABC"),
                             new("secondaryOrganisationInternalIdentifier", "DEF"),
                             new("secondaryOrganisationInternalIdentifier", "GHI"),
                             new("secondaryOrganisationInternalIdentifier", "JKL"),
                         })));

            var mockLogger = Substitute.For<ILogWrapper<OrdersActionFilter>>();

            var ordersActionFilter = new OrdersActionFilter(mockLogger);

            await ordersActionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            actionExecutingContext.Result.Should().BeOfType<NotFoundResult>();
            mockLogger.Received().LogWarning($"Attempt was made to access {requestPath} when user cannot access {internalOrgId}.");
        }
    }
}
