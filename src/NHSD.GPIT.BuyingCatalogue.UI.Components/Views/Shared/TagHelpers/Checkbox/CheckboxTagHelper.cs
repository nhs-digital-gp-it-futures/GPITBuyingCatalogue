using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.DataAttributes;

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

        private readonly IHtmlGenerator htmlGenerator;

        public CheckboxTagHelper(IHtmlGenerator htmlGenerator) => this.htmlGenerator = htmlGenerator;

        public enum CheckboxSize
        {
            Small,
            Medium,
        }

        [HtmlAttributeName("size")]
        public CheckboxSize Size { get; set; } = CheckboxSize.Medium;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(HiddenAttributeName)]
        public ModelExpression HiddenFor { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(TagHelperConstants.HintTextName)]
        public string HintText { get; set; }

        [HtmlAttributeName(TagHelperConstants.SubHintText)]
        public string SubHintText { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelEmbedHtml)]
        public bool EmbedHtml { get; set; } = false;

        [HtmlAttributeName(TagHelperConstants.SubGroupName)]
        public string SubGroup { get; set; }

        [HtmlAttributeName(TagHelperConstants.IndexName)]
        public int Index { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(TagHelperConstants.Class, NhsCheckboxItem);

            var label = GetCheckboxLabelBuilder();
            var input = GetCheckboxInputBuilder();

            if (!string.IsNullOrWhiteSpace(SubGroup))
            {
                input.Attributes.Add("subgroup", SubGroup.Replace(" ", string.Empty));
            }

            bool isSelected = input.Attributes.TryGetValue("checked", out string _);

            var childContent = await output.GetChildContentAsync();

            var classes = new List<string> { TagHelperConstants.NhsCheckBoxChildConditionalClass };

            if (!isSelected)
                classes.Add(TagHelperConstants.NhsCheckBoxChildConditionalHiddenClass);

            if (!childContent.IsEmptyOrWhiteSpace)
            {
                TagHelperFunctions.ProcessOutputForConditionalContent(
                    output,
                    context,
                    input,
                    childContent,
                    classes,
                    isSelected);
            }

            output.Content.AppendHtml(input);
            output.Content.AppendHtml(label);

            if (!string.IsNullOrWhiteSpace(HintText))
                output.Content.AppendHtml(GetCheckboxHintBuilder(HintText));

            if (!string.IsNullOrWhiteSpace(SubHintText))
                output.Content.AppendHtml(GetCheckboxHintBuilder(SubHintText, true));

            if (HiddenFor != null)
                output.Content.AppendHtml(GetHiddenInputBuilder());

            TagHelperFunctions.TellParentTagIfThisTagIsInError(ViewContext, context, For);
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

            var tagBuilder = htmlGenerator.GenerateLabel(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                labelText,
                new { @class = $"{TagHelperConstants.NhsLabel} {NhsCheckboxLabel}" });

            if (!EmbedHtml) return tagBuilder;

            tagBuilder.InnerHtml.Clear();
            tagBuilder.InnerHtml.AppendHtml(labelText);

            return tagBuilder;
        }

        private TagBuilder GetCheckboxHintBuilder(
            string value,
            bool isSubHint = false)
        {
            var builder = new TagBuilder(TagHelperConstants.Div);
            builder.AddCssClass(TagHelperConstants.NhsHint);
            builder.AddCssClass(TagHelperConstants.NhsCheckboxHint);
            if (isSubHint)
                builder.AddCssClass(TagHelperConstants.NhsCheckboxSubHint);

            builder.Attributes["id"] = TagBuilder.CreateSanitizedId($"{For.Name}_{Index}-item-hint", "_");

            if (EmbedHtml)
                builder.InnerHtml.AppendHtml(value);
            else
                builder.InnerHtml.Append(value);

            return builder;
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
    }
}
