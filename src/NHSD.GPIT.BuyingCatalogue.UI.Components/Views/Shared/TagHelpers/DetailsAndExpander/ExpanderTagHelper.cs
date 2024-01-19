using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.DetailsAndExpander
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class ExpanderTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-expander";
        public const string AddedStickerName = "added-sticker";
        public const string ColourModeName = "colour-mode";
        public const string SecondaryTextTitleName = "secondary-text-title";
        public const string SecondaryTextName = "secondary-text";
        public const string OpenName = "open";
        public const string ContentOnlyName = "content-only";

        private const string ExpanderClass = "nhsuk-expander";
        private const string ExpanderIndexClass = "nhsuk-expander-index";
        private const string ExpanderBlackAndWhite = "nhsuk-expander-black-and-white";

        public enum ExpanderColourMode
        {
            Normal = 0,
            BlackAndWhite = 1,
        }

        [HtmlAttributeName(TagHelperConstants.HeadingTextName)]
        public string HeadingText { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(AddedStickerName)]
        public bool AddedSticker { get; set; }

        [HtmlAttributeName(ColourModeName)]
        public ExpanderColourMode ColourMode { get; set; }

        [HtmlAttributeName(SecondaryTextTitleName)]
        public string SecondaryTextTitle { get; set; }

        [HtmlAttributeName(SecondaryTextName)]
        public string SecondaryText { get; set; }

        [HtmlAttributeName(OpenName)]
        public bool Open { get; set; }

        [HtmlAttributeName(ContentOnlyName)]
        public bool ContentOnly { get; set; }

        [HtmlAttributeName(DetailsAndExpanderTagHelperBuilders.BoldTitle)]
        public bool BoldTitle { get; set; }

        public override void Init(TagHelperContext context)
        {
            if (!context.Items.TryGetValue("InExpanderContext", out _))
                context.Items.Add("InExpanderContext", true);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (ContentOnly)
            {
                output.TagName = null;
                return;
            }

            if (string.IsNullOrWhiteSpace(SecondaryText) != string.IsNullOrWhiteSpace(SecondaryTextTitle))
                throw new ArgumentException($"you must set both {SecondaryTextName} and {SecondaryTextTitleName} or set neither.");

            output.TagName = "details";
            output.TagMode = TagMode.StartTagAndEndTag;

            var summary = string.IsNullOrWhiteSpace(SecondaryTextTitle)
                ? DetailsAndExpanderTagHelperBuilders.GetSummaryLabelBuilder(HeadingText, LabelText, BoldTitle, AddedSticker)
                : DetailsAndExpanderTagHelperBuilders.GetSummaryLabelBuilderWithSecondaryInformation(LabelText, SecondaryTextTitle, SecondaryText, BoldTitle, AddedSticker);

            var textItem = DetailsAndExpanderTagHelperBuilders.GetTextItem();

            var children = await output.GetChildContentAsync();

            var classAttribute = $"{DetailsAndExpanderTagHelperBuilders.DetailsClass} {ExpanderClass} {ExpanderIndexClass}";
            if (ColourMode == ExpanderColourMode.BlackAndWhite)
                classAttribute += $" {ExpanderBlackAndWhite}";

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, classAttribute));

            textItem.InnerHtml.AppendHtml(children);

            if (Open)
                output.Attributes.Add("open", string.Empty);

            output.Content
                .AppendHtml(summary)
                .AppendHtml(textItem);
        }
    }
}
