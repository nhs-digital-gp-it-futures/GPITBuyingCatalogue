using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ViewsTagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    public class SubmitButtonTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-submit-button";
        public const string ButtonTextOverrideName = "text";

        private const string DefaultButtonText = "Save and return";

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(ButtonTextOverrideName)]
        public string Text { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var button = GetSubmitButtonBuilder();

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add(TagHelperConstants.DataTestId, "section-submit-button");
            output.Attributes.Add(TagHelperConstants.Class, $"{TagHelperConstants.NhsMarginTop}-7");

            output.Content.AppendHtml(button);
        }

        private TagBuilder GetSubmitButtonBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Button);

            builder.AddCssClass(TagHelperConstants.NhsButton);

            builder.MergeAttribute(TagHelperConstants.Type, "submit");

            builder.InnerHtml.Append(Text ?? DefaultButtonText);

            return builder;
        }
    }
}
