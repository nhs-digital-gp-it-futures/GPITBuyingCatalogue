using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.DetailsAndExpander
{
    public static class DetailsAndExpanderTagHelperBuilders
    {
        public const string DetailsClass = "nhsuk-details";

        private const string DetailsSummaryClass = "nhsuk-details__summary";
        private const string DetailsSummaryTextClass = "nhsuk-details__summary-text";
        private const string DetailsTextClass = "nhsuk-details__text";
        private const string SummaryTextSecondaryClass = "nhsuk-details__summary-text_secondary";

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

        public static TagBuilder GetSummaryLabelBuilderWithSecondaryInformation(string labelText, string secondaryTextTite, string secondaryText)
        {
            var summaryBuilder = new TagBuilder("summary");
            summaryBuilder.AddCssClass(DetailsSummaryClass);

            var labelSpanBuilder = new TagBuilder(TagHelperConstants.Span);
            labelSpanBuilder.AddCssClass(DetailsSummaryTextClass);

            labelSpanBuilder.InnerHtml.Append(labelText);

            var secondarySpanBuilder = new TagBuilder(TagHelperConstants.Span);
            secondarySpanBuilder.AddCssClass(SummaryTextSecondaryClass);

            var secondaryTitleBuilder = new TagBuilder("b");
            secondaryTitleBuilder.InnerHtml.Append(secondaryTextTite);

            secondarySpanBuilder.InnerHtml
                .AppendHtml(secondaryTitleBuilder)
                .Append(secondaryText);

            summaryBuilder.InnerHtml
                .AppendHtml(labelSpanBuilder)
                .AppendHtml(secondarySpanBuilder);

            return summaryBuilder;
        }
    }
}
