using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [HtmlAttributeName(TitleName)]
        public string Title { get; set; }

        [HtmlAttributeName(TextName)]
        public string Text { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrWhiteSpace(Text) != string.IsNullOrWhiteSpace(Title))
                throw new ArgumentException($"you must set both {TextName} and {TitleName} or set neither.");

            output.TagName = TagHelperConstants.Span;
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, TagHelperConstants.NhsSummaryTextSecondaryClass));

            if (!string.IsNullOrEmpty(Title))
            {
                var secondaryTitleBuilder = new TagBuilder("b");
                secondaryTitleBuilder.InnerHtml.Append(Title);

                output.Content
                    .AppendHtml(secondaryTitleBuilder)
                    .Append(Text);
            }

            var childContent = await output.GetChildContentAsync();

            output.Content
                .AppendHtml(childContent.GetContent());
        }
    }
}
