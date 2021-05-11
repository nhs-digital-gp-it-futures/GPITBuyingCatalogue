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
        public const string IsYesOrNoName = "is-yes-no";


        private const string RadioItemClass = "nhsuk-radios__item";
        private const string RadioItemInputClass = "nhsuk-radios__input";
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

        [HtmlAttributeName(IsYesOrNoName)]
        public bool? IsYesNo { get; set; }

        public RadioButtonsTagHelper(IHtmlGenerator htmlGenerator)
        {
            this.htmlGenerator = htmlGenerator;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!IsYesNo.HasValue && (!Values.Any() || string.IsNullOrEmpty(ValueName) || string.IsNullOrEmpty(DisplayName)))
            {
                output.SuppressOutput();
                return;
            }

            List<TagBuilder> radioItems = IsYesNo == true ? BuildYesNoRadios() : BuildRadiosFromValueList();

            output.Content.Clear();

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, NhsRadios));

            radioItems.ForEach(ri => output.Content.AppendHtml(ri));
        }

        private List<TagBuilder> BuildRadiosFromValueList()
        {
            return (from item in Values
                    select BuildRadioItem(item)).ToList();
        }


        private List<TagBuilder> BuildYesNoRadios()
        {
            var yesBuilder = new TagBuilder(TagHelperConstants.Div);

            yesBuilder.AddCssClass(RadioItemClass);

            yesBuilder.InnerHtml.AppendHtml(BuildRadioInput("Yes"));
            yesBuilder.InnerHtml.AppendHtml(BuildRadioLabel("Yes"));

            var noBuilder = new TagBuilder(TagHelperConstants.Div);

            noBuilder.AddCssClass(RadioItemClass);

            noBuilder.InnerHtml.AppendHtml(BuildRadioInput("No"));
            noBuilder.InnerHtml.AppendHtml(BuildRadioLabel("No"));

            return new List<TagBuilder>
            {
                yesBuilder,
                noBuilder
            };

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

            return htmlGenerator.GenerateRadioButton(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                itemValue,
                null,
                new { @class = RadioItemInputClass });
        }

        private TagBuilder BuildRadioInput(string value)
        {
            var isChecked = value == "Yes" && For.Model?.ToString() == "Yes" || value == "No" && For.Model?.ToString() != "Yes";

            return GenerateRadioButton(isChecked, value, RadioItemInputClass);
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
                new {@class = RadioLabelClass });

            return builder;        
        }

        private TagBuilder BuildRadioLabel(string display)
        {

            var builder = htmlGenerator.GenerateLabel(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                display,
                new { @class = RadioLabelClass });

            return builder;
        }
        
        private TagBuilder GenerateRadioButton(bool isChecked, string value, string cssClass)
        {
            var builder = new TagBuilder(TagHelperConstants.Input);

            builder.GenerateId(For.Name, "");

            builder.MergeAttribute("name", For.Name);
            builder.MergeAttribute(TagHelperConstants.Type, "radio");
            builder.MergeAttribute("value", value);

            if (isChecked)
                builder.MergeAttribute("checked", "");

            builder.AddCssClass(cssClass);

            return builder;
        }



    }
}
