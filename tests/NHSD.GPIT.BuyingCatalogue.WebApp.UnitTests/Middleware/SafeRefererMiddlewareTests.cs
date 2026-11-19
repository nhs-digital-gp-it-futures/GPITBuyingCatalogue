using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.WebApp.Middleware;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Middleware;

public sealed class SafeRefererMiddlewareTests
{
    [Fact]
    public async Task Invoke_WithSameOriginReferer_SetsSafePathAndRewritesHeader()
    {
        var nextCalled = false;
        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var logger = Substitute.For<ILogger<SafeRefererMiddleware>>();
        var middleware = new SafeRefererMiddleware(next, logger);

        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");
        context.Request.Path = "/current";
        context.Request.Headers.Referer = "https://example.com/products?page=2";

        await middleware.Invoke(context);

        context.Items[SafeRefererMiddleware.SafeRefererItemKey].Should().Be("/products?page=2");
        context.Request.Headers.Referer.ToString().Should().Be("https://example.com/products?page=2");
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Invoke_WithExternalReferer_UsesRootFallbackAndRewritesHeader()
    {
        var nextCalled = false;
        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var logger = Substitute.For<ILogger<SafeRefererMiddleware>>();
        var middleware = new SafeRefererMiddleware(next, logger);

        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");
        context.Request.Path = "/current";
        context.Request.Headers.Referer = "https://evil.com/phish";

        await middleware.Invoke(context);

        context.Items[SafeRefererMiddleware.SafeRefererItemKey].Should().Be("/");
        context.Request.Headers.Referer.ToString().Should().Be("https://example.com/");
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Invoke_WithPortMismatch_UsesRootFallbackAndRewritesHeader()
    {
        var nextCalled = false;
        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var logger = Substitute.For<ILogger<SafeRefererMiddleware>>();
        var middleware = new SafeRefererMiddleware(next, logger);

        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");
        context.Request.Path = "/current";
        context.Request.Headers.Referer = "https://example.com:8443/products?page=2";

        await middleware.Invoke(context);

        context.Items[SafeRefererMiddleware.SafeRefererItemKey].Should().Be("/");
        context.Request.Headers.Referer.ToString().Should().Be("https://example.com/");
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Invoke_WithExternalRefererHttpEvilDoingsCom_IsInterceptedAndLogged()
    {
        var nextCalled = false;
        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var logger = Substitute.For<ILogger<SafeRefererMiddleware>>();
        var middleware = new SafeRefererMiddleware(next, logger);

        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("localhost:5001");
        context.Request.Path = "/current";
        context.Request.Headers.Referer = "http://evil.doings.com";

        await middleware.Invoke(context);

        context.Items[SafeRefererMiddleware.SafeRefererItemKey].Should().Be("/");
        context.Request.Headers.Referer.ToString().Should().Be("https://localhost:5001/");
        nextCalled.Should().BeTrue();

        logger.Received(1).Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(state =>
                state.ToString()!.Contains("Rejected non-local Referer")
                && state.ToString()!.Contains("http://evil.doings.com")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }
}
