using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ViewsTagHelpers
{
    [HtmlTargetElement(TagHelperName, ParentTag = ParentTagName)]
    public sealed class RadioButtonsTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-radio-buttons";
        public const string ParentTagName = "nhs-fieldset-form";
        public const string ValuesName = "values";
        public const string ValueNameName = "value-name";
        public const string DisplayNameName = "display-name";


        private const string RadioItemClass = "nhsuk-radios__item";
        private const string RadioItemInputClass  = "nhsuk-radios__input";
        private const string RadioLabelClass = "nhsuk-radios__label";
        private const string NhsRadios = "nhsuk-radios";

        private IHtmlGenerator htmlGenerator;

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

        public RadioButtonsTagHelper(IHtmlGenerator htmlGenerator)
        {
            this.htmlGenerator = htmlGenerator;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Values.Any() || string.IsNullOrEmpty(ValueName) || string.IsNullOrEmpty(DisplayName))
            {
                output.SuppressOutput();
                return;
            }

            var radioItems = new List<TagBuilder>();

            foreach(var item in Values)
            {
                radioItems.Add(BuildRadioItem(item));
            }

            output.Content.Clear();

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, NhsRadios));

            radioItems.ForEach(ri => output.Content.AppendHtml(ri));
        }

        private TagBuilder BuildRadioItem(object item)
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.AddCssClass(RadioItemClass);

            var input = BuildRadioInput(item);
            var label = BuildRadioLabel(item);

            builder.InnerHtml.AppendHtml(input);
            builder.InnerHtml.AppendHtml(label);

            return builder;
        }

        private TagBuilder BuildRadioInput(object item)
        {
            var itemValue = item.GetType().GetProperty(ValueName).GetValue(item).ToString();


            if (string.IsNullOrEmpty(itemValue))
                return new TagBuilder("empty");

            var builder = htmlGenerator.GenerateRadioButton(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                itemValue,
                null,
                null);

            builder.AddCssClass(RadioItemInputClass);

            return builder;
        }

        private TagBuilder BuildRadioLabel(object item)
        {
            var itemDisplay = item.GetType().GetProperty(DisplayName).GetValue(item).ToString();

            if (string.IsNullOrEmpty(itemDisplay))
                return new TagBuilder("empty");

            var builder = htmlGenerator.GenerateLabel(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                itemDisplay,
                null);

            builder.AddCssClass(RadioLabelClass);

            return builder;        
        }
    }
}
