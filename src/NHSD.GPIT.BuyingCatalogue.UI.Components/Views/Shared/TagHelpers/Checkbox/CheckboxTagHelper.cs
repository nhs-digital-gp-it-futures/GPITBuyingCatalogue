﻿using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.DataAttributes;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName, ParentTag = CheckboxContainerTagHelper.TagHelperName)]
    public sealed class CheckboxTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-checkbox";

        private const string HiddenAttributeName = "hidden-input";
        private const string NhsCheckboxItem = "nhsuk-checkboxes__item";
        private const string NhsCheckboxInput = "nhsuk-checkboxes__input";
        private const string NhsCheckboxLabel = "nhsuk-checkboxes__label";

        private readonly IHtmlGenerator htmlGenerator;

        public CheckboxTagHelper(IHtmlGenerator htmlGenerator) => this.htmlGenerator = htmlGenerator;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(HiddenAttributeName)]
        public ModelExpression HiddenFor { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (output is null)
                throw new ArgumentNullException(nameof(output));

            output.Reinitialize(TagHelperConstants.Div, TagMode.StartTagAndEndTag);

            output.Attributes.Add(TagHelperConstants.Class, NhsCheckboxItem);

            var label = GetCheckboxLabelBuilder();
            var input = GetCheckboxInputBuilder();

            output.Content.AppendHtml(input);
            output.Content.AppendHtml(label);

            if (HiddenFor != null)
                output.Content.AppendHtml(GetHiddenInputBuilder());
        }

        private TagBuilder GetCheckboxInputBuilder()
        {
            return htmlGenerator.GenerateCheckBox(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                (bool)For.Model,
                new { @class = NhsCheckboxInput });
        }

        private TagBuilder GetCheckboxLabelBuilder()
        {
            var labelText = LabelText ?? TagHelperFunctions.GetCustomAttribute<CheckboxAttribute>(For)?.DisplayText ?? string.Empty;

            return htmlGenerator.GenerateLabel(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                labelText,
                new { @class = $"{TagHelperConstants.NhsLabel} {NhsCheckboxLabel}" });
        }

        private TagBuilder GetHiddenInputBuilder()
        {
            return htmlGenerator.GenerateHidden(
                ViewContext,
                HiddenFor.ModelExplorer,
                HiddenFor.Name,
                HiddenFor.Model,
                false,
                null);
        }
    }
}
