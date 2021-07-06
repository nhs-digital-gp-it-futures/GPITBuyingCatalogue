using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Radios
{
    [HtmlTargetElement(TagHelperName, ParentTag = RadioButtonContainerTagHelper.TagHelperName)]
    public sealed class RadioButtonTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-radio-button";

        private readonly IHtmlGenerator htmlGenerator;

        public RadioButtonTagHelper(IHtmlGenerator htmlGenerator) => this.htmlGenerator = htmlGenerator;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(TagHelperConstants.Value)]
        public object Value { get; set; }

        [HtmlAttributeName(TagHelperConstants.ValueName)]
        public string ValueName { get; set; }

        [HtmlAttributeName(TagHelperConstants.DisplayName)]
        public string DisplayName { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, TagHelperConstants.RadioItemClass));

            var input = RadioButtonBuilders.GetRadioInputBuilder(ViewContext, For, htmlGenerator, Value, ValueName);
            var label = RadioButtonBuilders.GetRadioLabelBuilder(ViewContext, For, htmlGenerator, Value, DisplayName, ValueName);

            var childContent = await output.GetChildContentAsync();

            if (!childContent.IsEmptyOrWhiteSpace)
            {
                TagHelperFunctions.ProcessOutputForConditionalContent(
                    output,
                    context,
                    input,
                    childContent,
                    new[] { TagHelperConstants.NhsRadiosChildConditional, TagHelperConstants.NhsRadiosChildConditionalHidden });
            }

            output.Content
                .AppendHtml(input)
                .AppendHtml(label);
        }
    }
}
