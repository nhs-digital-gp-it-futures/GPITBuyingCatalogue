using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.DetailsAndExpander
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class DetailsTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-details";

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var summary = DetailsAndExpanderTagHelperBuilders.GetSummaryLabelBuilder(LabelText);

            var textItem = DetailsAndExpanderTagHelperBuilders.GetTextItem();

            var children = await output.GetChildContentAsync();

            output.Reinitialize("details", TagMode.StartTagAndEndTag);

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, DetailsAndExpanderTagHelperBuilders.DetailsClass));

            textItem.InnerHtml.AppendHtml(children);

            output.Content
                .AppendHtml(summary)
                .AppendHtml(textItem);
        }
    }
}
