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
        private readonly Mock<HttpRequest> httpRequestMock;
        private readonly Mock<HttpContext> httpContextMock;
        private readonly ActionExecutingContext actionExecutingContext;
        private readonly ActionExecutedContext actionExecutedContext;

        public OrdersActionFilterTests()
        {
            httpRequestMock = new Mock<HttpRequest>();
            httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);

            var actionContext = new ActionContext(
                httpContextMock.Object,
                Mock.Of<Microsoft.AspNetCore.Routing.RouteData>(),
                Mock.Of<ActionDescriptor>(),
                new ModelStateDictionary());

            actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>())
            {
                Result = new OkResult(),
            };

            actionExecutedContext = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());
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
            httpRequestMock.Setup(r => r.Path).Returns("/admin");

            var ordersActionFilter = new OrdersActionFilter(new Mock<ILogWrapper<OrdersActionFilter>>().Object);

            await ordersActionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task OrderMatchesUsersPrimaryInternalOrgId_ReturnsOk()
        {
            httpRequestMock.Setup(r => r.Path).Returns("/order/organisation/ABC/edit");

            httpContextMock.Setup(c => c.User).Returns(new ClaimsPrincipal(
                     new ClaimsIdentity(
                         new Claim[]
                         {
                             new(ClaimTypes.Role, "Buyer"),
                             new("primaryOrganisationInternalIdentifier", "ABC"),
                         })));

            var ordersActionFilter = new OrdersActionFilter(new Mock<ILogWrapper<OrdersActionFilter>>().Object);

            await ordersActionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task OrderMatchesUsersSecondaryInternalOrgId_ReturnsOk()
        {
            httpRequestMock.Setup(r => r.Path).Returns("/order/organisation/GHI/edit");

            httpContextMock.Setup(c => c.User).Returns(new ClaimsPrincipal(
                     new ClaimsIdentity(
                         new Claim[]
                         {
                             new(ClaimTypes.Role, "Buyer"),
                             new("primaryOrganisationInternalIdentifier", "ABC"),
                             new("secondaryOrganisationInternalIdentifier", "DEF"),
                             new("secondaryOrganisationInternalIdentifier", "GHI"),
                             new("secondaryOrganisationInternalIdentifier", "JKL"),
                         })));

            var ordersActionFilter = new OrdersActionFilter(new Mock<ILogWrapper<OrdersActionFilter>>().Object);

            await ordersActionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task OrderDoesntMatch_UsersPrimaryOrSecondaryInternalOrgIds_ReturnsNotFoundResult()
        {
            const string internalOrgId = "MNO";
            var requestPath = $"/order/organisation/{internalOrgId}/edit";

            httpRequestMock.Setup(r => r.Path).Returns(requestPath);

            httpContextMock.Setup(c => c.User).Returns(new ClaimsPrincipal(
                     new ClaimsIdentity(
                         new Claim[]
                         {
                             new(ClaimTypes.Role, "Buyer"),
                             new("primaryOrganisationInternalIdentifier", "ABC"),
                             new("secondaryOrganisationInternalIdentifier", "DEF"),
                             new("secondaryOrganisationInternalIdentifier", "GHI"),
                             new("secondaryOrganisationInternalIdentifier", "JKL"),
                         })));

            var mockLogger = new Mock<ILogWrapper<OrdersActionFilter>>();
            mockLogger.Setup(l => l.LogWarning(It.IsAny<string>()));

            var ordersActionFilter = new OrdersActionFilter(mockLogger.Object);

            await ordersActionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            actionExecutingContext.Result.Should().BeOfType<NotFoundResult>();
            mockLogger.Verify(l => l.LogWarning($"Attempt was made to access {requestPath} when user cannot access {internalOrgId}."), Times.Once);
        }
    }
}
