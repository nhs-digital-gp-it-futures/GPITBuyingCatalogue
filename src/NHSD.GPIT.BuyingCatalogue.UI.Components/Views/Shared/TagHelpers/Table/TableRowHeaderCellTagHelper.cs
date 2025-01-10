using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Table
{
    [HtmlTargetElement(TagHelperName, ParentTag = TableRowContainerTagHelper.TagHelperName)]
    public sealed class TableRowHeaderCellTagHelper : TagHelper
    {
        private const string TagHelperName = "nhs-table-row-header-cell";

        private const string RowHeaderRole = "rowheader";
        private const string RowHeaderScope = "row";

        private const string HeadingClass = "nhsuk-table-responsive__heading";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;

            output.TagName = "th";
            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Role, RowHeaderRole));
            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Scope, RowHeaderScope));

            var heading = GetHeadingBuilder(context);

            var children = await output.GetChildContentAsync();

            output.Content
                .AppendHtml(heading)
                .AppendHtml(children);
        }

        private static TagBuilder GetHeadingBuilder(TagHelperContext context)
        {
            if (!context.Items.TryGetValue(TagHelperConstants.CellColumnContextName, out object columnNames))
                return null;

            var columnNamesConverted = (Queue<TagHelperContent>)columnNames;

            if (columnNamesConverted.Count == 0)
                return null;

            var builder = new TagBuilder(TagHelperConstants.Span);
            builder.AddCssClass(HeadingClass);

            builder.InnerHtml.AppendHtml(columnNamesConverted.Dequeue());

            return builder;
        }
    }
}
