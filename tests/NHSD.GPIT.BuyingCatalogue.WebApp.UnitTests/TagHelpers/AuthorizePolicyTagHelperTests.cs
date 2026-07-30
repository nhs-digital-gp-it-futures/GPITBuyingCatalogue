using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.TagHelpers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.TagHelpers;

public static class AuthorizePolicyTagHelperTests
{
    private const string Policy = "TestPolicy";

    [Fact]
    public static void Process_UserHasPolicyClaim_RendersElement()
    {
        var user = CreatePrincipal(CataloguePermissions.ClaimType, Policy);

        var tagHelper = CreateTagHelper(user);
        var output = CreateOutput();

        tagHelper.Process(CreateContext(), output);

        output.TagName.Should().Be("li");
    }

    [Fact]
    public static void Process_UserDoesNotHavePolicyClaim_SuppressesElement()
    {
        var user = new ClaimsPrincipal();

        var tagHelper = CreateTagHelper(user);
        var output = CreateOutput();

        tagHelper.Process(CreateContext(), output);

        output.TagName.Should().BeNull();
    }

    private static AuthorizePolicyTagHelper CreateTagHelper(ClaimsPrincipal user) =>
        new()
        {
            Policy = Policy,
            ViewContext = new ViewContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user,
                },
            },
        };

    private static ClaimsPrincipal CreatePrincipal(string claim, string value) =>
        new(new ClaimsIdentity(new[] { new Claim(claim, value) }, "mock"));

    private static TagHelperContext CreateContext() =>
        new(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            "test");

    private static TagHelperOutput CreateOutput() =>
        new(
            "li",
            new TagHelperAttributeList(),
            (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
}
