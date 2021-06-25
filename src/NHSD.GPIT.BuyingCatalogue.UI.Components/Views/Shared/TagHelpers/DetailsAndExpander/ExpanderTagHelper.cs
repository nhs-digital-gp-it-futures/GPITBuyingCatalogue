using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.DetailsAndExpander
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class ExpanderTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-expander";

        private const string ExpanderClass = "nhsuk-expander";

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        public override void Init(TagHelperContext context)
        {
            if (!context.Items.TryGetValue("InExpanderContext", out _))
                context.Items.Add("InExpanderContext", true);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "details";
            output.TagMode = TagMode.StartTagAndEndTag;

            var summary = DetailsAndExpanderTagHelperBuilders.GetSummaryLabelBuilder(LabelText);

            var textItem = DetailsAndExpanderTagHelperBuilders.GetTextItem();

            var children = await output.GetChildContentAsync();

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, $"{DetailsAndExpanderTagHelperBuilders.DetailsClass} {ExpanderClass}"));

            textItem.InnerHtml.AppendHtml(children);

            output.Content
                .AppendHtml(summary)
                .AppendHtml(textItem);
        }
    }
}
