using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Table
{
    [HtmlTargetElement(TagHelperName, ParentTag = TableRowContainerTagHelper.TagHelperName)]
    public sealed class TableCellTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-table-cell";

        private const string ColumnNameName = "column-name";

        private const string CellRole = "cell";
        private const string CellClass = "nhsuk-table__cell";
        private const string HeadingClass = "nhsuk-table-responsive__heading";

        [HtmlAttributeName(ColumnNameName)]
        public string ColumnName { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Reinitialize("td", TagMode.StartTagAndEndTag);

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Role, CellRole));
            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, CellClass));

            var heading = GetHeadingBuilder();

            var children = await output.GetChildContentAsync();

            output.Content
                .AppendHtml(heading)
                .AppendHtml(children);

            AddHeadingToContext(context);
        }

        private void AddHeadingToContext(TagHelperContext context)
        {
            var headingList = context.Items["ColumnNames"] as List<string>;

            if (headingList is null)
                return;

            if (!headingList.Contains(ColumnName))
                headingList.Add(ColumnName);
        }

        private TagBuilder GetHeadingBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Span);
            builder.AddCssClass(HeadingClass);

            builder.InnerHtml.Append(ColumnName);

            return builder;
        }
    }
}
