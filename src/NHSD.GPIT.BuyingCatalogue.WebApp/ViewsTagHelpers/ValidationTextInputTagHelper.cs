using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.DataAttributes;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ViewsTagHelpers
{
    [HtmlTargetElement(TagHelperConstants.Div, Attributes = TagHelperName)]
    public sealed class ValidationTextInputTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-validation-input";

        private readonly IHtmlGenerator htmlGenerator;

        public ValidationTextInputTagHelper(IHtmlGenerator htmlGenerator)
        {
            this.htmlGenerator = htmlGenerator;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelHintName)]
        public string LabelHint { get; set; }

        [HtmlAttributeName(TagHelperConstants.DisableCharacterCounterName)]
        public bool? DisableCharacterCounter { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            output.Content.Clear();

            var outerDiv = TagHelperBuilders.GetOuterDivBuilder(For,ViewContext,DisableCharacterCounter);
            var outerTesting = TagHelperBuilders.GetOuterTestingDivBuilder(For);
            var innerTesting = TagHelperBuilders.GetInnerTestingDivBuilder();
            var formGroup = TagHelperBuilders.GetFormGroupBuilder(For);
            var label = TagHelperBuilders.GetLabelBuilder(ViewContext,For,htmlGenerator, LabelText);
            var hint = TagHelperBuilders.GetLabelHintBuilder(For,LabelHint);
            var validation = TagHelperBuilders.GetValidationBuilder(ViewContext,For,htmlGenerator);
            var input = GetInputBuilder();
            var counter = TagHelperBuilders.GetCounterBuilder(For,DisableCharacterCounter);

            formGroup.InnerHtml.AppendHtml(label);
            formGroup.InnerHtml.AppendHtml(hint);
            formGroup.InnerHtml.AppendHtml(validation);
            formGroup.InnerHtml.AppendHtml(input);
            formGroup.InnerHtml.AppendHtml(counter);
            innerTesting.InnerHtml.AppendHtml(formGroup);
            outerTesting.InnerHtml.AppendHtml(innerTesting);
            outerDiv.InnerHtml.AppendHtml(outerTesting);

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(outerDiv);
        }

        private TagBuilder GetInputBuilder()
        {
            var builder = htmlGenerator.GenerateTextBox(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                For.Model,
                null,
                null);

            if (TagHelperFunctions.GetCustomAttributes<PasswordAttribute>(For)?.Any() == true)
                builder.MergeAttribute(TagHelperConstants.Type, "password");

            if (!TagHelperFunctions.IsCounterDisabled(For, DisableCharacterCounter))
                 builder.AddCssClass(TagHelperConstants.GovUkJsCharacterCount);

            if (TagHelperFunctions.CheckIfModelStateHasErrors(ViewContext, For))
                builder.AddCssClass(TagHelperConstants.NhsValidationInputError);

            builder.AddCssClass(TagHelperConstants.NhsInput);

            builder.MergeAttribute(TagHelperConstants.AriaDescribedBy, $"{For.Name}-info {For.Name}-summary");

            builder.MergeAttribute(TagHelperConstants.DataTestId, $"{For.Name}-input");

            return builder;
        }
    }
}
