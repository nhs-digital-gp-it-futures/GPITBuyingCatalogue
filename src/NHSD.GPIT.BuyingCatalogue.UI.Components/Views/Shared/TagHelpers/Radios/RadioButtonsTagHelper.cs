using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName, ParentTag = FieldSetFormLabelTagHelper.TagHelperName)]
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

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            List<TagBuilder> radioItems = BuildRadiosFromValueList();

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, TagHelperConstants.NhsRadios));

            radioItems.ForEach(ri => output.Content.AppendHtml(ri));

            TagHelperFunctions.TellParentTagIfThisTagIsInError(ViewContext, context, For);
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
            var label = TagHelperBuilders.GetRadioLabelBuilder(ViewContext, For, htmlGenerator, item, DisplayName, ValueName);

            builder.InnerHtml.AppendHtml(input);
            builder.InnerHtml.AppendHtml(label);

            return builder;
        }
    }
}
