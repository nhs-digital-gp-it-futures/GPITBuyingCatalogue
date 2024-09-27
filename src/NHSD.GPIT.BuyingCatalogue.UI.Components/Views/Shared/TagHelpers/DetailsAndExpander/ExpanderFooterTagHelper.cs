using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.DetailsAndExpander
{
    [HtmlTargetElement(TagHelperName, ParentTag = ExpanderV2TagHelper.TagHelperName)]
    public sealed class ExpanderFooterTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-expander-footer";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var expanderContext = (ExpanderContext)context.Items[typeof(ExpanderV2TagHelper)];

            var content = new TagBuilder(TagHelperConstants.Div);

            content.AddCssClass(TagHelperConstants.NhsExpanderFooter);

            var childContent = await output.GetChildContentAsync();

            content.InnerHtml
                .AppendHtml(childContent.GetContent());

            expanderContext.FooterContent = content;

            output.SuppressOutput();
        }
    }
}
