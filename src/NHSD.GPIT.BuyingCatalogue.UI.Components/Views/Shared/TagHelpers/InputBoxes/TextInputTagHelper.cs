using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.DataAttributes;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.HintText;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(HintTextTagHelper.TagHelperName)]
    public sealed class TextInputTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-input";
        public const string InputWidthName = "input-width";
        public const string VisuallyHiddenErrorName = "visually-hidden-error";

        private readonly IHtmlGenerator htmlGenerator;

        public TextInputTagHelper(IHtmlGenerator htmlGenerator)
        {
            this.htmlGenerator = htmlGenerator;
        }

        public enum TextInputWidth
        {
            Thirty = 30,
            Twenty = 20,
            Ten = 10,
            Five = 5,
            Four = 4,
            Three = 3,
            Two = 2,
            Default = 0,
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

        [HtmlAttributeName(InputWidthName)]
        public TextInputWidth TextWidth { get; set; } = TextInputWidth.Default;

        [HtmlAttributeName(TagHelperConstants.CharacterCountName)]
        public bool CharacterCountEnabled { get; set; } = false;

        [HtmlAttributeName(TagHelperConstants.MaxCharacterLength)]
        public int? MaximumCharacterLength { get; set; }

        [HtmlAttributeName(VisuallyHiddenErrorName)]
        public bool VisuallyHiddenError { get; set; } = false;

        private int DefaultMaxLength => MaximumCharacterLength ?? 500;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            LabelHintContext labelHintContext = new LabelHintContext();
            context.Items.Add(typeof(LabelHintContext), labelHintContext);

            await output.GetChildContentAsync();

            var formGroup = TagHelperBuilders.GetFormGroupBuilder();
            var label = TagHelperBuilders.GetLabelBuilder(ViewContext, For, htmlGenerator, null, LabelText);
            var hint = TagHelperBuilders.GetLabelHintBuilder(For, LabelHint, null);
            var validation = TagHelperBuilders.GetValidationBuilder(ViewContext, For, htmlGenerator, VisuallyHiddenError);
            var input = GetInputBuilder();
            var counter = TagHelperBuilders.GetCounterBuilder(For, DefaultMaxLength, CharacterCountEnabled);

            formGroup.InnerHtml
                .AppendHtml(label)
                .AppendHtml(hint)
                .AppendHtml(labelHintContext.LabelHintHtml)
                .AppendHtml(validation)
                .AppendHtml(input)
                .AppendHtml(counter);

            TagHelperBuilders.UpdateOutputDiv(output, For, ViewContext, formGroup, CharacterCountEnabled, defaultMaxLength: DefaultMaxLength);
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
                { @class = TagHelperConstants.NhsInput, });

            if (TextWidth != TextInputWidth.Default)
                builder.AddCssClass($"nhsuk-input--width-{(int)TextWidth}");

            if (!builder.Attributes.Any(a => a.Key == "maxlength"))
            {
                builder.MergeAttribute("maxlength", DefaultMaxLength.ToString());
                builder.MergeAttribute("data-val-length-max", DefaultMaxLength.ToString());
                builder.MergeAttribute("data-val", "true");
            }

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

            if (TagHelperFunctions.GetCustomAttributes<PasswordAttribute>(For)?.Any() == true)
                builder.Attributes[TagHelperConstants.Type] = "password";

            if (TagHelperFunctions.CheckIfModelStateHasErrors(ViewContext, For))
                builder.AddCssClass(TagHelperConstants.NhsValidationInputError);

            return builder;
        }
    }
}
