using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.HintText
{
    [HtmlTargetElement(TagHelperName)]
    public class HintTextTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-hint-text";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context.Items.TryGetValue(typeof(LabelHintContext), out var value))
            {
                var cardContext = value as LabelHintContext;
                var content = new TagBuilder(TagHelperConstants.Div);
                content.AddCssClass(TagHelperConstants.NhsHint);
                var childContent = await output.GetChildContentAsync();
                content.InnerHtml.AppendHtml(childContent.GetContent());
                cardContext.LabelHintHtml = content;
                output.SuppressOutput();
            }
            else
            {
                output.TagName = TagHelperConstants.Div;
                output.AddClass(TagHelperConstants.NhsHint, HtmlEncoder.Default);
            }
        }
    }
}
