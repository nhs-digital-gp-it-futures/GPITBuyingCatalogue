using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        private const string IndexName = "index";

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

        [HtmlAttributeName(IndexName)]
        public int Index { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, TagHelperConstants.RadioItemClass));

            var input = RadioButtonBuilders.GetRadioInputBuilder(ViewContext, For, htmlGenerator, Value, ValueName, Index);
            var label = RadioButtonBuilders.GetRadioLabelBuilder(ViewContext, For, htmlGenerator, Value, DisplayName, Index);

            var childContent = await output.GetChildContentAsync();

            if (!childContent.IsEmptyOrWhiteSpace)
            {
                bool IsTheCurrentItemChecked()
                {
                    var radioCheckedValue = Value
                        .GetType()
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => string.Equals(p.Name, ValueName))
                        .Select(p => p.GetValue(Value, null))
                        .FirstOrDefault();

                    return radioCheckedValue != null && Equals(radioCheckedValue, For.Model);
                }

                List<string> tags = new List<string>
                {
                    TagHelperConstants.NhsRadiosChildConditional,
                    TagHelperConstants.NhsRadiosChildConditionalHidden,
                };

                if (IsTheCurrentItemChecked())
                    tags.Remove(TagHelperConstants.NhsRadiosChildConditionalHidden);

                TagHelperFunctions.ProcessOutputForConditionalContent(
                    output,
                    context,
                    input,
                    childContent,
                    tags);
            }

            output.Content
                .AppendHtml(input)
                .AppendHtml(label);
        }
    }
}
