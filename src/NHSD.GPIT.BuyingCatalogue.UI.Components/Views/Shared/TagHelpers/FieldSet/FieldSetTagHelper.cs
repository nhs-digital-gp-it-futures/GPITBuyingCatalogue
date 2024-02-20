using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using static NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.FieldSet.FieldSetTagHelperBuilders;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class FieldSetTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-fieldset";

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(FieldSetSizeName)]
        public FieldSetSize SelectedSize { get; set; } = FieldSetSize.Large;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.FieldSet;
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", NhsFieldset);
            var fieldsetheading = GetFieldSetLegendHeadingBuilder(SelectedSize, LabelText);
            var content = await output.GetChildContentAsync();
            output.Content
                .AppendHtml(fieldsetheading)
                .AppendHtml(content);
        }
    }
}
