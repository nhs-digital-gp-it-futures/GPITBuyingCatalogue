using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Card
{
    [HtmlTargetElement(TagHelperName)]
    public class CardTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-card";
        public const string TextName = "text";
        public const string TitleName = "title";
        public const string UrlName = "url";
        public const string SetMinHeightName = "set-min-height";
        public const string SetHeadingMinHeightName = "set-heading-min-height";

        private const string CardClass = "nhsuk-card";
        private const string CardMinHeightClass = "nhs-card__min-height";
        private const string CardClickableClass = "nhsuk-card--clickable";
        private const string CardContentClass = "nhsuk-card__content";
        private const string CardDescriptionClass = "nhsuk-card__description";
        private const string CardHeadingClass = "nhsuk-card__heading";
        private const string CardHeadingMobileClass = "nhsuk-heading-s";
        private const string CardHeadingMinHeightClass = "card-title-min-height";
        private const string CardLinkClass = "nhsuk-card__link";

        [HtmlAttributeName(TextName)]
        public string Text { get; set; }

        [HtmlAttributeName(TitleName)]
        public string Title { get; set; }

        [HtmlAttributeName(UrlName)]
        public string Url { get; set; }

        [HtmlAttributeName(SetMinHeightName)]
        public bool SetMinHeight { get; set; }

        [HtmlAttributeName(SetHeadingMinHeightName)]
        public bool SetHeadingMinHeight { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.AddClass(CardClass, HtmlEncoder.Default);

            if (!string.IsNullOrWhiteSpace(Url))
            {
                output.AddClass(CardClickableClass, HtmlEncoder.Default);
            }

            if (SetMinHeight)
            {
                output.AddClass(CardMinHeightClass, HtmlEncoder.Default);
            }

            var content = await BuildContentAsync(output);
            output.Content.AppendHtml(content);
        }

        private async Task<TagBuilder> BuildContentAsync(TagHelperOutput output)
        {
            var content = new TagBuilder(TagHelperConstants.Div);

            content.AddCssClass(CardContentClass);

            content.InnerHtml
                .AppendHtml(BuildHeading())
                .AppendHtml(await BuildCardTextAsync(output));

            return content;
        }

        private TagBuilder BuildHeading()
        {
            var heading = new TagBuilder(TagHelperConstants.HeaderTwo);

            heading.AddCssClass(CardHeadingClass);
            heading.AddCssClass(CardHeadingMobileClass);

            if (SetHeadingMinHeight)
                heading.AddCssClass(CardHeadingMinHeightClass);

            if (string.IsNullOrWhiteSpace(Url))
            {
                heading.InnerHtml.Append(Title);
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

            link.AddCssClass(CardLinkClass);
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
            cardText.AddCssClass(CardDescriptionClass);
            cardText.InnerHtml.Append(Text);
            return cardText;
        }
    }
}
