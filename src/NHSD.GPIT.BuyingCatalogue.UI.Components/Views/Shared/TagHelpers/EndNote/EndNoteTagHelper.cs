using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.EndNote
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class EndNoteTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-endnote";

        public const string EndNoteClass = "nhsuk-body-s nhsuk-u-secondary-text-color nhsuk-u-margin-top-7 nhsuk-u-margin-bottom-0";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, EndNoteClass));

            var children = await output.GetChildContentAsync();

            output.Content.AppendHtml(children);
        }
    }
}
