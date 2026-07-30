using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.TagHelpers;

[HtmlTargetElement("*", Attributes = AttributeName)]
public sealed class AuthorizePolicyTagHelper : TagHelper
{
    private const string AttributeName = "asp-authorize-policy";

    [HtmlAttributeName(AttributeName)]
    public string Policy { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var hasClaim = ViewContext.HttpContext.User.HasClaim(CataloguePermissions.ClaimType, Policy);

        if (!hasClaim)
            output.SuppressOutput();
    }
}
