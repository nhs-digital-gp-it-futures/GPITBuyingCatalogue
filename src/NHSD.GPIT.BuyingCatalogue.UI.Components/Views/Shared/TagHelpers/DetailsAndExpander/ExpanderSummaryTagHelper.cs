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

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(AddedStickerName)]
        public bool AddedSticker { get; set; }

        [HtmlAttributeName(DetailsAndExpanderTagHelperBuilders.BoldTitle)]
        public bool BoldTitle { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var expanderContext = (ExpanderContext)context.Items[typeof(ExpanderV2TagHelper)];

            var content = DetailsAndExpanderTagHelperBuilders.GetSummaryLabelBuilder(
                null,
                LabelText,
                BoldTitle,
                AddedSticker);

            var childContent = await output.GetChildContentAsync();

            content.InnerHtml
                .AppendHtml(childContent.GetContent());

            expanderContext.SummaryContent = content;

            output.SuppressOutput();
        }
    }
}
