using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class FieldSetFormTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-fieldset-form";

        private const string FieldSetSizeName = "size";

        private const string NhsFieldset = "nhsuk-fieldset";
        private const string NhsFieldsetLegend = "nhsuk-fieldset__legend";
        private const string NhsFieldsetLegendExtraLarge = "nhsuk-fieldset__legend--xl";
        private const string NhsFieldsetLegendLarge = "nhsuk-fieldset__legend--l";
        private const string NhsFieldsetLegendMedium = "nhsuk-fieldset__legend--m";
        private const string NhsFieldsetLegendSmall = "nhsuk-fieldset__legend--s";
        private const string NhsFieldSetLegendHeading = "nhsuk-fieldset__heading";

        public enum FieldSetSize
        {
            ExtraLarge = 1,
            Large = 0,
            Medium = 2,
            Small = 3,
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelHintName)]
        public string LabelHint { get; set; }

        [HtmlAttributeName(TagHelperConstants.DisableLabelAndHint)]
        public bool? DisableLabelAndHint { get; set; }

        [HtmlAttributeName(FieldSetSizeName)]
        public FieldSetSize SelectedSize { get; set; } = FieldSetSize.Large;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            string formName = TagHelperFunctions.GetModelKebabNameFromFor(For);

            var formGroup = TagHelperBuilders.GetFormGroupBuilder();
            var fieldset = GetFieldSetLegendHeadingBuilder(formName);
            var hint = TagHelperBuilders.GetLabelHintBuilder(For, LabelHint, formName, DisableLabelAndHint);

            var content = await output.GetChildContentAsync();

            formGroup.InnerHtml.AppendHtml(fieldset);
            formGroup.InnerHtml.AppendHtml(hint);
            formGroup.InnerHtml.AppendHtml(content);

            TagHelperBuilders.UpdateOutputDiv(output, null, ViewContext, formGroup, true, formName);
        }

        private static TagBuilder GetFieldsetBuilder(string formName)
        {
            var builder = new TagBuilder(TagHelperConstants.FieldSet);

            builder.AddCssClass(NhsFieldset);
            builder.MergeAttribute(TagHelperConstants.AriaDescribedBy, $"{formName}-hint");

            return builder;
        }

        private TagBuilder GetFieldsetLegendBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Legend);

            var selectedLedgendClass = SelectedSize switch
            {
                FieldSetSize.ExtraLarge => NhsFieldsetLegendExtraLarge,
                FieldSetSize.Large => NhsFieldsetLegendLarge,
                FieldSetSize.Medium => NhsFieldsetLegendMedium,
                FieldSetSize.Small => NhsFieldsetLegendSmall,
                _ => throw new System.InvalidCastException(),
            };

            builder.AddCssClass(NhsFieldsetLegend);
            builder.AddCssClass(selectedLedgendClass);

            return builder;
        }

        private TagBuilder GetFieldSetLegendHeadingBuilder(string formName)
        {
            var fieldset = GetFieldsetBuilder(formName);
            var fieldsetLegend = GetFieldsetLegendBuilder();
            var fieldsetlegendheader = GetFieldsetLegendHeadingTagBuilder();

            fieldsetLegend.InnerHtml.AppendHtml(fieldsetlegendheader);
            fieldset.InnerHtml.AppendHtml(fieldsetLegend);

            return fieldset;
        }

        private TagBuilder GetFieldsetLegendHeadingTagBuilder()
        {
            if (LabelText == null || DisableLabelAndHint == true)
                return null;

            var selectedLedgendTag = SelectedSize switch
            {
                FieldSetSize.ExtraLarge => TagHelperConstants.HeaderOne,
                FieldSetSize.Large => TagHelperConstants.HeaderTwo,
                FieldSetSize.Medium => TagHelperConstants.HeaderThree,
                FieldSetSize.Small => "label",
                _ => throw new System.InvalidCastException(),
            };

            var builder = new TagBuilder(selectedLedgendTag);
            builder.AddCssClass(NhsFieldSetLegendHeading);

            builder.InnerHtml.Append(LabelText);

            return builder;
        }
    }
}
