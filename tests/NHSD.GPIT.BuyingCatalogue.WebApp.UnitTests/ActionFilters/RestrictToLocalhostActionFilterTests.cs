using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ActionFilters
{
    public sealed class RestrictToLocalhostActionFilterTests
    {
        private readonly HttpContext httpContextMock;
        private readonly ActionExecutingContext actionExecutingContext;
        private readonly ActionExecutedContext actionExecutedContext;

        public RestrictToLocalhostActionFilterTests()
        {
            httpContextMock = Substitute.For<HttpContext>();

            var actionContext = new ActionContext(
                httpContextMock,
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
        public async Task RequestLocalhost_Returns_Ok()
        {
            var defaultContext = new DefaultHttpContext()
            {
                Connection =
                {
                    RemoteIpAddress = IPAddress.Loopback,
                },
            };

            httpContextMock.Connection.Returns(defaultContext.Connection);

            var filter = new RestrictToLocalhostActionFilter();

            await filter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task RequestNotLocalhost_Returns_UnauthorizedResult()
        {
            var defaultContext = new DefaultHttpContext()
            {
                Connection =
                {
                    RemoteIpAddress = new IPAddress(16885952),
                },
            };

            httpContextMock.Connection.Returns(defaultContext.Connection);

            var filter = new RestrictToLocalhostActionFilter();

            await filter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            actionExecutingContext.Result.Should().BeOfType<UnauthorizedResult>();
        }
    }
}
