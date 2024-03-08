using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Card;

[HtmlTargetElement(TagHelperName, ParentTag = CardV2TagHelper.TagHelperName)]
public class CardContentTagHelper : TagHelper
{
    internal const string TagHelperName = "nhs-card-content";
    internal const string TitleName = "title";
    internal const string UrlName = "url";
    private const string SizeName = "size";
    private const string AlignmentName = "centre-alignment";

    [HtmlAttributeName(TitleName)]
    public string Title { get; set; }

    [HtmlAttributeName(UrlName)]
    public string Url { get; set; }

    [HtmlAttributeName(SizeName)]
    public HeadingSize HeadingSize { get; set; } = HeadingSize.Small;

    [HtmlAttributeName(AlignmentName)]
    public bool CentralAlignment { get; set; } = false;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var cardContext = (CardContext)context.Items[typeof(CardV2TagHelper)];

        var content = new TagBuilder(TagHelperConstants.Div);

        content.AddCssClass(CardStyles.CardContentClass);

        if (CentralAlignment)
            content.AddCssClass(CardStyles.CardCenterAlignmentClass);

        var childContent = await output.GetChildContentAsync();

        content.InnerHtml
            .AppendHtml(BuildHeading(cardContext))
            .AppendHtml(childContent.GetContent());

        content.AddCssClass("nhs-card-v2__content");

        cardContext.ShouldBeClickable = !string.IsNullOrWhiteSpace(Url);
        cardContext.BodyContent = content;

        output.SuppressOutput();
    }

    private TagBuilder BuildHeading(CardContext cardContext)
    {
        if (string.IsNullOrWhiteSpace(Title))
            return null;

        var heading = new TagBuilder(TagHelperConstants.HeaderTwo);

        heading.AddCssClass(CardStyles.CardHeadingClass);
        heading.AddCssClass(HeadingSize.ToHeading());

        if (string.IsNullOrWhiteSpace(Url))
        {
            heading.InnerHtml.AppendHtml(Title);

            if (cardContext.HorizontalAlign)
                heading.AddCssClass(CardStyles.CardHeadingMinHeightClass);
        }
        else
        {
            heading.InnerHtml.AppendHtml(BuildHeadingLink());
        }

        return heading;
    }

    private TagBuilder BuildHeadingLink()
    {
        var link = new TagBuilder(TagHelperConstants.Anchor);

        link.AddCssClass(CardStyles.CardLinkClass);
        link.Attributes.Add("href", Url);
        link.InnerHtml.Append(Title);

        return link;
    }
}
