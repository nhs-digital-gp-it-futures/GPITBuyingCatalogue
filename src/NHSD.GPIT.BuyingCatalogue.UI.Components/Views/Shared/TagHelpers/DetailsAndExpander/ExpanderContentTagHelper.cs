using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.DetailsAndExpander
{
    [HtmlTargetElement(TagHelperName, ParentTag = ExpanderV2TagHelper.TagHelperName)]
    public sealed class ExpanderContentTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-expander-content";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var expanderContext = (ExpanderContext)context.Items[typeof(ExpanderV2TagHelper)];

            var content = DetailsAndExpanderTagHelperBuilders.GetTextItem();

            var childContent = await output.GetChildContentAsync();

            content.InnerHtml.AppendHtml(childContent.GetContent());

            expanderContext.BodyContent = content;

            output.SuppressOutput();
        }
    }
}
