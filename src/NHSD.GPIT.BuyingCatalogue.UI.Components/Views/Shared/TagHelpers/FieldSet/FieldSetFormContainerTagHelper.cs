using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;
using static NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.FieldSet.FieldSetTagHelperBuilders;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.FieldSet
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class FieldSetFormContainerTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-fieldset-form-container";

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelHintName)]
        public string LabelHint { get; set; }

        [HtmlAttributeName(FieldSetSizeName)]
        public FieldSetSize SelectedSize { get; set; } = FieldSetSize.Large;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            string formName = TagHelperFunctions.GetModelKebabNameFromFor(For);

            var formGroup = TagHelperBuilders.GetFormGroupBuilder();
            var fieldset = GetFieldsetBuilder(formName, LabelHint);
            var fieldsetheading = GetFieldSetLegendHeadingBuilder(SelectedSize, LabelText);
            var hint = TagHelperBuilders.GetLabelHintBuilder(For, LabelHint, formName);
            var errorMessage = BuildErrorMessage(formName);

            var content = await output.GetChildContentAsync();

            fieldset.InnerHtml
                .AppendHtml(fieldsetheading)
                .AppendHtml(hint)
                .AppendHtml(errorMessage)
                .AppendHtml(content);

            formGroup.InnerHtml
                .AppendHtml(fieldset);

            TagHelperBuilders.UpdateOutputDiv(output, null, ViewContext, formGroup, false, formName);
        }

        private TagBuilder BuildErrorMessage(string formName)
        {
            if (!TagHelperFunctions.CheckIfModelStateHasErrors(ViewContext, null, formName))
                return null;

            var outerBuilder = new TagBuilder(TagHelperConstants.Span);

            outerBuilder.AddCssClass(TagHelperConstants.NhsErrorMessage);
            outerBuilder.GenerateId($"{formName}-error", "_");

            var innerBuilder = new TagBuilder(TagHelperConstants.Span);

            innerBuilder.AddCssClass("nhsuk-u-visually-hidden");
            innerBuilder.InnerHtml.Append("Error: ");

            outerBuilder.InnerHtml.AppendHtml(innerBuilder);
            outerBuilder.InnerHtml.Append(ViewContext.ViewData.ModelState[formName].Errors[0].ErrorMessage);

            return outerBuilder;
        }
    }
}
