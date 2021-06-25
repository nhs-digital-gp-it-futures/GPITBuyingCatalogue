using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.DetailsAndExpander
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class DetailsTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-details";

        private const string ArrowsClass = "details-arrow";

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "details";
            output.TagMode = TagMode.StartTagAndEndTag;

            var summary = DetailsAndExpanderTagHelperBuilders.GetSummaryLabelBuilder(LabelText);

            var textItem = DetailsAndExpanderTagHelperBuilders.GetTextItem();

            var children = await output.GetChildContentAsync();

            var addArrowClass = InExpanderContext(context) == true ? ArrowsClass : string.Empty;

            output.Attributes.Add(new TagHelperAttribute(
                TagHelperConstants.Class,
                $"{DetailsAndExpanderTagHelperBuilders.DetailsClass} {addArrowClass}"));

            textItem.InnerHtml.AppendHtml(children);

            output.Content
                .AppendHtml(summary)
                .AppendHtml(textItem);
        }

        private static bool InExpanderContext(TagHelperContext context)
        {
            if (!context.Items.TryGetValue("InExpanderContext", out _))
                return false;

            return true;
        }
    }
}
