using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Card;

[HtmlTargetElement(TagHelperName)]
public class CardFooterTagHelper : TagHelper
{
    public const string TagHelperName = "nhs-card-footer";

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var cardContext = (CardContext)context.Items[typeof(CardV2TagHelper)];

        var content = new TagBuilder(TagHelperConstants.Div);

        content.AddCssClass("nhs-card-v2__footer");

        var childContent = await output.GetChildContentAsync();

        content.InnerHtml.AppendHtml(childContent.GetContent());

        cardContext.FooterContent = content;

        output.SuppressOutput();
    }
}
