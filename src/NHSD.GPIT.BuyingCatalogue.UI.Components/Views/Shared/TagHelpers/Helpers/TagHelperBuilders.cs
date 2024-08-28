using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
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
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            var attributes = new List<TagHelperAttribute>();

            if (!TagHelperFunctions.IsCounterDisabled(aspFor, enableCharacterCounter))
            {
                var maxCharacterLength = TagHelperFunctions.GetMaximumCharacterLength(aspFor) ?? defaultMaxLength;

                attributes.Add(new TagHelperAttribute(TagHelperConstants.DataModule, TagHelperConstants.GovUkCharacterCount));
                attributes.Add(new TagHelperAttribute(TagHelperConstants.DataMaxLength, maxCharacterLength.ToString()));
            }

            if (TagHelperFunctions.CheckIfModelStateHasErrors(viewContext, aspFor, validationName) || isInError)
                htmlContent.AddCssClass(TagHelperConstants.NhsFormGroupError);

            if (attributes.Count > 0)
            {
                attributes.ForEach(a => output.Attributes.Add(a));
            }
            else
            {
                foreach (var attribute in output.Attributes)
                {
                    if (attribute.Name.Equals("class", StringComparison.OrdinalIgnoreCase))
                        htmlContent.AddCssClass(attribute.Value.ToString());
                    else
                        htmlContent.Attributes.Add(new KeyValuePair<string, string>(attribute.Name, attribute.Value.ToString()));
                }

                output.TagName = string.Empty;
            }

            output.Content.AppendHtml(htmlContent);
        }

        public static TagBuilder GetFormGroupBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);
            builder.AddCssClass(TagHelperConstants.NhsFormGroup);
            return builder;
        }

        public static TagBuilder GetValidationBuilder(ViewContext viewContext, ModelExpression aspFor, IHtmlGenerator htmlGenerator, bool containsHiddenContent = false)
        {
            if (!TagHelperFunctions.CheckIfModelStateHasErrors(viewContext, aspFor))
                return null;

            var builder = htmlGenerator.GenerateValidationMessage(
                viewContext,
                aspFor.ModelExplorer,
                aspFor.Name,
                null,
                TagHelperConstants.Span,
                new { @class = TagHelperConstants.NhsErrorMessage });

            builder.GenerateId($"{aspFor.Name}-error", "_");

            if (containsHiddenContent)
            {
                var message = TagHelperFunctions.GetInnerHtml(builder);
                var regex = new Regex("&lt;(.*?)&gt;");
                var newMessage = regex.Replace(message, string.Empty);
                var hiddenMessage = regex.Match(message).Groups[1].Value;
                var hiddenSpan = GetHiddenContentBuilder(hiddenMessage);
                builder.InnerHtml.Clear();
                builder.InnerHtml.AppendHtml(newMessage).AppendHtml(hiddenSpan);
            }

            return builder;
        }

        public static TagBuilder GetLabelHintBuilder(ModelExpression aspFor, string hintText, string formName = null)
        {
            if ((string.IsNullOrEmpty(aspFor?.Name) && string.IsNullOrEmpty(formName))
                || string.IsNullOrEmpty(hintText))
                return null;

            var name = !string.IsNullOrEmpty(aspFor?.Name) ? aspFor.Name : formName;

            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.GenerateId($"{name}-hint", "_");
            builder.AddCssClass(TagHelperConstants.NhsHint);

            builder.InnerHtml.AppendHtml(hintText);

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
                return null;

            return htmlGenerator.GenerateLabel(
                viewContext,
                aspFor.ModelExplorer,
                string.IsNullOrWhiteSpace(aspFor.Name) ? formName : aspFor.Name,
                htmlAttributeLabelText,
                new { @class = $"{TagHelperConstants.NhsLabel} {TagHelperConstants.BoldLabel}" });
        }

        public static TagBuilder GetInlineLabelBuilder(
            ViewContext viewContext,
            ModelExpression aspFor,
            IHtmlGenerator htmlGenerator,
            string formName = null,
            string htmlAttributeLabelText = null)
        {
            if ((string.IsNullOrEmpty(aspFor.Name) && string.IsNullOrEmpty(formName))
                || string.IsNullOrEmpty(htmlAttributeLabelText))
                return null;

            return htmlGenerator.GenerateLabel(
                viewContext,
                aspFor.ModelExplorer,
                string.IsNullOrWhiteSpace(aspFor.Name) ? formName : aspFor.Name,
                htmlAttributeLabelText,
                new { @class = $"{TagHelperConstants.NhsLabel} {TagHelperConstants.NhsInLineLabel}" });
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

        public static TagBuilder GetVisuallHiddenSpanClassBuilder()
        {
            return GetHiddenContentBuilder(TagHelperConstants.NhsVisuallyHiddenSpanContent);
        }

        public static TagBuilder GetChildContentConditionalBuilder(TagBuilder input, IEnumerable<string> classes)
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            foreach (var cssClass in classes)
                builder.AddCssClass(cssClass);

            input.Attributes.TryGetValue("id", out string id);

            builder.GenerateId($"conditional-{id}", "_");

            return builder;
        }

        public static TagBuilder GetSelectListBuilder(
            IHtmlGenerator htmlGenerator,
            ViewContext viewContext,
            ModelExpression aspFor,
            SelectList items,
            string defaultValue,
            bool? allowMultiple = null,
            bool? useDefaultValue = true)
        {
            const string nhsSelectClass = "nhsuk-select";

            return htmlGenerator.GenerateSelect(
                viewContext,
                aspFor.ModelExplorer,
                useDefaultValue.GetValueOrDefault() ? defaultValue : null,
                aspFor.Name,
                items,
                allowMultiple == true,
                new { @class = nhsSelectClass });
        }

        public static TagBuilder GetHiddenContentBuilder(string content)
        {
            var builder = new TagBuilder(TagHelperConstants.Span);
            builder.AddCssClass(TagHelperConstants.NhsVisuallyHidden);
            builder.InnerHtml.Append(content);

            return builder;
        }
    }
}
