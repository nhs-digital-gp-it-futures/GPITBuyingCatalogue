using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.DataAttributes;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ViewsTagHelpers
{
    [HtmlTargetElement(TagHelperName, ParentTag = ParentTagName)]
    public sealed class ValidationCheckboxTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-validation-checkbox";
        public const string ParentTagName = "nhs-checkbox-container";

        private const string NhsCheckboxItems = "nhsuk-checkboxes__item";
        private const string NhsCheckboxInput = "nhsuk-checkboxes__input";
        private const string NhsCheckBoxLabel = "nhsuk-checkboxes__label";

        private readonly IHtmlGenerator htmlGenerator;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        public ValidationCheckboxTagHelper(IHtmlGenerator htmlGenerator)
        {
            this.htmlGenerator = htmlGenerator;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (output is null)
                throw new ArgumentNullException(nameof(output));

            output.Content.Clear();

            var label = GetCheckboxLabelBuilder();
            var input = GetCheckboxInputBuilder();

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", NhsCheckboxItems);

            output.Content.AppendHtml(input);
            output.Content.AppendHtml(label);
        }

        private TagBuilder GetCheckboxLabelBuilder( )
        {
            var labelBuilder = htmlGenerator.GenerateLabel(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                LabelText ?? TagHelperFunctions.GetCustomAttribute<CheckboxAttribute>(For).DisplayText,
                null);

            labelBuilder.AddCssClass(TagHelperConstants.NhsLabel);
            labelBuilder.AddCssClass(NhsCheckBoxLabel);

            return labelBuilder;
        }

        private TagBuilder GetCheckboxInputBuilder()
        {
            var inputBuilder = htmlGenerator.GenerateCheckBox(
            ViewContext,
            For.ModelExplorer,
            For.Name,
            (bool)For.Model,
            null);

            inputBuilder.AddCssClass(NhsCheckboxInput);

            return inputBuilder;
        }
    }
}
