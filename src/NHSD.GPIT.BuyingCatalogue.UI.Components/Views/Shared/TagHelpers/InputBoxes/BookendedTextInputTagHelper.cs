using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.InputBoxes
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class BookendedTextInputTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-bookended-input";

        private const string PrefixName = "Prefix";
        private const string SuffixName = "Suffix";

        private const int DefaultMaxLength = 10;

        private const string TextInputWidthClass = "nhsuk-input--width-5";
        private const string GovUkInputPrefixClass = "govuk-input__prefix";
        private const string GovUkInputSuffixClass = "govuk-input__suffix";
        private const string GovUkInputWrapperClass = "govuk-input__wrapper";
        private const string GovUkFormGroup = "govuk-form-group";

        private readonly IHtmlGenerator htmlGenerator;

        public BookendedTextInputTagHelper(IHtmlGenerator htmlGenerator)
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

        [HtmlAttributeName(PrefixName)]
        public string PrefixText { get; set; }

        [HtmlAttributeName(SuffixName)]
        public string SuffixText { get; set; }

        [HtmlAttributeName(TagHelperConstants.CharacterCountName)]
        public bool EnableCharacterCounter { get; set; } = false;

        [HtmlAttributeName(TagHelperConstants.DisableLabelAndHint)]
        public bool? DisableLabelAndHint { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (output is null)
                throw new ArgumentNullException(nameof(output));

            var formGroup = GetGovFormGroupBuilder();
            var label = TagHelperBuilders.GetLabelBuilder(ViewContext, For, htmlGenerator, null, LabelText, DisableLabelAndHint);
            var hint = TagHelperBuilders.GetLabelHintBuilder(For, LabelHint, null, DisableLabelAndHint);
            var validation = TagHelperBuilders.GetValidationBuilder(ViewContext, For, htmlGenerator);
            var inputWrapper = GetInputWrapper();

            var input = GetInputBuilder();
            var prefix = GetPrefixBuilder();
            var suffix = GetSuffixBuilder();

            inputWrapper.InnerHtml
                .AppendHtml(prefix)
                .AppendHtml(input)
                .AppendHtml(suffix);

            formGroup.InnerHtml
                .AppendHtml(label)
                .AppendHtml(hint)
                .AppendHtml(validation)
                .AppendHtml(inputWrapper);

            TagHelperBuilders.UpdateOutputDiv(output, For, ViewContext, formGroup, EnableCharacterCounter);
        }

        private static TagBuilder GetInputWrapper()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);
            builder.AddCssClass(GovUkInputWrapperClass);

            return builder;
        }

        private static TagBuilder GetGovFormGroupBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);
            builder.AddCssClass(GovUkFormGroup);
            return builder;
        }

        private TagBuilder GetInputBuilder()
        {
            var builder = htmlGenerator.GenerateTextBox(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                For.Model,
                null,
                new
                {
                    @class = $"{TagHelperConstants.NhsInput} {TextInputWidthClass}",
                    aria_describedby = $"{For.Name}-info {For.Name}-summary",
                    spellcheck = "false",
                });

            if (!builder.Attributes.Any(a => a.Key == "maxlength"))
                builder.MergeAttribute("maxlength", DefaultMaxLength.ToString());

            if (TagHelperFunctions.CheckIfModelStateHasErrors(ViewContext, For))
                builder.AddCssClass(TagHelperConstants.NhsValidationInputError);

            return builder;
        }

        private TagBuilder GetPrefixBuilder()
        {
            if (string.IsNullOrWhiteSpace(PrefixText))
                return null;

            var builder = new TagBuilder(TagHelperConstants.Div);
            builder.AddCssClass(GovUkInputPrefixClass);

            builder.MergeAttribute(TagHelperConstants.AriaHidden, "true");

            builder.InnerHtml.Append(PrefixText);

            return builder;
        }

        private TagBuilder GetSuffixBuilder()
        {
            if (string.IsNullOrWhiteSpace(SuffixText))
                return null;

            var builder = new TagBuilder(TagHelperConstants.Div);
            builder.AddCssClass(GovUkInputSuffixClass);

            builder.MergeAttribute(TagHelperConstants.AriaHidden, "true");

            builder.InnerHtml.Append(SuffixText);

            return builder;
        }
    }
}
