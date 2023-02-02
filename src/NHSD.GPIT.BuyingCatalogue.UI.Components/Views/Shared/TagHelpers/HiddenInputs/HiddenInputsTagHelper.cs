using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.HiddenInputs
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class HiddenInputsTagHelper : TagHelper
    {
        public const string TagHelperName = "hidden-inputs";

        public const string TitleName = "title";
        public const string CaptionName = "caption";
        public const string AdviceName = "advice";
        public const string AdditionalAdviceName = "additional-advice";

        private readonly IHtmlGenerator htmlGenerator;

        public HiddenInputsTagHelper(IHtmlGenerator htmlGenerator)
        {
            this.htmlGenerator = htmlGenerator;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TitleName)]
        public ModelExpression Title { get; set; }

        [HtmlAttributeName(CaptionName)]
        public ModelExpression Caption { get; set; }

        [HtmlAttributeName(AdviceName)]
        public ModelExpression Advice { get; set; }

        [HtmlAttributeName(AdditionalAdviceName)]
        public ModelExpression AdditionalAdvice { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            if (Title?.Model != null)
            {
                output.Content.AppendHtml(BuildHiddenInput(Title));
            }

            if (Caption?.Model != null)
            {
                output.Content.AppendHtml(BuildHiddenInput(Caption));
            }

            if (Advice?.Model != null)
            {
                output.Content.AppendHtml(BuildHiddenInput(Advice));
            }

            if (AdditionalAdvice?.Model != null)
            {
                output.Content.AppendHtml(BuildHiddenInput(AdditionalAdvice));
            }
        }

        private TagBuilder BuildHiddenInput(ModelExpression modelExpression)
        {
            return htmlGenerator.GenerateHidden(
                ViewContext,
                modelExpression.ModelExplorer,
                modelExpression.Name,
                value: modelExpression.Model,
                useViewData: false,
                htmlAttributes: null);
        }
    }
}
