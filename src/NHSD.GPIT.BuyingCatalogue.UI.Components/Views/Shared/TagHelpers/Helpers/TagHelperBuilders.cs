﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    public static class TagHelperBuilders
    {
        public static void UpdateOutputDiv(
            TagHelperOutput output,
            ModelExpression aspFor,
            ViewContext viewContext,
            TagBuilder htmlContent,
            bool enableCharacterCounter,
            string validationName = null,
            bool isInError = false,
            int defaultMaxLength = 0)
        {
            output.Reinitialize(TagHelperConstants.Div, TagMode.StartTagAndEndTag);

            output.Content.AppendHtml(htmlContent);

            var attributes = new List<TagHelperAttribute>();

            if (!TagHelperFunctions.IsCounterDisabled(aspFor, enableCharacterCounter))
            {
                var maxCharacterLength = TagHelperFunctions.GetMaximumCharacterLength(aspFor) ?? defaultMaxLength;

                attributes.Add(new TagHelperAttribute(TagHelperConstants.DataModule, TagHelperConstants.GovUkCharacterCount));
                attributes.Add(new TagHelperAttribute(TagHelperConstants.DataMaxLength, maxCharacterLength.ToString()));
            }

            attributes.ForEach(a => output.Attributes.Add(a));

            if (TagHelperFunctions.CheckIfModelStateHasErrors(viewContext, aspFor, validationName) || isInError)
            {
                output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, TagHelperConstants.NhsFormGroupError));
            }
        }

        public static TagBuilder GetFormGroupBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);
            builder.AddCssClass(TagHelperConstants.NhsFormGroup);
            return builder;
        }

        public static TagBuilder GetValidationBuilder(ViewContext viewContext, ModelExpression aspFor, IHtmlGenerator htmlGenerator)
        {
            if (!TagHelperFunctions.CheckIfModelStateHasErrors(viewContext, aspFor))
                return null;

            return htmlGenerator.GenerateValidationMessage(
                viewContext,
                aspFor.ModelExplorer,
                aspFor.Name,
                null,
                TagHelperConstants.Span,
                new { @class = TagHelperConstants.NhsErrorMessage });
        }

        public static TagBuilder GetLabelHintBuilder(ModelExpression aspFor, string hintText, string formName = null)
        {
            if ((string.IsNullOrEmpty(aspFor.Name) && string.IsNullOrEmpty(formName))
                || string.IsNullOrEmpty(hintText))
            {
                return null;
            }

            var name = !string.IsNullOrEmpty(aspFor.Name) ? aspFor.Name : formName;

            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.GenerateId($"{name}-hint", "_");
            builder.AddCssClass(TagHelperConstants.NhsHint);

            builder.InnerHtml.Append(hintText);

            return builder;
        }

        public static TagBuilder GetLabelBuilder(
            ViewContext viewContext,
            ModelExpression aspFor,
            IHtmlGenerator htmlGenerator,
            string formName = null,
            string htmlAttributeLabelText = null)
        {
            if ((string.IsNullOrEmpty(aspFor.Name) && string.IsNullOrEmpty(formName))
                || string.IsNullOrEmpty(htmlAttributeLabelText))
            {
                return null;
            }

            return htmlGenerator.GenerateLabel(
                viewContext,
                aspFor.ModelExplorer,
                string.IsNullOrWhiteSpace(aspFor.Name) ? formName : aspFor.Name,
                htmlAttributeLabelText,
                new { @class = TagHelperConstants.NhsLabel });
        }

        public static TagBuilder GetCounterBuilder(ModelExpression aspFor, int defaultMaxLength, bool enableCharacterCounter)
        {
            if (TagHelperFunctions.IsCounterDisabled(aspFor, enableCharacterCounter))
                return null;

            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.MergeAttribute(TagHelperConstants.AriaLive, TagHelperConstants.AriaLivePolite);
            builder.AddCssClass(TagHelperConstants.GovUkHint);
            builder.AddCssClass(TagHelperConstants.GovUkCharacterCountMessage);
            builder.GenerateId($"{aspFor.Name}-info", "_");

            var characterLength = TagHelperFunctions.GetCustomAttribute<StringLengthAttribute>(aspFor)?.MaximumLength ?? defaultMaxLength;

            builder.InnerHtml.Append(string.Format(TagHelperConstants.CharacterCountMessage, characterLength));

            return builder;
        }

        public static TagBuilder GetRadioLabelBuilder(
            ViewContext viewContext,
            ModelExpression aspFor,
            IHtmlGenerator htmlGenerator,
            object item,
            string displayName)
        {
            var itemText = item.GetType().GetProperty(displayName).GetValue(item).ToString();

            if (string.IsNullOrWhiteSpace(itemText))
                return null;

            return GetRadioLabelBuilder(viewContext, aspFor, htmlGenerator, itemText);
        }

        public static TagBuilder GetRadioLabelBuilder(
            ViewContext viewContext,
            ModelExpression aspFor,
            IHtmlGenerator htmlGenerator,
            string display)
        {
            return htmlGenerator.GenerateLabel(
                viewContext,
                aspFor.ModelExplorer,
                aspFor.Name,
                display,
                new { @class = TagHelperConstants.RadioLabelClass });
        }

        public static TagBuilder GetRadioInputBuilder(
                ViewContext viewContext,
                ModelExpression aspFor,
                IHtmlGenerator htmlGenerator,
                object item,
                string valueName)
        {
            var itemValue = item.GetType().GetProperty(valueName).GetValue(item).ToString();

            if (string.IsNullOrWhiteSpace(itemValue))
                return null;

            return GetRadioInputBuilder(viewContext, aspFor, htmlGenerator, itemValue);
        }

        public static TagBuilder GetRadioInputBuilder(
            ViewContext viewContext,
            ModelExpression aspFor,
            IHtmlGenerator htmlGenerator,
            string value,
            bool? isChecked = null)
        {
            return htmlGenerator.GenerateRadioButton(
                viewContext,
                aspFor.ModelExplorer,
                aspFor.Name,
                value,
                isChecked,
                new { @class = TagHelperConstants.RadioItemInputClass });
        }

        public static TagBuilder GetVisuallHiddenSpanClassBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Span);
            builder.AddCssClass(TagHelperConstants.NhsVisuallyHidden);
            builder.InnerHtml.Append(TagHelperConstants.NhsVisuallyHiddenSpanContent);

            return builder;
        }
    }
}
