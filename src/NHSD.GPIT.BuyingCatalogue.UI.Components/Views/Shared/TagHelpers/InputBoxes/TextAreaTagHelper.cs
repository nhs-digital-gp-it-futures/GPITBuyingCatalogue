using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class TextAreaTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-textarea";

        private const string TextAreaNumberOfRows = "number-of-rows";
        private const string NhsTextArea = "nhsuk-textarea";

        private const int DefaultNumberOfTextAreaRows = 5;
        private const int DefaultMaxLength = 1500;

        private readonly IHtmlGenerator htmlGenerator;

        public TextAreaTagHelper(IHtmlGenerator htmlGenerator)
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

        [HtmlAttributeName(TagHelperConstants.CharacterCountName)]
        public bool CharacterCountEnabled { get; set; } = true;

        [HtmlAttributeName(TextAreaNumberOfRows)]
        public int? NumberOfRows { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var formGroup = TagHelperBuilders.GetFormGroupBuilder();
            var label = TagHelperBuilders.GetLabelBuilder(ViewContext, For, htmlGenerator, null, LabelText);
            var hint = TagHelperBuilders.GetLabelHintBuilder(For, LabelHint, null);
            var validation = TagHelperBuilders.GetValidationBuilder(ViewContext, For, htmlGenerator);
            var input = GetInputBuilder();
            var counter = TagHelperBuilders.GetCounterBuilder(For, DefaultMaxLength, CharacterCountEnabled);

            formGroup.InnerHtml
                .AppendHtml(label)
                .AppendHtml(hint)
                .AppendHtml(validation)
                .AppendHtml(input)
                .AppendHtml(counter);

            TagHelperBuilders.UpdateOutputDiv(output, For, ViewContext, formGroup, CharacterCountEnabled, defaultMaxLength: DefaultMaxLength);
        }

        private TagBuilder GetInputBuilder()
        {
            var builder = htmlGenerator.GenerateTextArea(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                NumberOfRows ?? DefaultNumberOfTextAreaRows,
                0,
                new
                {
                    @class = NhsTextArea,
                });

            if (!builder.Attributes.Any(a => a.Key == "maxlength"))
                builder.MergeAttribute("maxlength", DefaultMaxLength.ToString());

            var describedBy = new List<string>();

            if (!string.IsNullOrWhiteSpace(LabelHint))
                describedBy.Add($"{For.Name}-hint");

            if (!TagHelperFunctions.IsCounterDisabled(For, CharacterCountEnabled))
            {
                describedBy.Add($"{For.Name}-info");
                builder.AddCssClass(TagHelperConstants.GovUkJsCharacterCount);
            }

            if (describedBy.Any())
            {
                builder.MergeAttribute(
                    TagHelperConstants.AriaDescribedBy,
                    TagBuilder.CreateSanitizedId(string.Join(' ', describedBy), "_"));
            }

            if (TagHelperFunctions.CheckIfModelStateHasErrors(ViewContext, For))
            {
                builder.AddCssClass(TagHelperConstants.NhsValidationInputError);
            }

            return builder;
        }
    }
}
