using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ViewsTagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(CheckBoxTagName)]
    public class CheckboxContainerTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-checkbox-container";
        public const string CheckBoxTagName = "nhs-validation-checkbox";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Content.Clear();

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, TagHelperConstants.NhsCheckboxes));

            var content = await output.GetChildContentAsync();

            output.Content.AppendHtml(content);
        }
    }
}
