using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;
using static System.Net.Mime.MediaTypeNames;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.DetailsAndExpander
{
    [HtmlTargetElement(TagHelperName, ParentTag = ExpanderV2TagHelper.TagHelperName)]
    public sealed class ExpanderSummaryTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-expander-summary";
        public const string AddedStickerName = "added-sticker";
        public const string SecondaryTitleName = "secondary-text-title";
        public const string SecondaryTextName = "secondary-text";

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(AddedStickerName)]
        public bool AddedSticker { get; set; }

        [HtmlAttributeName(DetailsAndExpanderTagHelperBuilders.BoldTitle)]
        public bool BoldTitle { get; set; }

        [HtmlAttributeName(SecondaryTitleName)]
        public string SecondaryTitle { get; set; }

        [HtmlAttributeName(SecondaryTextName)]
        public string SecondaryText { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrWhiteSpace(SecondaryText) != string.IsNullOrWhiteSpace(SecondaryTitle))
                throw new ArgumentException($"you must set both {SecondaryTextName} and {SecondaryTitleName} or set neither.");

            var expanderContext = (ExpanderContext)context.Items[typeof(ExpanderV2TagHelper)];

            var content = DetailsAndExpanderTagHelperBuilders.GetSummaryLabelBuilder(
                null,
                LabelText,
                BoldTitle,
                AddedSticker);

            if (SecondaryText != null)
            {
                var secondaryContent = GetSecondaryContent(SecondaryTitle, SecondaryText);
                content.InnerHtml
                    .AppendHtml(secondaryContent);
            }
            else
            {
                var childContent = await output.GetChildContentAsync();
                content.InnerHtml
                    .AppendHtml(childContent.GetContent());
            }

            expanderContext.SummaryContent = content;

            output.SuppressOutput();
        }

        private TagBuilder GetSecondaryContent(string title, string text)
        {
            var secondarySpanBuilder = new TagBuilder(TagHelperConstants.Span);
            secondarySpanBuilder.AddCssClass(TagHelperConstants.NhsSummaryTextSecondaryClass);

            var secondaryTitleBuilder = new TagBuilder("b");
            secondaryTitleBuilder.InnerHtml.Append(title);

            secondarySpanBuilder.InnerHtml
                .AppendHtml(secondaryTitleBuilder)
                .Append(text);

            return secondarySpanBuilder;
        }
    }
}
