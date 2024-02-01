using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Card;

[HtmlTargetElement(TagHelperName)]
[RestrictChildren(CardContentTagHelper.TagHelperName, CardFooterTagHelper.TagHelperName)]
public class CardV2TagHelper : TagHelper
{
    public const string TagHelperName = "nhs-card-v2";

    public bool HorizontalAlign { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var cardContext = new CardContext { HorizontalAlign = HorizontalAlign };
        context.Items.Add(typeof(CardV2TagHelper), cardContext);

        _ = await output.GetChildContentAsync();

        output.TagMode = TagMode.StartTagAndEndTag;
        output.TagName = "div";
        output.AddClass(CardStyles.CardClass, HtmlEncoder.Default);

        if (cardContext.ShouldBeClickable)
        {
            output.AddClass(CardStyles.CardClickableClass, HtmlEncoder.Default);
        }

        if (HorizontalAlign)
        {
            output.AddClass(CardStyles.CardMinHeightClass, HtmlEncoder.Default);
        }

        output.Content
            .AppendHtml(cardContext.BodyContent)
            .AppendHtml(cardContext.FooterContent);
    }
}
