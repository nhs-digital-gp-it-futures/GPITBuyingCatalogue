using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Checkbox;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(CheckboxTagHelper.TagHelperName)]
    public class CheckboxContainerTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-checkbox-container";

        public const string CheckBoxContextName = "CheckboxContext";

        private const string NhsCheckboxes = "nhsuk-checkboxes";
        private const string NhsCheckBoxConditionalClass = "nhsuk-checkboxes--conditional";
        private const string NhsCheckBoxConditionalHiddenClass = "nhsuk-checkboxes__conditional--hidden";

        private CheckboxContext checkboxContext;

        public override void Init(TagHelperContext context)
        {
            if (!context.Items.TryGetValue(CheckBoxContextName, out _))
            {
                checkboxContext = new CheckboxContext();

                context.Items.Add(CheckBoxContextName, checkboxContext);
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

            stringBuilder.Append(NhsCheckboxes);

            if (context.Items.TryGetValue(CheckBoxContextName, out object value))
            {
                if ((value as CheckboxContext).ContainsConditionalContent == true)
                {
                    stringBuilder.Append(' ');
                    stringBuilder.Append(NhsCheckBoxConditionalClass);

                    if (checkboxContext is null)
                    {
                        stringBuilder.Append(' ');
                        stringBuilder.Append(NhsCheckBoxConditionalHiddenClass);
                    }
                }
            }

            return stringBuilder.ToString();
        }
    }
}
