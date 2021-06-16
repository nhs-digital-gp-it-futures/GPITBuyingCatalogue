using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Table
{
    [HtmlTargetElement(TagHelperName, ParentTag = TableContainerTagHelper.TagHelperName)]
    [RestrictChildren(TableCellTagHelper.TagHelperName)]
    public sealed class TableRowContainerTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-table-row-container";

        private const string TableRowClass = "nhsuk-table__row";
        private const string TableRowRole = "row";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Reinitialize("tr", TagMode.StartTagAndEndTag);

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Role, TableRowRole));
            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, TableRowClass));

            var children = await output.GetChildContentAsync();

            output.Content.AppendHtml(children);
        }
    }
}
