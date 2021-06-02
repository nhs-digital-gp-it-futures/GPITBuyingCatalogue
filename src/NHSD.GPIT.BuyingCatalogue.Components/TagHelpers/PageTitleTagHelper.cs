using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class PageTitleTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-page-title";
        public const string TitleName = "title";
        public const string TitleAdviceName = "title-advice";
        public const string TitleAdviceAdditionalName = "title-advice-additional";

        [HtmlAttributeName(TitleName)]
        public string Title { get; set; }

        [HtmlAttributeName(TitleAdviceName)]
        public string TitleAdvice { get; set; }

        [HtmlAttributeName(TitleAdviceAdditionalName)]
        public string TitleAdviceAditional { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var title = GetTitleBuilder();
            var advice = GetAdviceBuilder();
            var additional = GetAdditionalAdviceBuilder();

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = TagHelperConstants.Div;

            output.Attributes.Add(
                new TagHelperAttribute(
                    TagHelperConstants.Class,
                    $"{TagHelperConstants.NhsMarginTop}-5 {TagHelperConstants.NhsMarginBottom}-7"));

            output.Content.AppendHtml(title);
            output.Content.AppendHtml(advice);
            output.Content.AppendHtml(additional);
        }

        private TagBuilder GetTitleBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Header);

            builder.AddCssClass($"{TagHelperConstants.NhsMarginBottom}-3");

            builder.InnerHtml.Append(Title);

            return builder;
        }

        private TagBuilder GetAdviceBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.SubHeader);

            builder.AddCssClass($"{TagHelperConstants.NhsFontSize}-22");

            builder.InnerHtml.Append(TitleAdvice);

            return builder;
        }

        private TagBuilder GetAdditionalAdviceBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.InnerHtml.Append(TitleAdviceAditional);

            return builder;
        }
    }
}
