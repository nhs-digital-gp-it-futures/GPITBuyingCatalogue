using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.DetailsAndExpander;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Radios;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.TimeInput;
using static NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.FieldSet.FieldSetTagHelperBuilders;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class FieldSetFormLabelTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-fieldset-form-label";

        private ParentChildContext parentChildContext;

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

        public override void Init(TagHelperContext context)
        {
            parentChildContext = new ParentChildContext();

            if (context.Items.TryGetValue(typeof(ParentChildContext), out _))
            {
                context.Items[typeof(ParentChildContext)] = parentChildContext;
            }
            else
            {
                context.Items.Add(typeof(ParentChildContext), parentChildContext);
            }
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            string formName = TagHelperFunctions.GetModelKebabNameFromFor(For);

            var formGroup = TagHelperBuilders.GetFormGroupBuilder();

            var fieldset = GetFieldsetBuilder(formName, LabelHint);

            var fieldSetHeading = GetFieldSetLegendHeadingBuilder(SelectedSize, LabelText);

            var content = await output.GetChildContentAsync();

            var labelHint = parentChildContext.IsTimeInput ? TimeInputConstants.TimeInputHint : LabelHint;

            var hint = TagHelperBuilders.GetLabelHintBuilder(For, labelHint, formName);

            var errorMessage = BuildErrorMessage(formName);

            if (IsChildInError())
                formGroup.AddCssClass(TagHelperConstants.NhsFormGroupError);

            fieldset.InnerHtml
                .AppendHtml(fieldSetHeading)
                .AppendHtml(hint)
                .AppendHtml(errorMessage)
                .AppendHtml(content);

            formGroup.InnerHtml
                .AppendHtml(fieldset);

            TagHelperBuilders.UpdateOutputDiv(output, null, ViewContext, formGroup, false, formName, IsChildInError());
        }

        private TagBuilder BuildErrorMessage(string formName)
        {
            if (!IsChildInError() && !TagHelperFunctions.CheckIfModelStateHasErrors(ViewContext, null, formName))
                return null;

            var outerBuilder = new TagBuilder(TagHelperConstants.Span);

            outerBuilder.AddCssClass(TagHelperConstants.NhsErrorMessage);
            outerBuilder.GenerateId($"{formName}-error", "_");

            var innerBuilder = new TagBuilder(TagHelperConstants.Span);

            innerBuilder.AddCssClass("nhsuk-u-visually-hidden");
            innerBuilder.InnerHtml.Append("Error: ");

            outerBuilder.InnerHtml.AppendHtml(innerBuilder);
            outerBuilder.InnerHtml.Append(parentChildContext.ErrorMessage ?? ViewContext.ViewData.ModelState[formName].Errors[0].ErrorMessage);

            return outerBuilder;
        }

        private bool IsChildInError()
        {
            if (parentChildContext is null)
                return false;

            // we should only error the whole fieldset if there is only one child and it's in error
            return parentChildContext.ChildInError;
        }
    }
}
