using System;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Pagination
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class PageNumberPaginationTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-page-number-pagination";

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

            PaginationBuilders.UpdatePaginationOutput(output);

            var paginationlist = PaginationBuilders.GetPaginationListBuilder();
            var previousLink = GetPreviousLinkBuilder();
            var nextLink = GetNextLinkBuilder();

            paginationlist
                .InnerHtml
                .AppendHtml(previousLink)
                .AppendHtml(nextLink);

            output.Content.AppendHtml(paginationlist);
        }

        private TagBuilder GetPreviousLinkBuilder()
        {
            if (CurrentPageNumber < 2
                || CurrentPageNumber == 0
                || TotalNumberOfPages == 0)
                return null;

            return PaginationBuilders.GetLinkBuilder(
                TagHelperConstants.NhsPaginationItemPrevious,
                TagHelperConstants.NhsPaginationLinkPrevious,
                GeneratePreviousLinkHref(),
                "Previous",
                GenerateNumberPageText(CurrentPageNumber - 1),
                TagHelperConstants.NhsIconArrowLeft,
                PaginationBuilders.PreviousArrowPath);
        }

        private TagBuilder GetNextLinkBuilder()
        {
            if (CurrentPageNumber == 0
                || TotalNumberOfPages == 0
                || CurrentPageNumber == TotalNumberOfPages)
                return null;

            return PaginationBuilders.GetLinkBuilder(
            TagHelperConstants.NhsPaginationItemNext,
            TagHelperConstants.NhsPaginationLinkNext,
            GenerateNextLinkHref(),
            "Next",
            GenerateNumberPageText(CurrentPageNumber + 1),
            TagHelperConstants.NhsIconArrowRight,
            PaginationBuilders.NextArrowPath);
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

        private string GenerateNumberPageText(int targetPageNumber) => $"{targetPageNumber} of {TotalNumberOfPages}";
    }
}
