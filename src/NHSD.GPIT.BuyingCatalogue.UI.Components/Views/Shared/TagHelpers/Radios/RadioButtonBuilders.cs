using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Radios
{
    public static class RadioButtonBuilders
    {
        public static void UpdateRadioContainerOutput(TagHelperOutput output, TagHelperContext context, ConditionalContext conditionalContext = null)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(
                TagHelperConstants.Class,
                TagHelperFunctions.BuildCssClassForConditionalContentOutput(
                    context,
                    conditionalContext,
                    TagHelperConstants.NhsRadios,
                    TagHelperConstants.NhsRadiosParentContainerConditional)));
        }

        public static TagBuilder BuildRadioItem(
            ViewContext viewcontext,
            ModelExpression aspFor,
            IHtmlGenerator htmlGenerator,
            object item,
            string valueName,
            string displayName)
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.AddCssClass(TagHelperConstants.RadioItemClass);

            var input = GetRadioInputBuilder(viewcontext, aspFor, htmlGenerator, item, valueName);
            var label = GetRadioLabelBuilder(viewcontext, aspFor, htmlGenerator, item, displayName, valueName);

            builder.InnerHtml.AppendHtml(input);
            builder.InnerHtml.AppendHtml(label);

            return builder;
        }

        public static TagBuilder GetRadioLabelBuilder(
            ViewContext viewContext,
            ModelExpression aspFor,
            IHtmlGenerator htmlGenerator,
            object item,
            string displayName,
            string valueName)
        {
            var itemText = GetGenericValueFromName(item, displayName);

            var itemValue = GetGenericValueFromName(item, valueName);

            return string.IsNullOrWhiteSpace(itemText)
                ? null
                : GetRadioLabelBuilder(viewContext, aspFor, htmlGenerator, itemText, itemValue);
        }

        public static TagBuilder GetRadioLabelBuilder(
            ViewContext viewContext,
            ModelExpression aspFor,
            IHtmlGenerator htmlGenerator,
            string display,
            string value)
        {
            return htmlGenerator.GenerateLabel(
                viewContext,
                aspFor.ModelExplorer,
                $"{aspFor.Name}_{value}",
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
            var itemValue = GetGenericValueFromName(item, valueName);

            return string.IsNullOrWhiteSpace(itemValue)
                ? null
                : GetRadioInputBuilder(viewContext, aspFor, htmlGenerator, itemValue);
        }

        public static TagBuilder GetRadioInputBuilder(
            ViewContext viewContext,
            ModelExpression aspFor,
            IHtmlGenerator htmlGenerator,
            string value,
            bool? isChecked = null)
        {
            var builder = htmlGenerator.GenerateRadioButton(
                            viewContext,
                            aspFor.ModelExplorer,
                            aspFor.Name,
                            value,
                            isChecked,
                            new { @class = TagHelperConstants.RadioItemInputClass });

            builder.Attributes["id"] = TagBuilder.CreateSanitizedId($"{aspFor.Name}_{value}", "_");

            return builder;
        }

        private static string GetGenericValueFromName(object item, string targetName)
        {
            var propertyInfo = item.GetType().GetProperty(targetName);

            if (propertyInfo is not null)
                return propertyInfo.GetValue(item)?.ToString();

            var methodInfo = item.GetType().GetExtensionMethod(Assembly.GetAssembly(item.GetType()), targetName);

            return methodInfo is not null
                ? methodInfo.Invoke(item, new[] { item })?.ToString()
                : string.Empty;
        }
    }
}
