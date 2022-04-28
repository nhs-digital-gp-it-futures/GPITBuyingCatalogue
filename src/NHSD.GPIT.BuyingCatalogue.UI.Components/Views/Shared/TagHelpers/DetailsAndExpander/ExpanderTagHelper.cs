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
        public const string ColourModeName = "colour-mode";
        public const string SecondaryTextTitleName = "secondary-text-title";
        public const string SecondaryTextName = "secondary-text";

        private const string ExpanderClass = "nhsuk-expander";
        private const string ExpanderBlackAndWhite = "nhsuk-expander-black-and-white";

        public enum ExpanderColourMode
        {
            Normal = 0,
            BlackAndWhite = 1,
        }

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(ColourModeName)]
        public ExpanderColourMode ColourMode { get; set; }

        [HtmlAttributeName(SecondaryTextTitleName)]
        public string SecondaryTextTitle { get; set; }

        [HtmlAttributeName(SecondaryTextName)]
        public string SecondaryText { get; set; }

        public override void Init(TagHelperContext context)
        {
            if (!context.Items.TryGetValue("InExpanderContext", out _))
                context.Items.Add("InExpanderContext", true);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrWhiteSpace(SecondaryText) != string.IsNullOrWhiteSpace(SecondaryTextTitle))
                throw new ArgumentException($"you must set both {SecondaryTextName} and {SecondaryTextTitleName} or set neither.");

            output.TagName = "details";
            output.TagMode = TagMode.StartTagAndEndTag;

            var summary = string.IsNullOrWhiteSpace(SecondaryTextTitle)
                ? DetailsAndExpanderTagHelperBuilders.GetSummaryLabelBuilder(LabelText)
                : DetailsAndExpanderTagHelperBuilders.GetSummaryLabelBuilderWithSecondaryInformation(LabelText, SecondaryTextTitle, SecondaryText);

            var textItem = DetailsAndExpanderTagHelperBuilders.GetTextItem();

            var children = await output.GetChildContentAsync();

            var blackAndWhiteClass = ColourMode == ExpanderColourMode.BlackAndWhite ? ExpanderBlackAndWhite : string.Empty;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, $"{DetailsAndExpanderTagHelperBuilders.DetailsClass} {ExpanderClass} {blackAndWhiteClass}"));

            textItem.InnerHtml.AppendHtml(children);

            output.Content
                .AppendHtml(summary)
                .AppendHtml(textItem);
        }
    }
}
