using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(CheckboxTagHelper.TagHelperName)]
    public class CheckboxContainerTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-checkbox-container";

        private ConditionalContext conditionalContext;

        public override void Init(TagHelperContext context)
        {
            if (!context.Items.TryGetValue(TagHelperConstants.ConditionalContextName, out _))
            {
                conditionalContext = new ConditionalContext();

                context.Items.Add(TagHelperConstants.ConditionalContextName, conditionalContext);
            }
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            var content = await output.GetChildContentAsync();

            output.Attributes.Add(
                new TagHelperAttribute(
                    TagHelperConstants.Class, BuildCssClassForOutput(context)));

            output.Content.AppendHtml(content);
        }

        private string BuildCssClassForOutput(TagHelperContext context)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(TagHelperConstants.NhsCheckboxes);

            // only apply to self if this is the parent container
            if (context.Items.TryGetValue(TagHelperConstants.ConditionalContextName, out object value))
            {
                if ((value as ConditionalContext).ContainsConditionalContent == true)
                {
                    if (conditionalContext is not null)
                    {
                        stringBuilder.Append(' ');
                        stringBuilder.Append(TagHelperConstants.NhsCheckBoxParentConditionalClass);
                    }
                }
            }

            return stringBuilder.ToString();
        }
    }
}
