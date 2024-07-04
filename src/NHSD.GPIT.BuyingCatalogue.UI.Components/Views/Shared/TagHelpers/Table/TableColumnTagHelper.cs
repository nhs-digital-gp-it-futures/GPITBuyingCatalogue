using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Table
{
    [HtmlTargetElement(TagHelperName, ParentTag = TableContainerTagHelper.TagHelperName)]
    public sealed class TableColumnTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-table-column";

        private const string NumericName = "numeric";

        [HtmlAttributeName(NumericName)]
        public bool Numeric { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.SuppressOutput();

            var childContent = await output.GetChildContentAsync();

            if (context.Items[TagHelperConstants.ColumnNameContextName] is not List<(TagHelperContent, bool)> columns)
                return;

            columns.Add((childContent, Numeric));
        }
    }
}
