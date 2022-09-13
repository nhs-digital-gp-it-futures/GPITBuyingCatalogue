using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    public class SubmitButtonTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-submit-button";
        public const string ButtonTextOverrideName = "text";

        private readonly IViewComponentHelper viewComponentHelper;

        public SubmitButtonTagHelper(IViewComponentHelper viewComponentHelper)
        {
            this.viewComponentHelper = viewComponentHelper;
        }

        [HtmlAttributeName(ButtonTextOverrideName)]
        public string Text { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // this tag helper uses the NhsSubmitButtonViewComponent for all its rendering.
            // this tag helper will be removed when we move to .NET 6.0 which will allow optional parameters for
            // view components, when we'll move over to using the NhsSubmitButton View Component Explicitly
            ((IViewContextAware)viewComponentHelper).Contextualize(ViewContext);

            var classes = output.Attributes.TryGetAttribute("class", out var className)
                ? className.Value.ToString()
                : string.Empty;

            var attributes = string.Join(' ', output.Attributes.Where(a => a.Name != "class").Select(a => $"{a.Name}={a.Value}"));

            var submitButtonFromViewComponent = await viewComponentHelper.InvokeAsync(
                "NhsSubmitButton",
                new
                {
                    text = Text,
                    addOnClasses = classes,
                    addOnAttributes = attributes,
                });

            output.SuppressOutput();
            output.PostElement.SetHtmlContent(submitButtonFromViewComponent);
        }
    }
}
