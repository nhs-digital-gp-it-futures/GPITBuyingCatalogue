using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.TimeInput
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class NhsTimeInputTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-time-input";

        private readonly IHtmlGenerator htmlGenerator;

        public NhsTimeInputTagHelper(IHtmlGenerator htmlGenerator) => this.htmlGenerator = htmlGenerator;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!typeof(DateTime?).IsAssignableFrom(For.ModelExplorer.ModelType))
                throw new ArgumentException($"{nameof(For)} is not of type DateTime from attribute {nameof(For.Name)}");

            var formGroup = TagHelperBuilders.GetFormGroupBuilder();
            var label = TagHelperBuilders.GetLabelBuilder(ViewContext, For, htmlGenerator, null, LabelText);
            var hint = TagHelperBuilders.GetLabelHintBuilder(For, TimeInputConstants.TimeInputHint, null);
            var validation = TagHelperBuilders.GetValidationBuilder(ViewContext, For, htmlGenerator);
            var input = GetInputBuilder();

            formGroup.InnerHtml
                .AppendHtml(label)
                .AppendHtml(hint)
                .AppendHtml(validation)
                .AppendHtml(input);

            TagHelperBuilders.UpdateOutputDiv(output, For, ViewContext, formGroup, false, defaultMaxLength: TimeInputConstants.MaxCharacterLength);
        }

        private TagBuilder GetInputBuilder()
        {
            var value = (DateTime?)For.Model == DateTime.MinValue || For.Model is null ? string.Empty : ((DateTime?)For.Model).Value.ToString("HH:mm");

            var builder = htmlGenerator.GenerateTextBox(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                value,
                null,
                new
                {
                    @class = $"{TagHelperConstants.NhsInput} {TimeInputConstants.TimeInputInputClass} {TimeInputConstants.TimeInputWidthClass}",
                    maxlength = TimeInputConstants.MaxCharacterLength.ToString(),
                });

            if (TagHelperFunctions.CheckIfModelStateHasErrors(ViewContext, For))
                builder.AddCssClass(TagHelperConstants.NhsValidationInputError);

            return builder;
        }
    }
}
