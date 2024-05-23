using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Extensions
{
    public sealed class HttpContextExtensionsTests
    {
        [Theory]
        [MockAutoData]
        public static void GetRefererUriBuilder_Uses_Request_Base_Uri(
            Uri requestUri,
            Uri refererUri,
            string path)
        {
            var refererBuilder = new UriBuilder(refererUri)
            {
                Path = path,
            };

            var context = new DefaultHttpContext()
            {
                Request =
                {
                    Scheme = requestUri.Scheme,
                    Host = new HostString(requestUri.Host),
                    Headers = { Referer = refererBuilder.Uri.ToString(), },
                },
            };

            var builder = context.GetRefererUriBuilder();
            builder.Uri.Authority.Should().Be(requestUri.Authority);
            builder.Uri.LocalPath.Should().Be($"/{path}");
        }
    }
}
