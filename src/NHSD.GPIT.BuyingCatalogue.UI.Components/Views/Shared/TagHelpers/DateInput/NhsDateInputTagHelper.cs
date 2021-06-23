﻿using System;
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
        private readonly IHtmlHelper htmlHelper;

        public NhsDateInputTagHelper(IHtmlGenerator htmlGenerator, IHtmlHelper htmlHelper)
        {
            this.htmlGenerator = htmlGenerator;
            this.htmlHelper = htmlHelper;
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

            var day = BuildInputItem(Day, "Day", DateInputWidth2Class, false);
            var month = BuildInputItem(Month, "Month", DateInputWidth2Class, false);
            var year = BuildInputItem(Year, "Year", DateInputWidth4Class, true);

            (htmlHelper as IViewContextAware).Contextualize(ViewContext);
            var id = htmlHelper.GenerateIdFromName(TagHelperFunctions.GetModelKebabNameFromFor(For));

            List<TagHelperAttribute> attributes = new List<TagHelperAttribute>
            {
                new TagHelperAttribute(TagHelperConstants.Id, id),
                new TagHelperAttribute(TagHelperConstants.Class, DateInputClass),
            };

            attributes.ForEach(a => output.Attributes.Add(a));

            output.Content
                .AppendHtml(day)
                .AppendHtml(month)
                .AppendHtml(year);

            TagHelperFunctions.TellParentTagIfThisTagIsInError(ViewContext, context, Day);
        }

        private static TagBuilder BuildDateInputContainerItem(bool removeRightMargin)
        {
            var inputitem = new TagBuilder(TagHelperConstants.Div);

            inputitem.AddCssClass(DateInputItemClass);

            if (removeRightMargin)
                inputitem.MergeAttribute("style", "margin-right:0");

            return inputitem;
        }

        private TagBuilder BuildInputItem(ModelExpression modelExpression, string labelText, string selectedWidthClass, bool removeRightMargin)
        {
            var item = BuildDateInputContainerItem(removeRightMargin);
            var label = BuildDateLabel(modelExpression, labelText);
            var input = BuildDateInput(modelExpression, selectedWidthClass);

            item.InnerHtml.AppendHtml(label);
            item.InnerHtml.AppendHtml(input);

            return item;
        }

        private TagBuilder BuildDateLabel(ModelExpression modelExpression, string labelText)
        {
            return htmlGenerator.GenerateLabel(
                ViewContext,
                modelExpression.ModelExplorer,
                modelExpression.Name,
                labelText,
                new { @class = $"{TagHelperConstants.NhsLabel} {DateInputLabelClass}" });
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

            if (TagHelperFunctions.CheckIfModelStateHasErrors(ViewContext, Day))
            {
                builder.AddCssClass(TagHelperConstants.NhsValidationInputError);
            }

            return builder;
        }
    }
}
