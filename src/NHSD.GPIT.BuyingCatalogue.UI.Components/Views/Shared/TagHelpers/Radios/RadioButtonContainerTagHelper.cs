using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Radios
{
    [HtmlTargetElement(TagHelperName, ParentTag = FieldSetFormLabelTagHelper.TagHelperName)]
    [RestrictChildren(RadioButtonTagHelper.TagHelperName, TagHelperConstants.Input)]
    public sealed class RadioButtonContainerTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-radio-button-container";

        private ConditionalContext conditionalContext;

        public override void Init(TagHelperContext context)
        {
            if (context.Items.TryGetValue(TagHelperConstants.ConditionalContextName, out _))
                return;

            conditionalContext = new ConditionalContext();

            context.Items.Add(TagHelperConstants.ConditionalContextName, conditionalContext);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = await output.GetChildContentAsync();

            RadioButtonBuilders.UpdateRadioContainerOutput(output, context, false, conditionalContext);

            output.Content.AppendHtml(childContent);
        }
    }
}
