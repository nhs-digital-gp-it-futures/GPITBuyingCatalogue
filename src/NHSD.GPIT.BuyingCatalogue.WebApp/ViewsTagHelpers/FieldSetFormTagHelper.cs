using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ViewsTagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(CheckBoxContainerTagName, RadioButtonTagName)]
    public sealed class FieldSetFormTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-fieldset-form";
        public const string CheckBoxContainerTagName = "nhs-checkbox-container";
        public const string RadioButtonTagName = "nhs-radio-buttons";
        public const string FieldSetFormName = "name";

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelHintName)]
        public string LabelHint { get; set; }

        [HtmlAttributeName(FieldSetFormName)]
        public string FormName { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var outerTesting = TagHelperBuilders.GetOuterTestingDivBuilder(FormName);
            var innerTesting = TagHelperBuilders.GetInnerTestingDivBuilder(TagHelperConstants.CheckBoxOptions);
            var formGroup = TagHelperBuilders.GetFormGroupBuilder(FormName);
            var fieldset = GetFieldSetLegendHeadingBuilder();
            var hint = TagHelperBuilders.GetLabelHintBuilder(FormName, LabelHint);

            var content = await output.GetChildContentAsync();

            formGroup.InnerHtml.AppendHtml(fieldset);
            formGroup.InnerHtml.AppendHtml(hint);
            formGroup.InnerHtml.AppendHtml(content);
            innerTesting.InnerHtml.AppendHtml(formGroup);
            outerTesting.InnerHtml.AppendHtml(innerTesting);

            TagHelperBuilders.UpdateOutputDiv(output, null, ViewContext, outerTesting, TagHelperConstants.SectionListForm, true, FormName);
        }

        private TagBuilder GetFieldSetLegendHeadingBuilder()
        {
            var fieldset = GetFieldsetBuilder();
            var fieldsetLegend = GetFieldsetLegendBuilder();
            var fieldsetlegendheader = GetFieldsetLegendHeadingTagBuilder();

            fieldsetLegend.InnerHtml.AppendHtml(fieldsetlegendheader);
            fieldset.InnerHtml.AppendHtml(fieldsetLegend);

            return fieldset;
        }

        private TagBuilder GetFieldsetBuilder()
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

            if (LabelText == null)
                return new TagBuilder("empty");

            var builder = new TagBuilder(TagHelperConstants.H1);

            builder.AddCssClass(TagHelperConstants.NhsFieldSetLegendHeading);

            builder.InnerHtml.Append(LabelText);

            return builder;
        }
    }
}
