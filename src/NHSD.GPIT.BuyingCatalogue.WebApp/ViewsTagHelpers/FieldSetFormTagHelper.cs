using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ViewsTagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(CheckBoxContainerTagName, RadioButtonTagName)]
    public sealed class FieldSetFormTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-fieldset-form";
        public const string CheckBoxContainerTagName = CheckboxContainerTagHelper.TagHelperName;
        public const string RadioButtonTagName = RadioButtonsTagHelper.TagHelperName;

        public const string ContainsRadioButtonsName = "contains-radio-buttons";

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

        [HtmlAttributeName(ContainsRadioButtonsName)]
        public bool? ContainsRadioButtons { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            string FormName = GetModelKebabNameFromFor();

            var outerTesting = TagHelperBuilders.GetOuterTestingDivBuilder(FormName);
            var innerTesting = TagHelperBuilders.GetInnerTestingDivBuilder(ContainsRadioButtons == true ?
                                                                            TagHelperConstants.RadioOptions :
                                                                            TagHelperConstants.CheckBoxOptions);
            var formGroup = TagHelperBuilders.GetFormGroupBuilder(FormName);
            var fieldset = GetFieldSetLegendHeadingBuilder(FormName);
            var hint = TagHelperBuilders.GetLabelHintBuilder(For, LabelHint, FormName, DisableLabelAndHint);

            var content = await output.GetChildContentAsync();

            formGroup.InnerHtml.AppendHtml(fieldset);
            formGroup.InnerHtml.AppendHtml(hint);
            formGroup.InnerHtml.AppendHtml(content);
            innerTesting.InnerHtml.AppendHtml(formGroup);
            outerTesting.InnerHtml.AppendHtml(innerTesting);

            TagHelperBuilders.UpdateOutputDiv(output, null, ViewContext, outerTesting, TagHelperConstants.SectionListForm, true, FormName);
        }

        private TagBuilder GetFieldSetLegendHeadingBuilder(string FormName)
        {
            var fieldset = GetFieldsetBuilder(FormName);
            var fieldsetLegend = GetFieldsetLegendBuilder();
            var fieldsetlegendheader = GetFieldsetLegendHeadingTagBuilder();

            fieldsetLegend.InnerHtml.AppendHtml(fieldsetlegendheader);
            fieldset.InnerHtml.AppendHtml(fieldsetLegend);

            return fieldset;
        }

        private TagBuilder GetFieldsetBuilder(string FormName)
        {
            var builder = new TagBuilder(TagHelperConstants.FieldSet);

            builder.AddCssClass(TagHelperConstants.NhsFieldset);
            builder.MergeAttribute(TagHelperConstants.AriaDescribedBy, $"{FormName}-hint");

            return builder;
        }

        private TagBuilder GetFieldsetLegendBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Legend);

            builder.AddCssClass(TagHelperConstants.NhsFieldsetLegend);
            builder.AddCssClass(TagHelperConstants.NhsFieldsetLegendOne);

            return builder;
        }

        private TagBuilder GetFieldsetLegendHeadingTagBuilder()
        {

            if (LabelText == null || DisableLabelAndHint == true)
                return new TagBuilder("empty");

            var builder = new TagBuilder(TagHelperConstants.H1);

            builder.AddCssClass(TagHelperConstants.NhsFieldSetLegendHeading);

            builder.InnerHtml.Append(LabelText);

            return builder;
        }

        private string GetModelKebabNameFromFor()
        {
            string name = For.Model.GetType().Name;
            name = name.Remove(name.Length - 5);

            var pattern = new Regex(@"[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+");
            return string.Join("-", pattern.Matches(name)).ToLower();
        }
    }
}
