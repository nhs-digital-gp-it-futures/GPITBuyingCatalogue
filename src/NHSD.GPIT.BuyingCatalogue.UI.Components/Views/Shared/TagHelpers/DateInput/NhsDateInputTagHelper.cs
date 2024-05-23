using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName, ParentTag = FieldSetFormLabelTagHelper.TagHelperName)]
    public sealed class NhsDateInputTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-date-input";
        public const string DayName = "day";
        public const string MonthName = "month";
        public const string YearName = "year";

        private const string DateInputItemClass = "nhsuk-date-input__item";
        private const string DateInputLabelClass = "nhsuk-date-input__label";
        private const string DateInputWidth2Class = "nhsuk-input--width-2";
        private const string DateInputWidth4Class = "nhsuk-input--width-4";
        private const string DateInputInputClass = "nhsuk-date-input__input";
        private const string DateInputClass = "nhsuk-date-input";

        private readonly IHtmlGenerator htmlGenerator;

        public NhsDateInputTagHelper(IHtmlGenerator htmlGenerator)
        {
            this.htmlGenerator = htmlGenerator;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(DayName)]
        public ModelExpression Day { get; set; }

        [HtmlAttributeName(MonthName)]
        public ModelExpression Month { get; set; }

        [HtmlAttributeName(YearName)]
        public ModelExpression Year { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            var day = BuildInputItem(Day, "Day", DateInputWidth2Class);
            var month = BuildInputItem(Month, "Month", DateInputWidth2Class);
            var year = BuildInputItem(Year, "Year", DateInputWidth4Class, removeRightMargin: true);

            var id = TagBuilder.CreateSanitizedId(TagHelperFunctions.GetModelKebabNameFromFor(For), "_");

            List<TagHelperAttribute> attributes = new List<TagHelperAttribute>
            {
                new(TagHelperConstants.Id, id),
                new(TagHelperConstants.Class, DateInputClass),
                new(TagHelperConstants.Style, "white-space:nowrap;"),
            };

            attributes.ForEach(output.Attributes.Add);

            output.Content
                .AppendHtml(day)
                .AppendHtml(month)
                .AppendHtml(year);

            TagHelperFunctions.TellParentTagIfThisTagIsInError(ViewContext, context, Day);
            TagHelperFunctions.TellParentTagIfThisTagIsInError(ViewContext, context, Month);
            TagHelperFunctions.TellParentTagIfThisTagIsInError(ViewContext, context, Year);
        }

        private static TagBuilder BuildDateInputContainerItem(bool removeRightMargin)
        {
            var inputItem = new TagBuilder(TagHelperConstants.Div);

            inputItem.AddCssClass(DateInputItemClass);

            if (removeRightMargin)
                inputItem.MergeAttribute("style", "margin-right:0");

            return inputItem;
        }

        private TagBuilder BuildInputItem(
            ModelExpression modelExpression,
            string labelText,
            string selectedWidthClass,
            bool removeRightMargin = false)
        {
            var item = BuildDateInputContainerItem(removeRightMargin);
            var label = BuildDateLabel(modelExpression, labelText);
            var input = BuildDateInput(modelExpression, selectedWidthClass);

            item.InnerHtml
                .AppendHtml(label)
                .AppendHtml(input);

            return item;
        }

        private TagBuilder BuildDateLabel(ModelExpression modelExpression, string labelText)
        {
            return htmlGenerator.GenerateLabel(
                ViewContext,
                modelExpression.ModelExplorer,
                modelExpression.Name,
                labelText,
                new { @class = $"{TagHelperConstants.NhsLabel} {DateInputLabelClass} {TagHelperConstants.BoldLabel}" });
        }

        private TagBuilder BuildDateInput(ModelExpression modelExpression, string widthClass)
        {
            var builder = htmlGenerator.GenerateTextBox(
                ViewContext,
                modelExpression.ModelExplorer,
                modelExpression.Name,
                modelExpression.Model,
                null,
                new
                {
                    @class = $"{TagHelperConstants.NhsInput} {DateInputInputClass} {widthClass}",
                    pattern = "[0-9]*",
                    inputmode = "numeric",
                });

            if (TagHelperFunctions.CheckIfModelStateHasErrors(ViewContext, Day)
                || TagHelperFunctions.CheckIfModelStateHasErrors(ViewContext, Month)
                || TagHelperFunctions.CheckIfModelStateHasErrors(ViewContext, Year))
            {
                builder.AddCssClass(TagHelperConstants.NhsValidationInputError);
            }

            return builder;
        }
    }
}
