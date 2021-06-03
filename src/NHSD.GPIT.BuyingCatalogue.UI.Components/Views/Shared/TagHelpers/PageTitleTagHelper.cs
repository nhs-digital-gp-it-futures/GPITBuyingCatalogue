using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class PageTitleTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-page-title";
        public const string TitleName = "title";
        public const string TitleAdviceName = "title-advice";
        public const string TitleAdviceAdditionalName = "title-advice-additional";

        private readonly IViewComponentHelper viewComponentHelper;

        public PageTitleTagHelper(IViewComponentHelper viewComponentHelper)
        {
            this.viewComponentHelper = viewComponentHelper;
        }

        [HtmlAttributeName(TitleName)]
        public string Title { get; set; }

        [HtmlAttributeName(TitleAdviceName)]
        public string TitleAdvice { get; set; }

        [HtmlAttributeName(TitleAdviceAdditionalName)]
        public string TitleAdviceAditional { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var title = GetTitleBuilder();
            var advice = GetAdviceBuilder();
            var additional = await GetAdditionalAdviceBuilder();

            output.TagName = string.Empty;

            output.Content.AppendHtml(title);
            output.Content.AppendHtml(advice);
            output.Content.AppendHtml(additional);
        }

        private TagBuilder GetTitleBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Header);

            builder.InnerHtml.Append(Title);

            return builder;
        }

        private TagBuilder GetAdviceBuilder()
        {
            if (string.IsNullOrWhiteSpace(TitleAdvice))
                return null;

            var builder = new TagBuilder(TagHelperConstants.Paragraph);

            builder.AddCssClass(TagHelperConstants.NhsLedeText);

            builder.InnerHtml.Append(TitleAdvice);

            return builder;
        }

        private async Task<IHtmlContent> GetAdditionalAdviceBuilder()
        {
            if (string.IsNullOrWhiteSpace(TitleAdviceAditional))
                return null;

            ((IViewContextAware)viewComponentHelper).Contextualize(ViewContext);

            return await viewComponentHelper.InvokeAsync("NhsInsetText", new { text = TitleAdviceAditional });
        }
    }
}
