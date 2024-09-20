using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.DetailsAndExpander
{
    public static class DetailsAndExpanderTagHelperBuilders
    {
        public const string DetailsClass = "nhsuk-details";
        public const string CounterClass = "counter-class";
        public const string BoldTitle = "bold-title";
        public const string DetailsSummaryTextSmallClass = "details-size-reducer";

        private const string DetailsSummaryClass = "nhsuk-details__summary";
        private const string DetailsSummaryTextClass = "nhsuk-details__summary-text";
        private const string DetailsSummaryTextLetterClass = "nhsuk-details__summary-text__letter";
        private const string DetailsTextClass = "nhsuk-details__text";
        private const string PaddingTop = "nhsuk-u-padding-top-7";
        private const string SummaryTextSecondaryClass = "nhsuk-details__summary-text_secondary";
        private const string SummaryTextSecondaryUrlClass = "nhsuk-details__summary-text_secondary_url";
        private const string TagTopLevelClass = "bc-c-task-list__task-status";
        private const string TagClass = "nhsuk-tag";
        private const string TagColourClass = "nhsuk-tag--blue";
        private const string VisuallyHidden = "nhsuk-u-visually-hidden";

        public enum DetailsLabelSize
        {
            Standard,
            Small,
        }

        public static TagBuilder GetTextItem()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);
            builder.AddCssClass(DetailsTextClass);

            return builder;
        }

        public static TagBuilder GetDetailsSummaryLabelBuilder(string labelText, bool bold, DetailsLabelSize size)
        {
            var summaryBuilder = new TagBuilder("summary");
            summaryBuilder.AddCssClass(DetailsSummaryClass);

            var spanBuilder = new TagBuilder(TagHelperConstants.Span);
            spanBuilder.AddCssClass(DetailsSummaryTextClass);

            if (size == DetailsLabelSize.Small)
            {
                spanBuilder.AddCssClass(DetailsSummaryTextSmallClass);
            }

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

            summaryBuilder.InnerHtml.AppendHtml(spanBuilder);

            return summaryBuilder;
        }

        public static TagBuilder GetSummaryLabelBuilder(string headingText, string labelText, bool bold, bool addedSticker = false)
        {
            var summaryBuilder = new TagBuilder("summary");
            summaryBuilder.AddCssClass(DetailsSummaryClass);

            if (!string.IsNullOrWhiteSpace(headingText))
            {
                summaryBuilder.AddCssClass(PaddingTop);

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

            if (addedSticker)
            {
                spanBuilder.InnerHtml.AppendHtml(GetAddedStickerBuilder());
            }

            if (bold)
            {
                var boldBuilder = new TagBuilder("b");

                boldBuilder.InnerHtml.Append(labelText);
                boldBuilder.InnerHtml.AppendHtml("&nbsp");
                boldBuilder.InnerHtml.AppendHtml("&nbsp");

                spanBuilder.InnerHtml.AppendHtml(boldBuilder);
            }
            else
            {
                spanBuilder.InnerHtml.Append(labelText);
                spanBuilder.InnerHtml.AppendHtml("&nbsp");
                spanBuilder.InnerHtml.AppendHtml("&nbsp");
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

        public static TagBuilder GetSummaryLabelBuilderWithSecondaryInformation(string labelText, string secondaryTextTitle, string secondaryText, string secondaryUrlTitle, string secondaryUrl, bool bold, bool addedSticker = false, bool jsOnly = false)
        {
            var summaryBuilder = new TagBuilder("summary");
            summaryBuilder.AddCssClass(DetailsSummaryClass);

            var labelSpanBuilder = new TagBuilder(TagHelperConstants.Span);
            labelSpanBuilder.AddCssClass(DetailsSummaryTextClass);

            if (addedSticker)
            {
                labelSpanBuilder.InnerHtml.AppendHtml(GetAddedStickerBuilder());
            }

            if (bold)
            {
                var boldBuilder = new TagBuilder("b");

                boldBuilder.InnerHtml.Append(labelText);
                boldBuilder.InnerHtml.AppendHtml("&nbsp");
                boldBuilder.InnerHtml.AppendHtml("&nbsp");

                labelSpanBuilder.InnerHtml.AppendHtml(boldBuilder);
            }
            else
            {
                labelSpanBuilder.InnerHtml.Append(labelText);
                labelSpanBuilder.InnerHtml.AppendHtml("&nbsp");
                labelSpanBuilder.InnerHtml.AppendHtml("&nbsp");
            }

            var summaryClass = secondaryUrlTitle is not null ? SummaryTextSecondaryUrlClass : SummaryTextSecondaryClass;

            if (jsOnly)
                summaryClass += $" {TagHelperConstants.NhsJsOnly}";

            var secondarySpanBuilder = new TagBuilder(TagHelperConstants.Span);
            secondarySpanBuilder.AddCssClass(summaryClass);

            if (!string.IsNullOrEmpty(secondaryTextTitle))
            {
                var secondaryTitleBuilder = new TagBuilder("b");
                secondaryTitleBuilder.InnerHtml.Append(secondaryTextTitle);

                secondarySpanBuilder.InnerHtml
                    .AppendHtml(secondaryTitleBuilder)
                    .Append(secondaryText);
            }

            if (!string.IsNullOrEmpty(secondaryTextTitle) && !string.IsNullOrEmpty(secondaryUrlTitle))
            {
                var breakRow = new TagBuilder("br") { TagRenderMode = TagRenderMode.SelfClosing };
                secondarySpanBuilder.InnerHtml
                    .AppendHtml(breakRow);
            }

            if (!string.IsNullOrEmpty(secondaryUrlTitle))
            {
                var secondaryUrlBuilder = new TagBuilder(TagHelperConstants.Anchor);

                secondaryUrlBuilder.AddCssClass($"{TagHelperConstants.NhsLink} {TagHelperConstants.NhsLinkNonVisited}");
                secondaryUrlBuilder.Attributes.Add(TagHelperConstants.Link, secondaryUrl);
                secondaryUrlBuilder.Attributes.Add(TagHelperConstants.Style, "float: right;");
                secondaryUrlBuilder.InnerHtml.Append(secondaryUrlTitle);

                secondarySpanBuilder.InnerHtml
                    .AppendHtml(secondaryUrlBuilder);
            }

            summaryBuilder.InnerHtml
                .AppendHtml(labelSpanBuilder)
                .AppendHtml(secondarySpanBuilder);

            return summaryBuilder;
        }

        private static TagBuilder GetAddedStickerBuilder()
        {
            var output = new TagBuilder(TagHelperConstants.Div);

            output.AddCssClass(TagTopLevelClass);

            var inner = new TagBuilder(TagHelperConstants.Strong);

            inner.AddCssClass(TagClass);
            inner.AddCssClass(TagColourClass);
            inner.InnerHtml.Append("Added");

            output.InnerHtml.AppendHtml(inner);

            return output;
        }
    }
}
