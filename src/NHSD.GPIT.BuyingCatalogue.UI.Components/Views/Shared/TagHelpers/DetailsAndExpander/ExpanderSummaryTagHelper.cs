using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.DetailsAndExpander
{
    [HtmlTargetElement(TagHelperName, ParentTag = ExpanderV2TagHelper.TagHelperName)]
    public sealed class ExpanderSummaryTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-expander-summary";
        public const string AddedStickerName = "added-sticker";

        public const string SecondaryTextTitleName = "secondary-text-title";
        public const string SecondaryTextName = "secondary-text";
        public const string SecondaryUrlTitleName = "secondary-url-title";
        public const string SecondaryUrlName = "secondary-url";

        [HtmlAttributeName(TagHelperConstants.HeadingTextName)]
        public string HeadingText { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(AddedStickerName)]
        public bool AddedSticker { get; set; }

        [HtmlAttributeName(SecondaryTextTitleName)]
        public string SecondaryTextTitle { get; set; }

        [HtmlAttributeName(SecondaryTextName)]
        public string SecondaryText { get; set; }

        [HtmlAttributeName(SecondaryUrlTitleName)]
        public string SecondaryUrlTitle { get; set; }

        [HtmlAttributeName(SecondaryUrlName)]
        public string SecondaryUrl { get; set; }

        [HtmlAttributeName(DetailsAndExpanderTagHelperBuilders.BoldTitle)]
        public bool BoldTitle { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var expanderContext = (ExpanderContext)context.Items[typeof(ExpanderV2TagHelper)];

            if (string.IsNullOrWhiteSpace(SecondaryText) != string.IsNullOrWhiteSpace(SecondaryTextTitle))
                throw new ArgumentException($"you must set both {SecondaryTextName} and {SecondaryTextTitleName} or set neither.");

            if (string.IsNullOrWhiteSpace(SecondaryUrl) != string.IsNullOrWhiteSpace(SecondaryUrlTitle))
                throw new ArgumentException($"you must set both {SecondaryUrlName} and {SecondaryUrlTitleName} or set neither.");

            // Maybe split this up further to potential secondary info in expander
            var content = string.IsNullOrWhiteSpace(SecondaryTextTitle) && string.IsNullOrWhiteSpace(SecondaryUrlTitle)
                ? DetailsAndExpanderTagHelperBuilders.GetSummaryLabelBuilder(HeadingText, LabelText, BoldTitle, AddedSticker)
                : DetailsAndExpanderTagHelperBuilders.GetSummaryLabelBuilderWithSecondaryInformation(LabelText, SecondaryTextTitle, SecondaryText, SecondaryUrlTitle, SecondaryUrl, AddedSticker);

            var childContent = await output.GetChildContentAsync();

            content.InnerHtml
                .AppendHtml(childContent.GetContent());

            expanderContext.SummaryContent = content;

            output.SuppressOutput();
        }
    }
}
