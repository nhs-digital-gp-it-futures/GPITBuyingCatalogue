using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ViewsTagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class PageTitleTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-page-title";
        public const string TitleName = "title";
        public const string TitleAdviceName = "title-advice";
        public const string TitleAdviceAdditionalName = "title-advice-additional";

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

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

            output.Attributes.Add(new TagHelperAttribute("class", $"{TagHelperConstants.NhsMarginTop}-5 {TagHelperConstants.NhsMarginBottom}-7"));

            output.Content.AppendHtml(title);
            output.Content.AppendHtml(advice);
            output.Content.AppendHtml(additional);
        }

        private TagBuilder GetTitleBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.H1);

            builder.AddCssClass($"{TagHelperConstants.NhsMarginBottom}-3");

            builder.MergeAttribute(TagHelperConstants.DataTestId, "section-title");

            builder.InnerHtml.Append(Title);

            return builder;
        }

        private TagBuilder GetAdviceBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.H2);

            builder.AddCssClass($"{TagHelperConstants.NhsFontSize}-22");

            builder.MergeAttribute(TagHelperConstants.DataTestId, "section-main-advice");

            builder.InnerHtml.Append(TitleAdvice);

            return builder;
        }

        private TagBuilder GetAdditionalAdviceBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.MergeAttribute(TagHelperConstants.DataTestId, "section-additional-advice");

            builder.InnerHtml.Append(TitleAdviceAditional);

            return builder;
        }


    }
}
