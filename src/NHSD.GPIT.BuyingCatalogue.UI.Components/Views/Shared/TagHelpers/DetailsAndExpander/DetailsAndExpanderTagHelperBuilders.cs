using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    public static class DetailsAndExpanderTagHelperBuilders
    {
        public const string DetailsClass = "nhsuk-details";

        private const string DetailsSummaryClass = "nhsuk-details__summary";
        private const string DetailsSummaryTextClass = "nhsuk-details__summary-text";
        private const string DetailsTextClass = "nhsuk-details__text";

        public static TagBuilder GetTextItem()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);
            builder.AddCssClass(DetailsTextClass);

            return builder;
        }

        public static TagBuilder GetSummaryLabelBuilder(string labelText)
        {
            var summaryBuilder = new TagBuilder("summary");
            summaryBuilder.AddCssClass(DetailsSummaryClass);

            var spanBuilder = new TagBuilder(TagHelperConstants.Span);
            spanBuilder.AddCssClass(DetailsSummaryTextClass);

            spanBuilder.InnerHtml.Append(labelText);

            summaryBuilder.InnerHtml.AppendHtml(spanBuilder);

            return summaryBuilder;
        }
    }
}
