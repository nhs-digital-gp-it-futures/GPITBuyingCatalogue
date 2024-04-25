using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.GDSPagination
{
    public static class GdsPaginationBuilders
    {
        public const string PreviousArrowPath = "m6.5938-0.0078125-6.7266 6.7266 6.7441 6.4062 1.377-1.449-4.1856-3.9768h12.896v-2h-12.984l4.2931-4.293-1.414-1.414z";
        public const string NextArrowPath = "m8.107-0.0078125-1.4136 1.414 4.2926 4.293h-12.986v2h12.896l-4.1855 3.9766 1.377 1.4492 6.7441-6.4062-6.7246-6.7266z";
        private const string AriaLabel = "Pagination";
        private const string XmlnsValue = "http://www.w3.org/2000/svg";
        private const string ViewBoxValue = "0 0 15 13";

        public static void UpdatePaginationOutput(TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Nav;
            output.TagMode = TagMode.StartTagAndEndTag;

            var attributes = new List<TagHelperAttribute>
            {
                new TagHelperAttribute(TagHelperConstants.Class, TagHelperConstants.GovUkPagination),
                new TagHelperAttribute(TagHelperConstants.AriaLabel, AriaLabel),
            };

            attributes.ForEach(a => output.Attributes.Add(a));
        }

        public static TagBuilder GetPageLinkBuilder(
            int pageNumber,
            string href,
            bool isCurrent = false)
        {
            var listItem = new TagBuilder(TagHelperConstants.ListItem);

            listItem.AddCssClass(TagHelperConstants.GovUkPaginationItem);

            if (isCurrent)
                listItem.AddCssClass(TagHelperConstants.GovUkPaginationCurrent);

            var link = GetNumberLinkBuilder(href, pageNumber, isCurrent);

            listItem.InnerHtml.AppendHtml(link);

            return listItem;
        }

        public static TagBuilder GetEllipsesBuilder()
        {
            var listItem = new TagBuilder(TagHelperConstants.ListItem);

            listItem.AddCssClass(TagHelperConstants.GovUkPaginationItem);
            listItem.AddCssClass(TagHelperConstants.GovUkPaginationEllipses);

            listItem.InnerHtml.Append("...");

            return listItem;
        }

        public static TagBuilder GetPaginationListBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.UnorderedList);

            builder.AddCssClass(TagHelperConstants.GovUkPaginationList);

            return builder;
        }

        public static TagBuilder GetPreviousArrowBuilder(string href)
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.AddCssClass(TagHelperConstants.GovUkPaginationPrevious);

            var link = GetArrowLinkBuilder(href, "Previous", TagHelperConstants.GovUkPaginationPreviousIcon, PreviousArrowPath);
            builder.InnerHtml
                .AppendHtml(link);
            return builder;
        }

        public static TagBuilder GetNextArrowBuilder(string href)
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.AddCssClass(TagHelperConstants.GovUkPaginationNext);

            var link = GetArrowLinkBuilder(href, "Next", TagHelperConstants.GovUkPaginationNextIcon, NextArrowPath);
            builder.InnerHtml
                .AppendHtml(link);
            return builder;
        }

        private static TagBuilder GetNumberLinkBuilder(string href, int pageNumber, bool isCurrent = false)
        {
            var builder = GetLinkBuilder(href);

            var ariaLabel = "Page " + pageNumber;
            builder.MergeAttribute(TagHelperConstants.AriaLabel, ariaLabel);

            if (isCurrent)
                builder.MergeAttribute(TagHelperConstants.AriaCurrent, "page");

            builder.InnerHtml.Append(pageNumber.ToString());
            return builder;
        }

        private static TagBuilder GetArrowLinkBuilder(string href, string value, string svgClass, string arrowSvgPath)
        {
            var builder = GetLinkBuilder(href);

            builder.MergeAttribute("rel", value.ToLower());

            var title = GetArrowTitleBuilder(value);
            var svg = GetArrowSvgBuilder(svgClass, arrowSvgPath);

            builder.InnerHtml
                .AppendHtml(title)
                .AppendHtml(svg);

            return builder;
        }

        private static TagBuilder GetArrowTitleBuilder(string title)
        {
            var builder = new TagBuilder(TagHelperConstants.Span);

            builder.AddCssClass(TagHelperConstants.GovUkPaginationTitle);
            builder.InnerHtml.Append(title);
            builder.InnerHtml.AppendHtml(GetHiddenPageBuilder());

            return builder;
        }

        private static TagBuilder GetHiddenPageBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Span);

            builder.AddCssClass(TagHelperConstants.NhsVisuallyHidden);
            builder.InnerHtml.Append(" page");

            return builder;
        }

        private static TagBuilder GetArrowSvgBuilder(string arrowClass, string arrowSvgPath)
        {
            var builder = new TagBuilder(TagHelperConstants.Svg);

            builder.AddCssClass(TagHelperConstants.GovUkPaginationIcon);
            builder.AddCssClass(arrowClass);
            builder.MergeAttribute(TagHelperConstants.Xmlns, XmlnsValue);
            builder.MergeAttribute(TagHelperConstants.ViewBox, ViewBoxValue);
            builder.MergeAttribute(TagHelperConstants.AriaHidden, "true");
            builder.MergeAttribute(TagHelperConstants.Focusable, "false");

            var path = GetArrowSvgPathBuilder(arrowSvgPath);
            builder.InnerHtml.AppendHtml(path);

            return builder;
        }

        private static TagBuilder GetArrowSvgPathBuilder(string path)
        {
            var builder = new TagBuilder(TagHelperConstants.Path);
            builder.MergeAttribute(TagHelperConstants.PathD, path);

            return builder;
        }

        private static TagBuilder GetLinkBuilder(string href)
        {
            var builder = new TagBuilder(TagHelperConstants.Anchor);

            builder.MergeAttribute(TagHelperConstants.Link, href);

            builder.AddCssClass(TagHelperConstants.GovUkPaginationLink);

            return builder;
        }
    }
}
