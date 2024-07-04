using System;
using System.Web;
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

        public const string BaseUrlName = "base-url";
        public const string CurrentPageNumberName = "current-page-number";
        public const string TotalNumberOfPagesName = "total-number-of-pages";

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(BaseUrlName)]
        public string BaseUrl { get; set; }

        [HtmlAttributeName(CurrentPageNumberName)]
        public int CurrentPageNumber { get; set; }

        [HtmlAttributeName(TotalNumberOfPagesName)]
        public int TotalNumberOfPages { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // if either the page number data isn't correctly set, suppress and return
            if (TotalNumberOfPages <= 1 || CurrentPageNumber == 0)
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

            var rangeFloor = Math.Max((CurrentPageNumber + 2 > TotalNumberOfPages) ? TotalNumberOfPages - 4 : CurrentPageNumber - 2, 1);
            var rangeCeiling = Math.Min(rangeFloor + 4, TotalNumberOfPages);

            if (rangeFloor > 1)
            {
                paginationList.InnerHtml.AppendHtml(GdsPaginationBuilders.GetPageLinkBuilder(1, GenerateLinkHrefForPage(1), CurrentPageNumber == 1));
                paginationList.InnerHtml.AppendHtml(GdsPaginationBuilders.GetEllipsesBuilder());
            }

            for (var i = rangeFloor; i <= rangeCeiling; i++)
            {
                paginationList.InnerHtml.AppendHtml(GdsPaginationBuilders.GetPageLinkBuilder(i, GenerateLinkHrefForPage(i), CurrentPageNumber == i));
            }

            if (rangeCeiling == TotalNumberOfPages) return paginationList;

            paginationList.InnerHtml.AppendHtml(GdsPaginationBuilders.GetEllipsesBuilder());
            paginationList.InnerHtml.AppendHtml(GdsPaginationBuilders.GetPageLinkBuilder(TotalNumberOfPages, GenerateLinkHrefForPage(TotalNumberOfPages), CurrentPageNumber == TotalNumberOfPages));

            return paginationList;
        }

        private TagBuilder GetPreviousLinkBuilder()
        {
            return CurrentPageNumber < 2
                ? null
                : GdsPaginationBuilders.GetPreviousArrowBuilder(GeneratePreviousLinkHref());
        }

        private TagBuilder GetNextLinkBuilder()
        {
            return CurrentPageNumber == TotalNumberOfPages
                ? null
                : GdsPaginationBuilders.GetNextArrowBuilder(GenerateNextLinkHref());
        }

        private string GeneratePreviousLinkHref()
        {
            return GenerateLinkHrefForPage(CurrentPageNumber - 1);
        }

        private string GenerateNextLinkHref()
        {
            return GenerateLinkHrefForPage(CurrentPageNumber + 1);
        }

        private string GenerateLinkHrefForPage(int pageNumber)
        {
            string ModifyPageParameter(string url)
            {
                var builder = new UriBuilder(url);
                var query = HttpUtility.ParseQueryString(builder.Query);
                query.Set("page", pageNumber.ToString());
                builder.Query = query.ToString() ?? string.Empty;
                return builder.Uri.PathAndQuery;
            }

            var url = string.IsNullOrWhiteSpace(BaseUrl)
                ? ViewContext.HttpContext.Request.GetEncodedUrl()
                : BaseUrl;

            return ModifyPageParameter(url);
        }
    }
}
