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

        private const string CellRole = "cell";
        private const string CellClass = "nhsuk-table__cell";
        private const string CellClassNumeric = "nhsuk-table__cell--numeric";
        private const string HeadingClass = "nhsuk-table-responsive__heading";
        private const string NumericName = "numeric";

        [HtmlAttributeName(NumericName)]
        public bool Numeric { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "td";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Role, CellRole));
            if (Numeric)
            {
                output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, CellClassNumeric));
            }
            else
            {
                output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, CellClass));
            }

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
