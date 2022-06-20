using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.SummaryList
{
    [HtmlTargetElement(TagHelperName, ParentTag = PdfSummaryListTagHelper.TagHelperName)]
    public sealed class PdfSummaryListRowTagHelper : TagHelper
    {
        public const string TagHelperName = "pdf-summary-list-row";

        private const string SummaryListRowClass = "pdf-summary-list-row";

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, SummaryListRowClass));

            var key = new TagBuilder(TagHelperConstants.Div);
            key.InnerHtml.Append(LabelText);

            var value = new TagBuilder(TagHelperConstants.Div);
            value.InnerHtml.AppendHtml(await output.GetChildContentAsync());

            output.Content
                .AppendHtml(key)
                .AppendHtml(value);
        }
    }
}
