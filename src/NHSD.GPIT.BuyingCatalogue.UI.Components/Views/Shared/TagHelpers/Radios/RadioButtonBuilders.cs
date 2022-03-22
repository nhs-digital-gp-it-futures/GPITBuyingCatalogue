﻿using System.Reflection;
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
            int index,
            string valueName,
            string displayName,
            string hintName)
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.AddCssClass(TagHelperConstants.RadioItemClass);

            var hint = GetRadioHintBuilder(aspFor, item, hintName, index);
            var input = GetRadioInputBuilder(viewcontext, aspFor, htmlGenerator, item, valueName, index, hint);
            var label = GetRadioLabelBuilder(viewcontext, aspFor, htmlGenerator, item, displayName, index);

            builder.InnerHtml
                .AppendHtml(input)
                .AppendHtml(label)
                .AppendHtml(hint);

            return builder;
        }

        public static TagBuilder GetRadioLabelBuilder(
            ViewContext viewContext,
            ModelExpression aspFor,
            IHtmlGenerator htmlGenerator,
            object item,
            string displayName,
            int index)
        {
            var itemText = GetGenericValueFromName(item, displayName);

            return itemText is null
                ? null
                : GetRadioLabelBuilder(viewContext, aspFor, htmlGenerator, itemText.ToString(), index);
        }

        public static TagBuilder GetRadioLabelBuilder(
            ViewContext viewContext,
            ModelExpression aspFor,
            IHtmlGenerator htmlGenerator,
            string display,
            int index)
        {
            return htmlGenerator.GenerateLabel(
                viewContext,
                aspFor.ModelExplorer,
                $"{aspFor.Name}_{index}",
                display,
                new { @class = TagHelperConstants.RadioLabelClass });
        }

        public static TagBuilder GetRadioInputBuilder(
                ViewContext viewContext,
                ModelExpression aspFor,
                IHtmlGenerator htmlGenerator,
                object item,
                string valueName,
                int index,
                TagBuilder hintBuilder = null)
        {
            var itemValue = GetGenericValueFromName(item, valueName);

            return itemValue is null
                ? null
                : GetRadioInputBuilder(viewContext, aspFor, htmlGenerator, itemValue, index, hintBuilder: hintBuilder);
        }

        public static TagBuilder GetRadioInputBuilder(
            ViewContext viewContext,
            ModelExpression aspFor,
            IHtmlGenerator htmlGenerator,
            object value,
            int index,
            bool? isChecked = null,
            TagBuilder hintBuilder = null)
        {
            var builder = htmlGenerator.GenerateRadioButton(
                            viewContext,
                            aspFor.ModelExplorer,
                            aspFor.Name,
                            value,
                            isChecked,
                            new { @class = TagHelperConstants.RadioItemInputClass });

            builder.Attributes["id"] = TagBuilder.CreateSanitizedId($"{aspFor.Name}_{index}", "_");

            if (hintBuilder is not null)
                builder.Attributes[TagHelperConstants.AriaDescribedBy] = hintBuilder.Attributes["id"];

            return builder;
        }

        public static TagBuilder GetRadioHintBuilder(
            ModelExpression aspFor,
            object item,
            string hintName,
            int index)
        {
            if (string.IsNullOrWhiteSpace(hintName))
                return null;

            var itemValue = GetGenericValueFromName(item, hintName);

            return itemValue is null
                ? null
                : GetRadioHintBuilder(aspFor, itemValue, index);
        }

        public static TagBuilder GetRadioHintBuilder(
            ModelExpression aspFor,
            object value,
            int index)
        {
            var builder = new TagBuilder(TagHelperConstants.Div);
            builder.AddCssClass(TagHelperConstants.NhsHint);
            builder.AddCssClass(TagHelperConstants.NhsRadiosHint);

            builder.Attributes["id"] = TagBuilder.CreateSanitizedId($"{aspFor.Name}_{index}-item-hint", "_");

            builder.InnerHtml.Append(value.ToString());

            return builder;
        }

        private static object GetGenericValueFromName(object item, string targetName)
        {
            var propertyInfo = item.GetType().GetProperty(targetName);

            if (propertyInfo is not null)
                return propertyInfo.GetValue(item);

            var methodInfo = item.GetType().GetExtensionMethod(Assembly.GetAssembly(item.GetType()), targetName);

            return methodInfo is not null
                ? methodInfo.Invoke(item, new[] { item })
                : string.Empty;
        }
    }
}
