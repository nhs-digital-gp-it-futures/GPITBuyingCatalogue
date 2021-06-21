using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using static NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.FieldSet.FieldSetTagHelperBuilders;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(
        CheckboxContainerTagHelper.TagHelperName,
        RadioButtonsTagHelper.TagHelperName,
        YesNoRadioButtonTagHelper.TagHelperName,
        NhsDateInputTagHelper.TagHelperName)]
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

            var fieldset = GetFieldsetBuilder(formName);

            var fieldsetheading = GetFieldSetLegendHeadingBuilder(SelectedSize, LabelText);

            var hint = TagHelperBuilders.GetLabelHintBuilder(For, LabelHint, formName);

            var content = await output.GetChildContentAsync();

            var errorMessage = BuildErrorMessage();

            if (IsChildInError())
                formGroup.AddCssClass(TagHelperConstants.NhsFormGroupError);

            fieldset.InnerHtml
                .AppendHtml(fieldsetheading)
                .AppendHtml(hint)
                .AppendHtml(errorMessage)
                .AppendHtml(content);

            formGroup.InnerHtml
                .AppendHtml(fieldset);

            TagHelperBuilders.UpdateOutputDiv(output, null, ViewContext, formGroup, true, formName, IsChildInError());
        }

        private TagBuilder BuildErrorMessage()
        {
            if (!IsChildInError())
                return null;

            var outerBuilder = new TagBuilder(TagHelperConstants.Span);

            outerBuilder.AddCssClass(TagHelperConstants.NhsErrorMessage);
            outerBuilder.GenerateId($"{TagHelperFunctions.GetModelKebabNameFromFor(For)}-error", "_");

            var innerBuilder = new TagBuilder(TagHelperConstants.Span);

            innerBuilder.AddCssClass("nhsuk-u-visually-hidden");
            innerBuilder.InnerHtml.Append("Error: ");

            outerBuilder.InnerHtml.AppendHtml(innerBuilder);
            outerBuilder.InnerHtml.Append(parentChildContext.ErrorMessage);

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
