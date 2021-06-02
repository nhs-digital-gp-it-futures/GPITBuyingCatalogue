using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName, ParentTag = FieldSetFormTagHelper.TagHelperName)]
    public sealed class RadioButtonsTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-radio-buttons";
        public const string ValuesName = "values";
        public const string ValueNameName = "value-name";
        public const string DisplayNameName = "display-name";

        private readonly IHtmlGenerator htmlGenerator;

        public RadioButtonsTagHelper(IHtmlGenerator htmlGenerator) => this.htmlGenerator = htmlGenerator;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(ValuesName)]
        public IEnumerable<object> Values { get; set; }

        [HtmlAttributeName(ValueNameName)]
        public string ValueName { get; set; }

        [HtmlAttributeName(DisplayNameName)]
        public string DisplayName { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Values.Any() || string.IsNullOrWhiteSpace(ValueName) || string.IsNullOrWhiteSpace(DisplayName))
            {
                output.SuppressOutput();
                return;
            }

            List<TagBuilder> radioItems = BuildRadiosFromValueList();

            output.Content.Clear();

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, TagHelperConstants.NhsRadios));

            radioItems.ForEach(ri => output.Content.AppendHtml(ri));
        }

        private List<TagBuilder> BuildRadiosFromValueList()
        {
            return (from item in Values
                    select BuildRadioItem(item)).ToList();
        }

        private TagBuilder BuildRadioItem(object item)
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.AddCssClass(TagHelperConstants.RadioItemClass);

            var input = TagHelperBuilders.GetRadioInputBuilder(ViewContext, For, htmlGenerator, item, ValueName);
            var label = TagHelperBuilders.GetRadioLabelBuilder(ViewContext, For, htmlGenerator, item, DisplayName);

            builder.InnerHtml.AppendHtml(input);
            builder.InnerHtml.AppendHtml(label);

            return builder;
        }
    }
}
