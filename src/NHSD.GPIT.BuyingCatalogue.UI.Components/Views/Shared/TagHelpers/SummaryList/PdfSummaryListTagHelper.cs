using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.SummaryList
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(PdfSummaryListRowTagHelper.TagHelperName)]
    public sealed class PdfSummaryListTagHelper : TagHelper
    {
        public const string TagHelperName = "pdf-summary-list";

        private const string PdfSummaryListClass = "pdf-summary-list";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, PdfSummaryListClass));
            output.Content.AppendHtml(await output.GetChildContentAsync());
        }
    }
}
