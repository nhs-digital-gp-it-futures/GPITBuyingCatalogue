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

    private CardContext cardContext;

    public bool HorizontalAlign { get; set; }

    public bool Inverted { get; set; }

    public override void Init(TagHelperContext context)
    {
        cardContext = new CardContext { HorizontalAlign = HorizontalAlign };

        if (context.Items.TryGetValue(typeof(CardV2TagHelper), out _))
        {
            context.Items[typeof(CardV2TagHelper)] = cardContext;
        }
        else
        {
            context.Items.Add(typeof(CardV2TagHelper), cardContext);
        }
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
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

        if (Inverted)
        {
            output.AddClass(CardStyles.CardInverted, HtmlEncoder.Default);
        }

        if (cardContext.FooterContent is not null)
        {
            cardContext.BodyContent.AddCssClass("nhs-card-v2__content-with-footer");
        }

        output.Content
            .AppendHtml(cardContext.BodyContent)
            .AppendHtml(cardContext.FooterContent);
    }
}
