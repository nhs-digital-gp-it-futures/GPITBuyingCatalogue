using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.DataAttributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ViewsTagHelpers
{
    public static class TagHelperBuilders
    {

        #region Global Builders
        public static void UpdateOutputDiv(
            TagHelperOutput output,
            ModelExpression For,
            ViewContext viewContext,
            TagBuilder htmlContent,
            bool? DisableCharacterCounter,
            string validationName = null)
        {

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(htmlContent);

            var attributes = new List<TagHelperAttribute>();

            if (!TagHelperFunctions.IsCounterDisabled(For, DisableCharacterCounter))
            {
                attributes.Add(new TagHelperAttribute(TagHelperConstants.DataModule, TagHelperConstants.GovUkCharacterCount));
                attributes.Add(new TagHelperAttribute(TagHelperConstants.DataMaxLength, TagHelperFunctions.GetMaximumCharacterLength(For).ToString()));
            }

            attributes.ForEach(a => output.Attributes.Add(a));

            var modelState = viewContext.ViewData.ModelState;
            if (!modelState.ContainsKey(For?.Name ?? validationName))
                return;

            if (TagHelperFunctions.CheckIfModelStateHasErrors(viewContext, For))
            {
                output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, TagHelperConstants.NhsFormGroupError));
            }
        }

        public static TagBuilder GetFormGroupBuilder(string name)
        {
            var builder = new TagBuilder(TagHelperConstants.Div);
            builder.AddCssClass(TagHelperConstants.NhsFormGroup);
            return builder;
        }

        public static TagBuilder GetValidationBuilder(ViewContext viewContext, ModelExpression For, IHtmlGenerator htmlGenerator)
        {
            return htmlGenerator.GenerateValidationMessage(
                viewContext,
                For.ModelExplorer,
                For.Name,
                null,
                TagHelperConstants.Span,
                new { @class = TagHelperConstants.NhsErrorMessage });
        }

        public static TagBuilder GetLabelHintBuilder(ModelExpression For, string HintText, string FormName = null, bool? HtmlAttributeDisableLabelAndHint = null)
        {
            if (string.IsNullOrEmpty(For.Name)
                && string.IsNullOrEmpty(FormName)
                || HtmlAttributeDisableLabelAndHint.HasValue
                && HtmlAttributeDisableLabelAndHint.Value
                || TagHelperFunctions.GetCustomAttribute<DisableLabelAndHintAttribute>(For) != null
                || string.IsNullOrEmpty(HintText)
                && string.IsNullOrEmpty(TagHelperFunctions.GetCustomAttribute<LabelHintAttribute>(For)?.Text))
            {
                return new TagBuilder("empty");
            }

            var name = !string.IsNullOrEmpty(For.Name) ? For.Name : FormName;

            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.GenerateId($"{name}-hint", "_");

            builder.AddCssClass(TagHelperConstants.NhsHint);

            builder.InnerHtml.Append(HintText);

            return builder;
        }

        #endregion
        #region Text Area And Text Input Specific Builders
        public static TagBuilder GetLabelBuilder(
            ViewContext viewContext,
            ModelExpression For,
            IHtmlGenerator htmlGenerator,
            string FormName = null,
            string HtmlAttributeLabelText = null,
            bool? HtmlAttributeDisableLabelAndHint = null)
        {
            if (string.IsNullOrEmpty(For.Name)
                && string.IsNullOrEmpty(FormName)
                || HtmlAttributeDisableLabelAndHint.HasValue
                && HtmlAttributeDisableLabelAndHint.Value
                || TagHelperFunctions.GetCustomAttribute<DisableLabelAndHintAttribute>(For) != null
                || string.IsNullOrEmpty(HtmlAttributeLabelText)
                && string.IsNullOrEmpty(TagHelperFunctions.GetCustomAttribute<LabelTextAttribute>(For)?.Text))
            {
                return new TagBuilder("empty");
            }

            var name = !string.IsNullOrEmpty(For.Name) ? For.Name : FormName;

            return htmlGenerator.GenerateLabel(
                viewContext,
                For.ModelExplorer,
                name,
                HtmlAttributeLabelText ?? TagHelperFunctions.GetCustomAttribute<LabelTextAttribute>(For)?.Text,
                new { @class = TagHelperConstants.NhsLabel });
        } 

        public static TagBuilder GetCounterBuilder(ModelExpression For, bool? disableCharacterCounter)
        {
            if (TagHelperFunctions.IsCounterDisabled(For, disableCharacterCounter))
                return new TagBuilder("empty");

            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.MergeAttribute(TagHelperConstants.AriaLive, TagHelperConstants.AriaLivePolite);
            builder.AddCssClass(TagHelperConstants.GovUkHint);
            builder.AddCssClass(TagHelperConstants.GovUkCharacterCountMessage);
            builder.GenerateId($"{For.Name}-info", "_");

            builder.InnerHtml.Append(string.Format(TagHelperConstants.CharacterCountMessage, TagHelperFunctions.GetCustomAttribute<StringLengthAttribute>(For).MaximumLength));

            return builder;
        }
        #endregion
    }
}
