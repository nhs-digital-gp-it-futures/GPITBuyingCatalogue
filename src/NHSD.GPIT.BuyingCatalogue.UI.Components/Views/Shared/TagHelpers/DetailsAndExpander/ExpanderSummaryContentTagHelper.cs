using System;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.DetailsAndExpander
{
    [HtmlTargetElement(TagHelperName, ParentTag = ExpanderSummaryTagHelper.TagHelperName)]
    public sealed class ExpanderSummaryContentTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-expander-summary-content";

        public const string TitleName = "title";
        public const string TextName = "text";
        public const string UrlTitleName = "url-title";
        public const string UrlName = "url";
        public const string JsOnlyName = "js-only";

        [HtmlAttributeName(TitleName)]
        public string Title { get; set; }

        [HtmlAttributeName(TextName)]
        public string Text { get; set; }

        [HtmlAttributeName(UrlTitleName)]
        public string UrlTitle { get; set; }

        [HtmlAttributeName(UrlName)]
        public string Url { get; set; }

        [HtmlAttributeName(JsOnlyName)]
        public bool JsOnly { get; set; } = false;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrWhiteSpace(Text) != string.IsNullOrWhiteSpace(Title))
                throw new ArgumentException($"you must set both {TextName} and {TitleName} or set neither.");

            if (string.IsNullOrWhiteSpace(Url) != string.IsNullOrWhiteSpace(UrlTitle))
                throw new ArgumentException($"you must set both {UrlName} and {UrlTitleName} or set neither.");

            var summaryClass = UrlTitle is not null ? TagHelperConstants.NhsSummaryTextSecondaryUrlClass : TagHelperConstants.NhsSummaryTextSecondaryClass;
            if (JsOnly)
                summaryClass += $" {TagHelperConstants.NhsJsOnly}";

            output.TagName = TagHelperConstants.Span;
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, summaryClass));

            if (!string.IsNullOrEmpty(Title))
            {
                var secondaryTitleBuilder = new TagBuilder("b");
                secondaryTitleBuilder.InnerHtml.Append(Title);

                output.Content
                    .AppendHtml(secondaryTitleBuilder)
                    .Append(Text);
            }

            if (!string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(UrlTitle))
            {
                var breakRow = new TagBuilder("br") { TagRenderMode = TagRenderMode.SelfClosing };
                output.Content
                    .AppendHtml(breakRow);
            }

            if (!string.IsNullOrEmpty(UrlTitle))
            {
                var secondaryUrlBuilder = new TagBuilder(TagHelperConstants.Anchor);

                secondaryUrlBuilder.AddCssClass($"{TagHelperConstants.NhsLink} {TagHelperConstants.NhsLinkNonVisited}");
                secondaryUrlBuilder.Attributes.Add(TagHelperConstants.Link, Url);
                secondaryUrlBuilder.Attributes.Add(TagHelperConstants.Style, "float: right;");
                secondaryUrlBuilder.InnerHtml.Append(UrlTitle);

                output.Content
                    .AppendHtml(secondaryUrlBuilder);
            }

            var childContent = await output.GetChildContentAsync();

            output.Content
                .AppendHtml(childContent.GetContent());
        }
    }
}
