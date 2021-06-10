using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(CheckboxTagHelper.TagHelperName)]
    public class CheckboxContainerTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-checkbox-container";

        private const string NhsCheckboxes = "nhsuk-checkboxes";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Reinitialize(TagHelperConstants.Div, TagMode.StartTagAndEndTag);

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, NhsCheckboxes));

            var content = await output.GetChildContentAsync();

            output.Content.AppendHtml(content);
        }
    }
}
