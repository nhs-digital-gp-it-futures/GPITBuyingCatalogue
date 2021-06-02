using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(
        CheckboxContainerTagHelper.TagHelperName,
        RadioButtonsTagHelper.TagHelperName,
        YesNoRadioButtonTagHelper.TagHelperName)]
    public sealed class FieldSetFormTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-fieldset-form";

        private const string NhsFieldset = "nhsuk-fieldset";
        private const string NhsFieldsetLegend = "nhsuk-fieldset__legend";
        private const string NhsFieldsetLegendOne = "nhsuk-fieldset__legend--1";
        private const string NhsFieldSetLegendHeading = "nhsuk-fieldset__heading";

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

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            string formName = GetModelKebabNameFromFor();

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

        private static TagBuilder GetFieldsetLegendBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Legend);

            builder.AddCssClass(NhsFieldsetLegend);
            builder.AddCssClass(NhsFieldsetLegendOne);

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

            var builder = new TagBuilder(TagHelperConstants.Header);
            builder.AddCssClass(NhsFieldSetLegendHeading);

            builder.InnerHtml.Append(LabelText);

            return builder;
        }

        private string GetModelKebabNameFromFor()
        {
            string name = For.Model.GetType().Name;

            // removes the word Model from the end of the Model, e.g SolutionDescriptionModel becomes SolutionDescription
            name = name.Remove(name.Length - 5);

            var pattern = new Regex(@"[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+");
            return string.Join("-", pattern.Matches(name)).ToLower();
        }
    }
}
