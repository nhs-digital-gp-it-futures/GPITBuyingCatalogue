using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Table
{
    [HtmlTargetElement(TableRowHeaderCellTagHelper.TagHelperName, ParentTag = TableRowContainerTagHelper.TagHelperName)]
    public sealed class TableRowHeaderCellTagHelper : TagHelper
    {
        private const string TagHelperName = "nhs-table-row-header-cell";

        private const string RowHeaderClass = "nhsuk-custom-table__row_header";
        private const string RowHeaderRole = "rowheader";
        private const string RowHeaderScope = "row";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;

            output.TagName = "th";
            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, RowHeaderClass));
            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Role, RowHeaderRole));
            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Scope, RowHeaderScope));

            var heading = CellHeadingBuilder.GetHeadingBuilder(context);

            var children = await output.GetChildContentAsync();

            output.Content
                .AppendHtml(heading)
                .AppendHtml(children);
        }
    }
}
