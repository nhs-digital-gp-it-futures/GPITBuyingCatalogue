using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Pagination
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class PageLinkPaginationTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-page-link-pagination";

        public const string NextUrlName = "next-url";
        public const string PreviousUrlName = "previous-url";
        public const string NextPageSubTextName = "next-subtext";
        public const string PreviousPageSubTextName = "previous-subtext";

        [HtmlAttributeName(NextUrlName)]
        public string NextUrl { get; set; }

        [HtmlAttributeName(PreviousUrlName)]
        public string PreviousUrl { get; set; }

        [HtmlAttributeName(NextPageSubTextName)]
        public string NextPageSubText { get; set; }

        [HtmlAttributeName(PreviousPageSubTextName)]
        public string PreviousPageSubText { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // since each arrow requires 2 pieces of data, if both arrows don't have complete data, suppress and return
            if ((string.IsNullOrWhiteSpace(NextUrl) || string.IsNullOrWhiteSpace(NextPageSubText))
                && (string.IsNullOrWhiteSpace(PreviousUrl) || string.IsNullOrWhiteSpace(PreviousPageSubText)))
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
            if (string.IsNullOrWhiteSpace(PreviousUrl)
                || string.IsNullOrWhiteSpace(PreviousPageSubText))
                return null;

            return PaginationBuilders.GetLinkBuilder(
                TagHelperConstants.NhsPaginationItemPrevious,
                TagHelperConstants.NhsPaginationLinkPrevious,
                PreviousUrl,
                "Previous",
                PreviousPageSubText,
                TagHelperConstants.NhsIconArrowLeft,
                PaginationBuilders.PreviousArrowPath);
        }

        private TagBuilder GetNextLinkBuilder()
        {
            if (string.IsNullOrWhiteSpace(NextUrl)
                || string.IsNullOrWhiteSpace(NextPageSubText))
                return null;

            return PaginationBuilders.GetLinkBuilder(
            TagHelperConstants.NhsPaginationItemNext,
            TagHelperConstants.NhsPaginationLinkNext,
            NextUrl,
            "Next",
            NextPageSubText,
            TagHelperConstants.NhsIconArrowRight,
            PaginationBuilders.NextArrowPath);
        }
    }
}
