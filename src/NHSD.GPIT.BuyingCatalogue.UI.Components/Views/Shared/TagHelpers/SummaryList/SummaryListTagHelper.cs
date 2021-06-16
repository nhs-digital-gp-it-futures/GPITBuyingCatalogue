using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.SummaryList
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(SummaryListRowTagHelper.TagHelperName)]
    public sealed class SummaryListTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-summary-list";

        private const string SummaryListClass = "nhsuk-summary-list";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Reinitialize("dl", TagMode.StartTagAndEndTag);

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, SummaryListClass));

            var children = await output.GetChildContentAsync();

            output.Content.AppendHtml(children);
        }
    }
}
