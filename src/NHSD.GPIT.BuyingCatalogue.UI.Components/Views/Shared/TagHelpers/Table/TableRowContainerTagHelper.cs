using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Table
{
    [HtmlTargetElement(TagHelperName, ParentTag = TableContainerTagHelper.TagHelperName)]
    public sealed class TableRowContainerTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-table-row-container";

        private const string TableRowClass = "nhsuk-table__row";
        private const string TableRowRole = "row";

        public override void Init(TagHelperContext context)
        {
            if (!context.Items.TryGetValue(TagHelperConstants.ColumnNameContextName, out object columnNames))
                return;

            var columnNamesConverted = (List<(TagHelperContent Content, bool Numeric)>)columnNames;

            if (columnNamesConverted.Count == 0)
                return;

            var cellColumnNames = new Queue<TagHelperContent>(columnNamesConverted.Select(c => c.Content).ToList());

            context.Items.Add(TagHelperConstants.CellColumnContextName, cellColumnNames);
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
