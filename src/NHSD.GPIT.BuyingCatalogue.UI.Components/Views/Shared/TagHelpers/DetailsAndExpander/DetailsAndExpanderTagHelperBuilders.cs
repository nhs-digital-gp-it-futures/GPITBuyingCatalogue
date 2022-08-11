﻿using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.DetailsAndExpander
{
    public static class DetailsAndExpanderTagHelperBuilders
    {
        public const string DetailsClass = "nhsuk-details";
        public const string CounterClass = "counter-class";
        public const string BoldTitle = "bold-title";

        private const string DetailsSummaryClass = "nhsuk-details__summary";
        private const string DetailsSummaryTextClass = "nhsuk-details__summary-text";
        private const string DetailsSummaryTextLetterClass = "nhsuk-details__summary-text__letter";
        private const string DetailsTextClass = "nhsuk-details__text";
        private const string SummaryTextSecondaryClass = "nhsuk-details__summary-text_secondary";
        private const string VisuallyHidden = "nhsuk-u-visually-hidden";

        public static TagBuilder GetTextItem()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);
            builder.AddCssClass(DetailsTextClass);

            return builder;
        }

        public static TagBuilder GetSummaryLabelBuilder(string headingText, string labelText, bool bold)
        {
            var summaryBuilder = new TagBuilder("summary");
            summaryBuilder.AddCssClass(DetailsSummaryClass);

            if (!string.IsNullOrWhiteSpace(headingText))
            {
                var h3Builder = new TagBuilder(TagHelperConstants.HeaderThree);
                h3Builder.AddCssClass(DetailsSummaryTextLetterClass);

                var openingSpan = new TagBuilder(TagHelperConstants.Span);
                openingSpan.AddCssClass(VisuallyHidden);
                openingSpan.InnerHtml.Append("letter");

                var closingSpan = new TagBuilder(TagHelperConstants.Span);
                closingSpan.AddCssClass(VisuallyHidden);
                closingSpan.InnerHtml.Append("-");

                h3Builder.InnerHtml.AppendHtml(openingSpan);
                h3Builder.InnerHtml.Append(headingText);
                h3Builder.InnerHtml.AppendHtml(closingSpan);

                summaryBuilder.InnerHtml.AppendHtml(h3Builder);
            }

            var spanBuilder = new TagBuilder(TagHelperConstants.Span);
            spanBuilder.AddCssClass(DetailsSummaryTextClass);

            if (bold)
            {
                var boldBuilder = new TagBuilder("b");
                boldBuilder.InnerHtml.Append(labelText);

                spanBuilder.InnerHtml.AppendHtml(boldBuilder);
            }
            else
            {
                spanBuilder.InnerHtml.Append(labelText);
            }

            if (!string.IsNullOrWhiteSpace(headingText))
            {
                var counterSpanBuilder = new TagBuilder(TagHelperConstants.Span);
                counterSpanBuilder.AddCssClass(CounterClass);
                counterSpanBuilder.Attributes.Add("subgroup", headingText.Replace(" ", string.Empty));

                spanBuilder.InnerHtml.AppendHtml(counterSpanBuilder);
            }

            summaryBuilder.InnerHtml.AppendHtml(spanBuilder);

            return summaryBuilder;
        }

        public static TagBuilder GetSummaryLabelBuilderWithSecondaryInformation(string labelText, string secondaryTextTite, string secondaryText, bool bold)
        {
            var summaryBuilder = new TagBuilder("summary");
            summaryBuilder.AddCssClass(DetailsSummaryClass);

            var labelSpanBuilder = new TagBuilder(TagHelperConstants.Span);
            labelSpanBuilder.AddCssClass(DetailsSummaryTextClass);

            if (bold)
            {
                var boldBuilder = new TagBuilder("b");
                boldBuilder.InnerHtml.Append(labelText);

                labelSpanBuilder.InnerHtml.AppendHtml(boldBuilder);
            }
            else
            {
                labelSpanBuilder.InnerHtml.Append(labelText);
            }

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
