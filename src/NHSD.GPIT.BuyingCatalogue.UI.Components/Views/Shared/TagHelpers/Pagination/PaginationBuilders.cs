using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Pagination
{
    public static class PaginationBuilders
    {
        public const string PreviousArrowPath = "M4.1 12.3l2.7 3c.2.2.5.2.7 0 .1-.1.1-.2.1-.3v-2h11c.6 0 1-.4 1-1s-.4-1-1-1h-11V9c0-.2-.1-.4-.3-.5h-.2c-.1 0-.3.1-.4.2l-2.7 3c0 .2 0 .4.1.6z";
        public const string NextArrowPath = "M19.6 11.66l-2.73-3A.51.51 0 0 0 16 9v2H5a1 1 0 0 0 0 2h11v2a.5.5 0 0 0 .32.46.39.39 0 0 0 .18 0 .52.52 0 0 0 .37-.16l2.73-3a.5.5 0 0 0 0-.64z";
        private const string NavRole = "navigation";
        private const string AriaLabel = "Pagination";
        private const string XmlnsValue = "http://www.w3.org/2000/svg";
        private const string ViewBoxValue = "0 0 24 24";

        public static void UpdatePaginationOutput(TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Nav;
            output.TagMode = TagMode.StartTagAndEndTag;

            var attributes = new List<TagHelperAttribute>
            {
                new TagHelperAttribute(TagHelperConstants.Class, TagHelperConstants.NhsPagination),
                new TagHelperAttribute(TagHelperConstants.Role, NavRole),
                new TagHelperAttribute(TagHelperConstants.AriaLabel, AriaLabel),
            };

            attributes.ForEach(a => output.Attributes.Add(a));
        }

        public static TagBuilder GetPaginationSvgPathBuilder(string path)
        {
            var builder = new TagBuilder(TagHelperConstants.Path);
            builder.MergeAttribute(TagHelperConstants.PathD, path);

            return builder;
        }

        public static TagBuilder GetLinkBuilder(
            string listItemClass,
            string linkBuilderClass,
            string href,
            string titleText,
            string pageSubText,
            string svgClass,
            string arrowSvgPath)
        {
            var listItem = new TagBuilder(TagHelperConstants.ListItem);

            listItem.AddCssClass(listItemClass);

            var link = GetPaginationLinkBuilder(linkBuilderClass, href);

            var title = GetPaginationTitleBuilder(titleText);
            var colon = GetPaginationColonBuilder();
            var page = GetPaginationPageBuilder(pageSubText);
            var svg = GetPaginationSvgBuilder(svgClass);
            var path = GetPaginationSvgPathBuilder(arrowSvgPath);

            svg.InnerHtml.AppendHtml(path);

            link.InnerHtml
                .AppendHtml(title)
                .AppendHtml(colon)
                .AppendHtml(page)
                .AppendHtml(svg);

            listItem.InnerHtml.AppendHtml(link);

            return listItem;
        }

        public static TagBuilder GetPaginationListBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.UnorderedList);

            builder.AddCssClass(TagHelperConstants.NhsList);
            builder.AddCssClass(TagHelperConstants.NhsPaginationList);

            return builder;
        }

        private static TagBuilder GetPaginationTitleBuilder(string title)
        {
            var builder = new TagBuilder(TagHelperConstants.Span);

            builder.AddCssClass(TagHelperConstants.NhsPaginationTitle);
            builder.InnerHtml.Append(title);

            return builder;
        }

        private static TagBuilder GetPaginationColonBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Span);

            builder.AddCssClass(TagHelperConstants.NhsVisuallyHidden);
            builder.InnerHtml.Append(":");

            return builder;
        }

        private static TagBuilder GetPaginationPageBuilder(string text)
        {
            var builder = new TagBuilder(TagHelperConstants.Span);

            builder.AddCssClass(TagHelperConstants.NhsPaginationPage);
            builder.InnerHtml.Append(text);

            return builder;
        }

        private static TagBuilder GetPaginationLinkBuilder(string linkClass, string href)
        {
            var builder = new TagBuilder(TagHelperConstants.Anchor);

            builder.MergeAttribute("href", href);
            builder.AddCssClass(TagHelperConstants.NhsPaginationLink);
            builder.AddCssClass(linkClass);

            return builder;
        }

        private static TagBuilder GetPaginationSvgBuilder(string arrowClass)
        {
            var builder = new TagBuilder(TagHelperConstants.Svg);

            builder.AddCssClass(TagHelperConstants.NhsIcon);
            builder.AddCssClass(arrowClass);
            builder.MergeAttribute(TagHelperConstants.Xmlns, XmlnsValue);
            builder.MergeAttribute(TagHelperConstants.ViewBox, ViewBoxValue);
            builder.MergeAttribute(TagHelperConstants.AriaHidden, "true");

            return builder;
        }
    }
}
