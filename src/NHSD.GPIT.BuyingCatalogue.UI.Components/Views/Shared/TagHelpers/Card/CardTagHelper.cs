using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Card
{
    [Obsolete("Deprecated in favour of CardV2TagHelper")]
    [HtmlTargetElement(TagHelperName)]
    public class CardTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-card";
        public const string TextName = "text";
        public const string TitleName = "title";
        public const string UrlName = "url";
        public const string HorizontalAlignName = "horizontal-align";
        private const string SizeName = "size";

        [HtmlAttributeName(TextName)]
        public string Text { get; set; }

        [HtmlAttributeName(TitleName)]
        public string Title { get; set; }

        [HtmlAttributeName(UrlName)]
        public string Url { get; set; }

        [HtmlAttributeName(HorizontalAlignName)]
        public bool HorizontalAlign { get; set; }

        [HtmlAttributeName(SizeName)]
        public HeadingSize HeadingSize { get; set; } = HeadingSize.Small;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.AddClass(CardStyles.CardClass, HtmlEncoder.Default);

            if (!string.IsNullOrWhiteSpace(Url))
            {
                output.AddClass(CardStyles.CardClickableClass, HtmlEncoder.Default);

                if (HorizontalAlign)
                    output.AddClass(CardStyles.CardMinHeightClass, HtmlEncoder.Default);
            }

            var content = await BuildContentAsync(output);
            output.Content.AppendHtml(content);
        }

        private async Task<TagBuilder> BuildContentAsync(TagHelperOutput output)
        {
            var content = new TagBuilder(TagHelperConstants.Div);

            content.AddCssClass(CardStyles.CardContentClass);

            if (!string.IsNullOrWhiteSpace(Title))
            {
                content.InnerHtml
                    .AppendHtml(BuildHeading());
            }

            content.InnerHtml
                .AppendHtml(await BuildCardTextAsync(output));

            return content;
        }

        private TagBuilder BuildHeading()
        {
            var heading = new TagBuilder(TagHelperConstants.HeaderTwo);

            heading.AddCssClass(CardStyles.CardHeadingClass);
            heading.AddCssClass(HeadingSize.ToHeading());

            if (string.IsNullOrWhiteSpace(Url))
            {
                heading.InnerHtml.Append(Title);

                if (HorizontalAlign)
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

        private async Task<IHtmlContent> BuildCardTextAsync(TagHelperOutput output)
        {
            var hasText = !string.IsNullOrWhiteSpace(Text);

            if (!hasText)
                return await output.GetChildContentAsync();

            var cardText = new TagBuilder(TagHelperConstants.Paragraph);
            cardText.AddCssClass(CardStyles.CardDescriptionClass);
            cardText.InnerHtml.Append(Text);
            return cardText;
        }
    }
}
