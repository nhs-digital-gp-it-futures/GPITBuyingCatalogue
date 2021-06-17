using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Table
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(
        TableRowContainerTagHelper.TagHelperName,
        TableColumnTagHelper.TagHelperName)]
    public sealed class TableContainerTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-table";

        private const string DisableHeaderName = "disable-header";

        private const string TableResponsiveClass = "nhsuk-table-responsive";
        private const string TableCaptionClass = "nhsuk-table__caption";
        private const string TableRole = "table";
        private const string TableBodyClass = "nhsuk-table__body";
        private const string HeaderClass = "nhsuk-table__head";
        private const string HeaderRole = "rowgroup";
        private const string HeaderRowRole = "row";
        private const string HeaderRowColumnRole = "columnheader";
        private const string HeaderRowColumnScope = "col";

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(DisableHeaderName)]
        public bool DisableHeader { get; set; }

        private List<TagHelperContent> ColumnNames { get; set; }

        public override void Init(TagHelperContext context)
        {
            ColumnNames = new List<TagHelperContent>();

            context.Items.Add("ColumnNames", ColumnNames);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Reinitialize("table", TagMode.StartTagAndEndTag);

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, TableResponsiveClass));
            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Role, TableRole));

            var caption = GetCaptionBuilder();
            var body = GetBodyBuilder();

            var children = await output.GetChildContentAsync();

            var header = GetHeaderBuilder(context);

            body.InnerHtml.AppendHtml(children);

            output.Content
                .AppendHtml(caption)
                .AppendHtml(header)
                .AppendHtml(body);
        }

        private static TagBuilder GetHeaderColumnBuilder(TagHelperContent columnName)
        {
            var builder = new TagBuilder("th");
            builder.MergeAttribute(TagHelperConstants.Role, HeaderRowColumnRole);
            builder.MergeAttribute(TagHelperConstants.Scope, HeaderRowColumnScope);

            builder.InnerHtml.AppendHtml(columnName);

            return builder;
        }

        private static TagBuilder GetBodyBuilder()
        {
            var builder = new TagBuilder("tbody");

            builder.AddCssClass(TableBodyClass);

            return builder;
        }

        private TagBuilder GetCaptionBuilder()
        {
            var builder = new TagBuilder("caption");

            builder.AddCssClass(TableCaptionClass);

            builder.InnerHtml.Append(LabelText);

            return builder;
        }

        private TagBuilder GetHeaderBuilder(TagHelperContext context)
        {
            if (DisableHeader)
                return null;

            if (!context.Items.TryGetValue("ColumnNames", out object columnNames))
                return null;

            var columnNamesConverted = (List<TagHelperContent>)columnNames;

            if (columnNamesConverted.Count == 0)
                return null;

            var headerBuilder = new TagBuilder("thead");

            headerBuilder.AddCssClass(HeaderClass);
            headerBuilder.MergeAttribute(TagHelperConstants.Role, HeaderRole);

            var headerRowBuilder = new TagBuilder("tr");
            headerRowBuilder.MergeAttribute(TagHelperConstants.Role, HeaderRowRole);

            foreach (var columnName in columnNamesConverted)
            {
                var column = GetHeaderColumnBuilder(columnName);
                headerRowBuilder.InnerHtml.AppendHtml(column);
            }

            headerBuilder.InnerHtml.AppendHtml(headerRowBuilder);

            return headerBuilder;
        }
    }
}
