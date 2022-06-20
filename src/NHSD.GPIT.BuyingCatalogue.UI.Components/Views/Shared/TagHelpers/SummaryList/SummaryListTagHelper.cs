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
        private const string SummaryListNoBordersClass = "nhsuk-summary-list--no-border";

        [HtmlAttributeName(TagHelperConstants.NoBordersName)]
        public bool NoBorders { get; set; } = false;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "dl";
            output.TagMode = TagMode.StartTagAndEndTag;

            var classes = NoBorders
                ? $"{SummaryListClass} {SummaryListNoBordersClass}"
                : SummaryListClass;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, classes));

            var children = await output.GetChildContentAsync();

            output.Content.AppendHtml(children);
        }
    }
}
