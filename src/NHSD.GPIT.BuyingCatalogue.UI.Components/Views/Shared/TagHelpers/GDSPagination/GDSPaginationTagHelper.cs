﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.GDSPagination
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class GdsPaginationTagHelper : TagHelper
    {
        public const string TagHelperName = "govuk-pagination";

        public const string NextUrlName = "next-url";
        public const string PreviousUrlName = "previous-url";
        public const string CurrentPageNumberName = "current-page-number";
        public const string TotalNumberOfPagesName = "total-number-of-pages";

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(NextUrlName)]
        public string NextUrl { get; set; }

        [HtmlAttributeName(PreviousUrlName)]
        public string PreviousUrl { get; set; }

        [HtmlAttributeName(CurrentPageNumberName)]
        public int CurrentPageNumber { get; set; }

        [HtmlAttributeName(TotalNumberOfPagesName)]
        public int TotalNumberOfPages { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // if either the page number data isn't correctly set, suppress and return
            if (TotalNumberOfPages == 0 || CurrentPageNumber == 0)
            {
                output.SuppressOutput();
                return;
            }

            GdsPaginationBuilders.UpdatePaginationOutput(output);

            var previousLink = GetPreviousLinkBuilder();
            var nextLink = GetNextLinkBuilder();
            var numberList = GetNumberListBuilder();

            output.Content
                .AppendHtml(previousLink)
                .AppendHtml(numberList)
                .AppendHtml(nextLink);
        }

        private TagBuilder GetNumberListBuilder()
        {
            var paginationList = GdsPaginationBuilders.GetPaginationListBuilder();

            var hasEllipses = false;
            for (var i = 1; i <= TotalNumberOfPages; i++)
            {
                var floor = CurrentPageNumber - 2;

                if (CurrentPageNumber + 2 > TotalNumberOfPages)
                    floor = TotalNumberOfPages - 4;

                if (i == 1 || Enumerable.Range(Math.Max(floor, 1), 5).Contains(i) || i == TotalNumberOfPages)
                {
                    paginationList.InnerHtml.AppendHtml(GdsPaginationBuilders.GetPageLinkBuilder(i, GenerateLinkHrefForPage(i), CurrentPageNumber == i));
                    hasEllipses = false;
                }
                else
                {
                    if (hasEllipses)
                        continue;

                    paginationList.InnerHtml.AppendHtml(GdsPaginationBuilders.GetEllipsesBuilder());
                    hasEllipses = true;
                }
            }

            return paginationList;
        }

        private TagBuilder GetPreviousLinkBuilder()
        {
            if (CurrentPageNumber < 2
                || CurrentPageNumber == 0
                || TotalNumberOfPages == 0)
                return null;

            return GdsPaginationBuilders.GetPreviousArrowBuilder(GeneratePreviousLinkHref());
        }

        private TagBuilder GetNextLinkBuilder()
        {
            if (CurrentPageNumber == 0
                || TotalNumberOfPages == 0
                || CurrentPageNumber == TotalNumberOfPages)
                return null;

            return GdsPaginationBuilders.GetNextArrowBuilder(GenerateNextLinkHref());
        }

        private string GeneratePreviousLinkHref()
        {
            if (!string.IsNullOrWhiteSpace(PreviousUrl))
                return PreviousUrl;

            return GenerateLinkHrefForPage(CurrentPageNumber - 1);
        }

        private string GenerateNextLinkHref()
        {
            if (!string.IsNullOrWhiteSpace(NextUrl))
                return NextUrl;

            return GenerateLinkHrefForPage(CurrentPageNumber + 1);
        }

        private string GenerateLinkHrefForPage(int pageNumber)
        {
            var builder = new UriBuilder(ViewContext.HttpContext.Request.GetEncodedUrl())
            {
                Query = new QueryBuilder(ViewContext.HttpContext.Request.Query.Where(q => q.Key != "page"))
                {
                    { "page", pageNumber.ToString() },
                }.ToString(),
            };

            return builder.Uri.PathAndQuery;
        }
    }
}
