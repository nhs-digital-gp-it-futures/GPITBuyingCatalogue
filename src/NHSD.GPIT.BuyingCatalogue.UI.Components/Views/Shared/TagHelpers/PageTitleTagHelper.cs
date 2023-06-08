using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class PageTitleTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-page-title";
        public const string ModelName = "model";
        public const string TitleName = "title";
        public const string TitleCaptionName = "caption";
        public const string TitleAdviceName = "advice";
        public const string TitleAdviceAdditionalName = "additional-advice";

        private readonly IViewComponentHelper viewComponentHelper;

        public PageTitleTagHelper(IViewComponentHelper viewComponentHelper)
        {
            this.viewComponentHelper = viewComponentHelper;
        }

        [HtmlAttributeName(ModelName)]
        public IPageTitleModel Model { get; set; }

        [HtmlAttributeName(TitleName)]
        public string Title { get; set; }

        [HtmlAttributeName(TitleCaptionName)]
        public string TitleCaption { get; set; }

        [HtmlAttributeName(TitleAdviceName)]
        public string TitleAdvice { get; set; }

        [HtmlAttributeName(TitleAdviceAdditionalName)]
        public string TitleAdviceAdditional { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // this tag helper uses the NhsPageTitleViewComponent for all its rendering.
            // this tag helper will be removed when we move to .NET 6.0 which will allow optional parameters for
            // view components, when we'll move over to using the NhsPageTitle View Component Explicitly
            ((IViewContextAware)viewComponentHelper).Contextualize(ViewContext);

            var pageTitleFromViewComponent = await viewComponentHelper.InvokeAsync(
                "NhsPageTitle",
                new
                {
                    title = Model?.Title ?? Title,
                    titleCaption = Model?.Caption ?? TitleCaption,
                    titleAdvice = Model?.Advice ?? TitleAdvice,
                    titleAdditionalAdvice = Model?.AdditionalAdvice ?? TitleAdviceAdditional,
                });

            output.TagName = string.Empty;

            output.Content.AppendHtml(pageTitleFromViewComponent);
        }
    }
}
