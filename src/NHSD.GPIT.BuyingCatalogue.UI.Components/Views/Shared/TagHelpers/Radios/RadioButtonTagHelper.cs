using System;
using System.Collections.Generic;
using System.Linq;
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
        private const string DisplayedTextName = "display-text";

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

        [HtmlAttributeName(DisplayedTextName)]
        public string DisplayText { get; set; }

        [HtmlAttributeName(TagHelperConstants.HintName)]
        public string HintName { get; set; }

        [HtmlAttributeName(TagHelperConstants.HintTextName)]
        public string HintText { get; set; }

        [HtmlAttributeName(TagHelperConstants.IndexName)]
        public int Index { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var valueIsComplexObject = !Value.GetType().IsPrimitive && Value.GetType() != typeof(string);

            if (!valueIsComplexObject && string.IsNullOrWhiteSpace(DisplayText))
                throw new ArgumentException("Primitive value types must be used with DisplayText");

            if (valueIsComplexObject && (string.IsNullOrWhiteSpace(ValueName) || string.IsNullOrWhiteSpace(DisplayName)))
                throw new ArgumentException("Complex value types must be used with ValueName and DisplayName");

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, TagHelperConstants.RadioItemClass));

            var hint = BuildHintFromTextOrValue();
            var input = valueIsComplexObject
                ? RadioButtonBuilders.GetRadioInputBuilder(ViewContext, For, htmlGenerator, Value, ValueName, Index, hint)
                : RadioButtonBuilders.GetRadioInputBuilder(ViewContext, For, htmlGenerator, Value, Index, hintBuilder: hint);

            var label = valueIsComplexObject
                ? RadioButtonBuilders.GetRadioLabelBuilder(ViewContext, For, htmlGenerator, Value, DisplayName, Index)
                : RadioButtonBuilders.GetRadioLabelBuilder(ViewContext, For, htmlGenerator, DisplayText, Index);

            var childContent = await output.GetChildContentAsync();

            if (!childContent.IsEmptyOrWhiteSpace)
            {
                var tags = new List<string>(2)
                {
                    TagHelperConstants.NhsRadiosChildConditional,
                    TagHelperConstants.NhsRadiosChildConditionalHidden,
                };

                if (input.Attributes.Keys.Any(key => key == "checked"))
                    tags.Remove(TagHelperConstants.NhsRadiosChildConditionalHidden);

                TagHelperFunctions.ProcessOutputForConditionalContent(
                    output,
                    context,
                    input,
                    childContent,
                    tags);
            }

            TagHelperFunctions.TellParentTagIfThisTagIsInError(ViewContext, context, For);

            output.Content
                .AppendHtml(input)
                .AppendHtml(label)
                .AppendHtml(hint);
        }

        private TagBuilder BuildHintFromTextOrValue()
        {
            if (!string.IsNullOrWhiteSpace(HintName))
                return RadioButtonBuilders.GetRadioHintBuilder(For, Value, HintName, Index);
            else if (!string.IsNullOrWhiteSpace(HintText))
                return RadioButtonBuilders.GetRadioHintBuilder(For, HintText, Index);
            return null;
        }
    }
}
