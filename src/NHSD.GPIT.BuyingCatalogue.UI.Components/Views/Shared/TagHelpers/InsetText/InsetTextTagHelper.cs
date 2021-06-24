using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.InsetText
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class InsetTextTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-inset-text";

        private const string NhsInsetTextClass = "nhsuk-inset-text";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, NhsInsetTextClass));

            var children = await output.GetChildContentAsync();

            var span = TagHelperBuilders.GetVisuallHiddenSpanClassBuilder();

            output.Content
                .AppendHtml(span)
                .AppendHtml(children);
        }
    }
}
