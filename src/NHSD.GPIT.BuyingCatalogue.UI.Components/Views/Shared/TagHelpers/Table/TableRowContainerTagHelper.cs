using System.Collections.Generic;
using System.Threading.Tasks;
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

        private Queue<TagHelperContent> CellColumnNames { get; set; }

        public override void Init(TagHelperContext context)
        {
            if (!context.Items.TryGetValue("ColumnNames", out object columnNames))
                return;

            var columnNamesConverted = (List<TagHelperContent>)columnNames;

            if (columnNamesConverted.Count == 0)
                return;

            CellColumnNames = new Queue<TagHelperContent>(columnNamesConverted);

            context.Items.Add("CellColumnNames", CellColumnNames);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "tr";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Role, TableRowRole));
            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, TableRowClass));

            var children = await output.GetChildContentAsync();

            output.Content.AppendHtml(children);
        }
    }
}
