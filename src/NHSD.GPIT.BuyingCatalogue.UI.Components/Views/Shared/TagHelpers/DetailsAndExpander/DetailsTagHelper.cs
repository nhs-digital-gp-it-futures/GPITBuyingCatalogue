using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.TagHelpers;
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

        [HtmlAttributeName(DetailsAndExpanderTagHelperBuilders.BoldTitle)]
        public bool BoldTitle { get; set; }

        [HtmlAttributeName(TagHelperConstants.Size)]
        public DetailsAndExpanderTagHelperBuilders.DetailsLabelSize Size { get; set; } = DetailsAndExpanderTagHelperBuilders.DetailsLabelSize.Standard;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "details";
            output.TagMode = TagMode.StartTagAndEndTag;

            var summary = DetailsAndExpanderTagHelperBuilders.GetDetailsSummaryLabelBuilder(LabelText, BoldTitle, Size);

            var textItem = DetailsAndExpanderTagHelperBuilders.GetTextItem();

            var children = await output.GetChildContentAsync();

            var addArrowClass = InExpanderContext(context) == true ? ArrowsClass : string.Empty;

            output.Attributes.Add(new TagHelperAttribute(
                TagHelperConstants.Class,
                $"{DetailsAndExpanderTagHelperBuilders.DetailsClass} {addArrowClass}"));

            if (Size == DetailsAndExpanderTagHelperBuilders.DetailsLabelSize.Small)
            {
                output.AddClass(DetailsAndExpanderTagHelperBuilders.DetailsSummaryTextSmallClass, HtmlEncoder.Default);
            }

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
