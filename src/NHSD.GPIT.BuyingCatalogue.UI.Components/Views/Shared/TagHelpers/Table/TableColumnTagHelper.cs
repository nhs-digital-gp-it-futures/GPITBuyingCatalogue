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

        private const string CellHeadingTextName = "cell-heading-text";
        private const string NumericName = "numeric";

        [HtmlAttributeName(CellHeadingTextName)]
        public string CellHeadingText { get; set; }

        [HtmlAttributeName(NumericName)]
        public bool Numeric { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.SuppressOutput();

            var childContent = await output.GetChildContentAsync();

            if (context.Items[TagHelperConstants.ColumnNameContextName]
                is not List<(TagHelperContent HeaderContent, TagHelperContent CellHeadingContent, bool Numeric)> columns)
                return;

            var cellHeadingContent = string.IsNullOrWhiteSpace(CellHeadingText)
                ? childContent
                : new DefaultTagHelperContent().SetContent(CellHeadingText);

            columns.Add((childContent, cellHeadingContent, Numeric));
        }
    }
}
