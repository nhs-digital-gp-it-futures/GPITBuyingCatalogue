using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.DataAttributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ViewsTagHelpers
{
    public static class TagHelperBuilders
    {

        #region Global Builders
        public static TagBuilder GetOuterDivBuilder(ModelExpression For, ViewContext viewContext, bool? DisableCharacterCounter)
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            var attributes = new Dictionary<string, string>
            {
                { TagHelperConstants.DataTestId, TagHelperConstants.SectionTextField},
            };

            if (!TagHelperFunctions.IsCounterDisabled(For, DisableCharacterCounter))
            {
                attributes.Add(TagHelperConstants.DataModule, TagHelperConstants.GovUkCharacterCount);
                attributes.Add(TagHelperConstants.DataMaxLength, TagHelperFunctions.GetMaximumCharacterLength(For).ToString());
            }

            builder.MergeAttributes(attributes);

            var modelState = viewContext.ViewData.ModelState;
            if (!modelState.ContainsKey(For.Name))
            {
                return builder;
            }

            if (TagHelperFunctions.CheckIfModelStateHasErrors(viewContext, For))
            {
                builder.AddCssClass(TagHelperConstants.NhsFormGroupError);
            }

            return builder;
        }

        public static void UpdateOutputDiv(
            TagHelperOutput output,
            ModelExpression For,
            ViewContext viewContext,
            TagBuilder htmlContent,
            string SectionName,
            bool DisableCharacterCounter,
            string validationName = null)
        {

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(htmlContent);

            var attributes = new List<TagHelperAttribute>
            {
                new TagHelperAttribute(TagHelperConstants.DataTestId, SectionName),
            };

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
                output.Attributes.Add(new TagHelperAttribute("class", TagHelperConstants.NhsFormGroupError));
            }
        }

        public static TagBuilder GetFormGroupBuilder(string name)
        {
            var builder = new TagBuilder(TagHelperConstants.Div);
            builder.AddCssClass(TagHelperConstants.NhsFormGroup);
            builder.MergeAttribute(TagHelperConstants.DataTestId, $"question-{name}");
            return builder;
        }

        public static TagBuilder GetOuterTestingDivBuilder(string name)
        {
            var Builder = new TagBuilder(TagHelperConstants.Div);

            Builder.MergeAttribute(TagHelperConstants.DataTestId, $"question-{name}");

            return Builder;
        }

        public static TagBuilder GetInnerTestingDivBuilder(string fieldtype)
        {
            var Builder = new TagBuilder(TagHelperConstants.Div);

            Builder.MergeAttribute(TagHelperConstants.DataTestId, fieldtype);

            return Builder;
        }

        public static TagBuilder GetValidationBuilder(ViewContext viewContext, ModelExpression For, IHtmlGenerator htmlGenerator)
        {
            var builder = htmlGenerator.GenerateValidationMessage(
                viewContext,
                For.ModelExplorer,
                For.Name,
                null,
                TagHelperConstants.Span,
                null);

            builder.AddCssClass(TagHelperConstants.NhsErrorMessage);
            builder.Attributes[TagHelperConstants.DataTestId] = $"{For.Name}-error";

            return builder;
        }

        public static TagBuilder GetLabelHintBuilder(string name, string HintText)
        {
            if (string.IsNullOrEmpty(HintText))
                return new TagBuilder("empty");

            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.GenerateId($"{name}-hint", "");

            builder.AddCssClass(TagHelperConstants.NhsHint);

            builder.InnerHtml.Append(HintText);

            return builder;
        }

        #endregion
        #region Text Area And Text Input Specific Builders
        public static TagBuilder GetLabelBuilder(ViewContext viewContext, ModelExpression For, IHtmlGenerator htmlGenerator, string HtmlAttributeLabelText = null)
        {
            var builder = htmlGenerator.GenerateLabel(
                viewContext,
                For.ModelExplorer,
                For.Name,
                HtmlAttributeLabelText ?? TagHelperFunctions.GetCustomAttribute<LabelTextAttribute>(For)?.Text,
                null);

            builder.AddCssClass(TagHelperConstants.NhsLabel);
            builder.MergeAttribute(TagHelperConstants.DataTestId, $"{For.Name}-label");

            return builder;
        } 

        public static TagBuilder GetCounterBuilder(ModelExpression For, bool? disableCharacterCounter)
        {
            if (TagHelperFunctions.IsCounterDisabled(For, disableCharacterCounter))
                return new TagBuilder("empty");

            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.MergeAttribute(TagHelperConstants.AriaLive, TagHelperConstants.AriaLivePolite);
            builder.AddCssClass(TagHelperConstants.GovUkHint);
            builder.AddCssClass(TagHelperConstants.GovUkCharacterCountMessage);
            builder.GenerateId($"{For.Name}-info", string.Empty);

            builder.InnerHtml.Append(string.Format(TagHelperConstants.CharacterCountMessage, TagHelperFunctions.GetCustomAttribute<StringLengthAttribute>(For).MaximumLength));

            return builder;
        }

        #endregion
        #region Checkbox and Radio Fieldset Builder



        #endregion
        #region Checkbox Specific Builders



        #endregion
    }
}
