using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NHSD.GPIT.BuyingCatalogue.WebApp.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Middleware;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Extensions;

public sealed class HttpContextSafeRefererExtensionsTests
{
    [Fact]
    public void GetSafeRefererOrFallback_WithSafeRefererItem_ReturnsSafeReferer()
    {
        const string expectedSafeReferer = "/solutions?filter=test";
        const string fallback = "/";

        var context = new DefaultHttpContext();
        context.Items[SafeRefererMiddleware.SafeRefererItemKey] = expectedSafeReferer;

        var result = context.GetSafeRefererOrFallback(fallback);

        result.Should().Be(expectedSafeReferer);
    }

    [Fact]
    public void GetSafeRefererOrFallback_WithoutSafeRefererItem_ReturnsFallback()
    {
        const string fallback = "/";

        var context = new DefaultHttpContext();

        var result = context.GetSafeRefererOrFallback(fallback);

        result.Should().Be(fallback);
    }

    [Fact]
    public void GetSafeRefererOrFallback_WithNonStringSafeRefererItem_ReturnsFallback()
    {
        const string fallback = "/";

        var context = new DefaultHttpContext();
        context.Items[SafeRefererMiddleware.SafeRefererItemKey] = 123;

        var result = context.GetSafeRefererOrFallback(fallback);

        result.Should().Be(fallback);
    }

    [Fact]
    public void GetSafeRefererOrFallback_WithWhitespaceSafeRefererItem_ReturnsFallback()
    {
        const string fallback = "/";

        var context = new DefaultHttpContext();
        context.Items[SafeRefererMiddleware.SafeRefererItemKey] = "   ";

        var result = context.GetSafeRefererOrFallback(fallback);

        result.Should().Be(fallback);
    }
}
