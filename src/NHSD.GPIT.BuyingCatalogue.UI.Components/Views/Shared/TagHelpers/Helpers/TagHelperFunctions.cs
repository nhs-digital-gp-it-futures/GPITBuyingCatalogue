using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.DataAttributes;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    public static class TagHelperFunctions
    {
        public static int? GetMaximumCharacterLength(ModelExpression aspFor)
        {
            return GetCustomAttribute<StringLengthAttribute>(aspFor)?.MaximumLength;
        }

        public static T GetCustomAttribute<T>(ModelExpression aspFor)
            where T : Attribute
        {
            return aspFor?.Metadata?
            .ContainerType?
            .GetProperty(aspFor.Name[(aspFor.Name.LastIndexOf('.') + 1)..])?
            .GetCustomAttribute<T>();
        }

        public static IEnumerable<T> GetCustomAttributes<T>(ModelExpression aspFor)
            where T : Attribute
        {
            return aspFor?.Metadata?
            .ContainerType?
            .GetProperty(aspFor.Name[(aspFor.Name.LastIndexOf('.') + 1)..])?
            .GetCustomAttributes<T>();
        }

        public static bool IsCounterDisabled(ModelExpression aspFor, bool htmlAttributeEnableCharacterCounter)
        {
            return htmlAttributeEnableCharacterCounter == false
                   || GetCustomAttribute<PasswordAttribute>(aspFor) != null;
        }

        public static bool CheckIfModelStateHasAnyErrors(ViewContext viewContext, params ModelExpression[] modelExpressions)
        {
            return modelExpressions.Any(me => CheckIfModelStateHasErrors(viewContext, me));
        }

        public static bool CheckIfModelStateHasErrors(ViewContext viewContext, ModelExpression aspFor, string validationName = null)
        {
            var modelState = viewContext.ViewData?.ModelState;
            return !(modelState?[aspFor?.Name ?? validationName] is null) && modelState[aspFor?.Name ?? validationName].Errors.Any();
        }

        public static string GetErrorMessageFromModelState(ViewContext viewContext, ModelExpression aspFor, string validationName = null)
        {
            var modelState = viewContext.ViewData?.ModelState;

            if (modelState?[aspFor?.Name ?? validationName] is not null && modelState[aspFor?.Name ?? validationName].Errors.Any())
                return modelState?[aspFor?.Name ?? validationName].Errors.FirstOrDefault().ErrorMessage;

            return string.Empty;
        }

        public static string GetModelKebabNameFromFor(ModelExpression aspFor)
        {
            string name = aspFor.Model is null || !string.IsNullOrWhiteSpace(aspFor.Name) ? aspFor.Name : aspFor.Model.GetType().Name;

            // removes the word Model from the end of the Model, e.g SolutionDescriptionModel becomes SolutionDescription
            if (name.Contains("Model", StringComparison.OrdinalIgnoreCase))
                name = name.Replace("Model", string.Empty);

            var pattern = new Regex(@"[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+");
            return string.Join("-", pattern.Matches(name)).ToLower();
        }

        public static void TellParentTagIfThisTagIsInError(ViewContext viewContext, TagHelperContext context, ModelExpression model, string validationName = null)
        {
            if (CheckIfModelStateHasErrors(viewContext, model, validationName))
            {
                if (!context.Items.TryGetValue(typeof(ParentChildContext), out object pChildContext))
                    return;

                ParentChildContext parentChildContext = (ParentChildContext)pChildContext;

                if (parentChildContext is not null)
                {
                    if (!parentChildContext.ChildInError)
                    {
                        parentChildContext.ChildInError = true;
                        parentChildContext.ErrorMessage = GetErrorMessageFromModelState(viewContext, model);
                    }
                }
            }
        }

        public static void TellParentThisHasConditionalChildContent(TagHelperContext context)
        {
            if (context.Items.TryGetValue(TagHelperConstants.ConditionalContextName, out object value))
            {
                (value as ConditionalContext).ContainsConditionalContent = true;
            }
        }

        public static void ProcessOutputForConditionalContent(
            TagHelperOutput output,
            TagHelperContext context,
            TagBuilder input,
            TagHelperContent childContent,
            IEnumerable<string> classes,
            bool isSelected = false)
        {
            var childContainer = TagHelperBuilders.GetChildContentConditionalBuilder(input, classes);

            childContainer.InnerHtml.AppendHtml(childContent);

            output.PostElement.AppendHtml(childContainer);

            childContainer.Attributes.TryGetValue("id", out string containerId);

            input.MergeAttribute(TagHelperConstants.AriaControls, containerId);
            input.MergeAttribute(TagHelperConstants.AriaExpanded, isSelected.ToString());

            TellParentThisHasConditionalChildContent(context);
        }

        public static string BuildCssClassForConditionalContentOutput(TagHelperContext context, ConditionalContext conditionalContext, string baseClass, string additionalClass)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(baseClass);
            if (ShouldIncludeClassForConditionalContent(context, conditionalContext))
            {
                stringBuilder.Append(' ');
                stringBuilder.Append(additionalClass);
            }

            return stringBuilder.ToString();
        }

        public static bool ShouldIncludeClassForConditionalContent(TagHelperContext context, ConditionalContext conditionalContext)
        {
            // only apply to self if this is the parent container
            if (!context.Items.TryGetValue(TagHelperConstants.ConditionalContextName, out object value))
                return false;

            if (!((ConditionalContext)value).ContainsConditionalContent)
                return false;

            if (conditionalContext is null)
                return false;

            return true;
        }
    }
}
