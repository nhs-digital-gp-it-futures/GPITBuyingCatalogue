using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Table
{
    [HtmlTargetElement(TagHelperName, ParentTag = TableContainerTagHelper.TagHelperName)]
    public sealed class TableColumnTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-table-column";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.SuppressOutput();

            var childContent = await output.GetChildContentAsync();

            var columnNames = context.Items["ColumnNames"] as List<TagHelperContent>;

            if (columnNames is null)
                return;

            columnNames.Add(childContent);
        }
    }
}
