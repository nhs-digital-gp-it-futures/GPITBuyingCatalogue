﻿using System.Collections.Generic;
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

        private ParentChildContext parentChildContext;

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(DisableHeaderName)]
        public bool DisableHeader { get; set; }

        [HtmlAttributeName(CatchErrorsName)]
        public bool CatchesErrors { get; set; } = true;

        private List<TagHelperContent> ColumnNames { get; set; }

        public override void Init(TagHelperContext context)
        {
            if (CatchesErrors)
            {
                parentChildContext = new ParentChildContext();
                context.Items.Add(typeof(ParentChildContext), parentChildContext);
            }

            ColumnNames = new List<TagHelperContent>();

            context.Items.Add("ColumnNames", ColumnNames);
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
