﻿using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Card
{
    [HtmlTargetElement(TagHelperName)]
    public class CardTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-card";
        public const string TextName = "text";
        public const string TitleName = "title";
        public const string UrlName = "url";

        private const string CardClass = "nhsuk-card";
        private const string CardClickableClass = "nhsuk-card--clickable";
        private const string CardContentClass = "nhsuk-card__content";
        private const string CardDescriptionClass = "nhsuk-card__description";
        private const string CardHeadingClass = "nhsuk-card__heading";
        private const string CardHeadingMobileClass = "nhsuk-heading-m";
        private const string CardLinkClass = "nhsuk-card__link";

        [HtmlAttributeName(TextName)]
        public string Text { get; set; }

        [HtmlAttributeName(TitleName)]
        public string Title { get; set; }

        [HtmlAttributeName(UrlName)]
        public string Url { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.AddClass(CardClass, HtmlEncoder.Default);

            if (!string.IsNullOrWhiteSpace(Url))
            {
                output.AddClass(CardClickableClass, HtmlEncoder.Default);
            }

            output.Content.AppendHtml(BuildContent());
        }

        private TagBuilder BuildContent()
        {
            var content = new TagBuilder(TagHelperConstants.Div);

            content.AddCssClass(CardContentClass);

            content.InnerHtml
                .AppendHtml(BuildHeading())
                .AppendHtml(BuildCardText());

            return content;
        }

        private TagBuilder BuildHeading()
        {
            var heading = new TagBuilder(TagHelperConstants.HeaderTwo);

            heading.AddCssClass(CardHeadingClass);
            heading.AddCssClass(CardHeadingMobileClass);

            if (string.IsNullOrWhiteSpace(Url))
            {
                heading.InnerHtml.Append(Title);
            }
            else
            {
                heading.InnerHtml.AppendHtml(BuildHeadingLink());
            }

            return heading;
        }

        private TagBuilder BuildHeadingLink()
        {
            var link = new TagBuilder(TagHelperConstants.Anchor);

            link.AddCssClass(CardLinkClass);
            link.Attributes.Add("href", Url);
            link.InnerHtml.Append(Title);

            return link;
        }

        private TagBuilder BuildCardText()
        {
            var cardText = new TagBuilder(TagHelperConstants.Paragraph);

            cardText.AddCssClass(CardDescriptionClass);
            cardText.InnerHtml.Append(Text);

            return cardText;
        }
    }
}
