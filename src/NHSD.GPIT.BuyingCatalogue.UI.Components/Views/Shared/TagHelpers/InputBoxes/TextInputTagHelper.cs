﻿using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.DataAttributes;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class TextInputTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-input";

        private const int DefaultMaxLength = 500;

        private readonly IHtmlGenerator htmlGenerator;

        public TextInputTagHelper(IHtmlGenerator htmlGenerator)
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
        public bool EnableCharacterCounter { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var formGroup = TagHelperBuilders.GetFormGroupBuilder();
            var label = TagHelperBuilders.GetLabelBuilder(ViewContext, For, htmlGenerator, null, LabelText);
            var hint = TagHelperBuilders.GetLabelHintBuilder(For, LabelHint, null);
            var validation = TagHelperBuilders.GetValidationBuilder(ViewContext, For, htmlGenerator);
            var input = GetInputBuilder();
            var counter = TagHelperBuilders.GetCounterBuilder(For, DefaultMaxLength, EnableCharacterCounter);

            formGroup.InnerHtml
                .AppendHtml(label)
                .AppendHtml(hint)
                .AppendHtml(validation)
                .AppendHtml(input)
                .AppendHtml(counter);

            TagHelperBuilders.UpdateOutputDiv(output, For, ViewContext, formGroup, EnableCharacterCounter, defaultMaxLength: DefaultMaxLength);
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
                    @class = TagHelperConstants.NhsInput,
                    aria_describedby = $"{For.Name}-info {For.Name}-summary",
                });

            if (!builder.Attributes.Any(a => a.Key == "maxlength"))
            {
                builder.MergeAttribute("maxlength", DefaultMaxLength.ToString());
                builder.MergeAttribute("data-val-length-max", DefaultMaxLength.ToString());
                builder.MergeAttribute("data-val", "true");
            }

            if (TagHelperFunctions.GetCustomAttributes<PasswordAttribute>(For)?.Any() == true)
                builder.Attributes[TagHelperConstants.Type] = "password";

            if (!TagHelperFunctions.IsCounterDisabled(For, EnableCharacterCounter))
                builder.AddCssClass(TagHelperConstants.GovUkJsCharacterCount);

            if (TagHelperFunctions.CheckIfModelStateHasErrors(ViewContext, For))
                builder.AddCssClass(TagHelperConstants.NhsValidationInputError);

            return builder;
        }
    }
}
