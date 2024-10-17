using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(SubmitButtonTagHelper.TagHelperName, "vc:nhs-secondary-button", TagHelperConstants.Anchor, "vc:nhs-delete-button")]
    public class ButtonGroupTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-button-group";
        public const string ButtonGroupClassName = "nhsuk-button-group";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            var content = await output.GetChildContentAsync();

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, ButtonGroupClassName));

            output.Content.AppendHtml(content);
        }
    }
}
