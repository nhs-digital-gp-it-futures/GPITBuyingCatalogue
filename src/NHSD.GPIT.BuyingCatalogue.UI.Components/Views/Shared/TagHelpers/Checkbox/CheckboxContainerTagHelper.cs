using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(CheckboxTagHelper.TagHelperName, TagHelperConstants.Input)]
    public sealed class CheckboxContainerTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-checkbox-container";

        private ConditionalContext conditionalContext;

        public enum CheckboxSize
        {
            Normal,
            Small,
        }

        [HtmlAttributeName(TagHelperConstants.Size)]
        public CheckboxSize Size { get; set; } = CheckboxSize.Normal;

        public override void Init(TagHelperContext context)
        {
            if (context.Items.TryGetValue(TagHelperConstants.ConditionalContextName, out _))
                return;

            conditionalContext = new ConditionalContext();

            context.Items.Add(TagHelperConstants.ConditionalContextName, conditionalContext);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            var content = await output.GetChildContentAsync();

            output.Attributes.Add(
                new TagHelperAttribute(
                    TagHelperConstants.Class,
                    TagHelperFunctions.BuildCssClassForConditionalContentOutput(
                        context,
                        conditionalContext,
                        Size == CheckboxSize.Small ? $"{TagHelperConstants.NhsCheckboxes} {TagHelperConstants.NhsCheckboxesSizeSmall}" : $"{TagHelperConstants.NhsCheckboxes}",
                        TagHelperConstants.NhsCheckBoxParentConditionalClass)));

            output.Content.AppendHtml(content);
        }
    }
}
