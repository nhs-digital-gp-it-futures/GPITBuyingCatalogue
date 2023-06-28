using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Table
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class TableContainerTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-table";

        private const string DisableHeaderName = "disable-header";
        private const string CatchErrorsName = "catches-errors";

        private const string TableResponsiveClass = "nhsuk-table-responsive";
        private const string TableCaptionClass = "nhsuk-table__caption";
        private const string TableRole = "table";
        private const string TableBodyClass = "nhsuk-table__body";
        private const string HeaderClass = "nhsuk-table__head";
        private const string HeaderRole = "rowgroup";
        private const string HeaderRowRole = "row";
        private const string HeaderRowColumnRole = "columnheader";
        private const string HeaderRowColumnScope = "col";
        private const string HeadingClassNumeric = "nhsuk-table__header--numeric";

        private ParentChildContext parentChildContext;

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelHintName)]
        public string HintText { get; set; }

        [HtmlAttributeName(DisableHeaderName)]
        public bool DisableHeader { get; set; }

        [HtmlAttributeName(CatchErrorsName)]
        public bool CatchesErrors { get; set; } = true;

        public override void Init(TagHelperContext context)
        {
            if (CatchesErrors)
            {
                parentChildContext = new ParentChildContext();
                context.Items.Add(typeof(ParentChildContext), parentChildContext);
            }

            var columns = new List<(TagHelperContent, bool)>();

            context.Items.Add(TagHelperConstants.ColumnNameContextName, columns);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.SetAttribute(new TagHelperAttribute(TagHelperConstants.Class, TagHelperConstants.NhsFormGroup));

            var caption = GetCaptionBuilder();
            var body = GetBodyBuilder();
            var table = GetTableBuilder();

            var children = await output.GetChildContentAsync();

            var errorMessage = BuildErrorMessage();
            var header = GetHeaderBuilder(context);

            if (IsChildInError())
            {
                output.Attributes.SetAttribute(new TagHelperAttribute(
                    TagHelperConstants.Class, $"{TagHelperConstants.NhsFormGroup} {TagHelperConstants.NhsFormGroupError}"));
            }

            body.InnerHtml.AppendHtml(children);

            table.InnerHtml
                .AppendHtml(caption)
                .AppendHtml(header)
                .AppendHtml(body);

            output.Content
                .AppendHtml(errorMessage)
                .AppendHtml(table);
        }

        private static TagBuilder GetTableBuilder()
        {
            var builder = new TagBuilder("table");

            builder.AddCssClass(TableResponsiveClass);

            builder.MergeAttribute(TagHelperConstants.Role, TableRole);

            return builder;
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

            builder.InnerHtml
                .Append(LabelText)
                .AppendHtml(GetCaptionHintBuilder());

            return builder;
        }

        private TagBuilder GetCaptionHintBuilder()
        {
            if (string.IsNullOrWhiteSpace(HintText))
                return null;

            var builder = new TagBuilder("span");

            builder.AddCssClass(TagHelperConstants.NhsHint);
            builder.MergeAttribute("style", "margin-bottom:0;");

            var hiddenSpanBuilder = new TagBuilder("span");

            hiddenSpanBuilder.AddCssClass(TagHelperConstants.NhsVisuallyHidden);
            hiddenSpanBuilder.InnerHtml.Append("-");

            builder.InnerHtml
                .AppendHtml(hiddenSpanBuilder)
                .AppendHtml(HintText);

            return builder;
        }

        private TagBuilder GetHeaderBuilder(TagHelperContext context)
        {
            if (DisableHeader)
                return null;

            if (!context.Items.TryGetValue(TagHelperConstants.ColumnNameContextName, out object columns))
                return null;

            var columnsConverted = (List<(TagHelperContent Content, bool Numeric)>)columns;

            if (columnsConverted.Count == 0)
                return null;

            var headerBuilder = new TagBuilder("thead");

            headerBuilder.AddCssClass(HeaderClass);
            headerBuilder.MergeAttribute(TagHelperConstants.Role, HeaderRole);

            var headerRowBuilder = new TagBuilder("tr");
            headerRowBuilder.MergeAttribute(TagHelperConstants.Role, HeaderRowRole);

            foreach (var columnDetails in columnsConverted)
            {
                var column = GetHeaderColumnBuilder(columnDetails.Content);

                if (columnDetails.Numeric)
                {
                    column.MergeAttribute(TagHelperConstants.Class, HeadingClassNumeric);
                }

                headerRowBuilder.InnerHtml.AppendHtml(column);
            }

            headerBuilder.InnerHtml.AppendHtml(headerRowBuilder);

            return headerBuilder;
        }

        private TagBuilder BuildErrorMessage()
        {
            if (!IsChildInError())
                return null;

            var outerBuilder = new TagBuilder(TagHelperConstants.Span);

            outerBuilder.AddCssClass(TagHelperConstants.NhsErrorMessage);

            var innerBuilder = new TagBuilder(TagHelperConstants.Span);

            innerBuilder.AddCssClass("nhsuk-u-visually-hidden");
            innerBuilder.InnerHtml.Append("Error: ");

            outerBuilder.InnerHtml.AppendHtml(innerBuilder);
            outerBuilder.InnerHtml.Append(parentChildContext.ErrorMessage);

            return outerBuilder;
        }

        private bool IsChildInError()
        {
            if (parentChildContext is null)
                return false;

            return parentChildContext.ChildInError;
        }
    }
}
