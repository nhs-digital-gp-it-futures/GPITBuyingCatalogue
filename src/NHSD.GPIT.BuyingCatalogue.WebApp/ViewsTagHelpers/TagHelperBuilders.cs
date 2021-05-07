using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NHSD.GPIT.BuyingCatalogue.WebApp.DataAttributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ViewsTagHelpers
{
    public static class TagHelperBuilders
    {
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

        public static TagBuilder GetFormGroupBuilder(ModelExpression For)
        {
            var builder = new TagBuilder(TagHelperConstants.Div);
            builder.AddCssClass(TagHelperConstants.NhsFormGroup);
            builder.MergeAttribute(TagHelperConstants.DataTestId, $"question-{For.Name}");
            return builder;
        }

        public static TagBuilder GetOuterTestingDivBuilder(ModelExpression For)
        {
            var Builder = new TagBuilder(TagHelperConstants.Div);

            Builder.MergeAttribute(TagHelperConstants.DataTestId, $"question-{For.Name}");

            return Builder;
        }

        public static TagBuilder GetInnerTestingDivBuilder()
        {
            var Builder = new TagBuilder(TagHelperConstants.Div);

            Builder.MergeAttribute(TagHelperConstants.DataTestId, TagHelperConstants.TextFieldInput);

            return Builder;
        }

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

        public static TagBuilder GetLabelHintBuilder(ModelExpression For, string HtmlAttributeLabelHint = null)
        {
            var hintText = HtmlAttributeLabelHint ?? TagHelperFunctions.GetCustomAttribute<LabelHintAttribute>(For)?.Text;

            if (string.IsNullOrEmpty(hintText))
                return new TagBuilder("empty");

            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.GenerateId($"{For.Name}-hint", "");

            builder.AddCssClass(TagHelperConstants.NhsHint);
            builder.AddCssClass($"{TagHelperConstants.NhsUMarginBottom}-3");

            builder.InnerHtml.Append(hintText);

            return builder;
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

        public static  TagBuilder GetCounterBuilder(ModelExpression For, bool? disableCharacterCounter)
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


    }
}
