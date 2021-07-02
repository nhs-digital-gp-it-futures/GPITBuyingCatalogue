using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.DataAttributes;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Checkbox;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName, ParentTag = CheckboxContainerTagHelper.TagHelperName)]
    public sealed class CheckboxTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-checkbox";

        private const string HiddenAttributeName = "hidden-input";
        private const string NhsCheckboxItem = "nhsuk-checkboxes__item";
        private const string NhsCheckboxInput = "nhsuk-checkboxes__input";
        private const string NhsCheckboxLabel = "nhsuk-checkboxes__label";
        private const string ChildContentContainerClass = "nhsuk-checkboxes__conditional nhsuk-checkboxes__conditional--hidden";

        private readonly IHtmlGenerator htmlGenerator;

        public CheckboxTagHelper(IHtmlGenerator htmlGenerator) => this.htmlGenerator = htmlGenerator;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(HiddenAttributeName)]
        public ModelExpression HiddenFor { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(TagHelperConstants.Class, NhsCheckboxItem);

            var label = GetCheckboxLabelBuilder();
            var input = GetCheckboxInputBuilder();

            var childContent = await output.GetChildContentAsync();

            if (!childContent.IsEmptyOrWhiteSpace)
            {
                ProcessOutputForConditionalContent(output, context, input, childContent);
            }

            output.Content.AppendHtml(input);
            output.Content.AppendHtml(label);

            if (HiddenFor != null)
                output.Content.AppendHtml(GetHiddenInputBuilder());

            TagHelperFunctions.TellParentTagIfThisTagIsInError(ViewContext, context, For);
        }

        private static void TellParentThisHasConditionalChildContent(TagHelperContext context)
        {
            if (context.Items.TryGetValue(CheckboxContainerTagHelper.CheckBoxContextName, out object value))
            {
                (value as CheckboxContext).ContainsConditionalContent = true;
            }
        }

        private TagBuilder GetCheckboxInputBuilder()
        {
            return htmlGenerator.GenerateCheckBox(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                (bool)For.Model,
                new { @class = NhsCheckboxInput });
        }

        private TagBuilder GetCheckboxLabelBuilder()
        {
            var labelText = LabelText ?? TagHelperFunctions.GetCustomAttribute<CheckboxAttribute>(For)?.DisplayText ?? string.Empty;

            return htmlGenerator.GenerateLabel(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                labelText,
                new { @class = $"{TagHelperConstants.NhsLabel} {NhsCheckboxLabel}" });
        }

        private TagBuilder GetHiddenInputBuilder()
        {
            return htmlGenerator.GenerateHidden(
                ViewContext,
                HiddenFor.ModelExplorer,
                HiddenFor.Name,
                HiddenFor.Model,
                false,
                null);
        }

        private TagBuilder GetChildContentConditionalBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.AddCssClass(ChildContentContainerClass);

            builder.GenerateId($"conditional-{For.Name}", "_");

            return builder;
        }

        private void ProcessOutputForConditionalContent(TagHelperOutput output, TagHelperContext context, TagBuilder input, TagHelperContent childContent)
        {
            var childContainer = GetChildContentConditionalBuilder();

            childContainer.InnerHtml.AppendHtml(childContent);

            output.PostElement.AppendHtml(childContainer);

            childContainer.Attributes.TryGetValue("id", out string containerId);

            input.MergeAttribute(TagHelperConstants.AriaControls, containerId);
            input.MergeAttribute(TagHelperConstants.AriaExpanded, "false");

            TellParentThisHasConditionalChildContent(context);
        }
    }
}
