using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName, ParentTag = FieldSetFormLabelTagHelper.TagHelperName)]
    public class YesNoRadioButtonTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-yesno-radio-buttons";

        private readonly IHtmlGenerator htmlGenerator;

        public YesNoRadioButtonTagHelper(IHtmlGenerator htmlGenerator) => this.htmlGenerator = htmlGenerator;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var isYesChecked = For.Model?.ToString() == "Yes";

            var yesRadio = BuildYesRadio(isYesChecked);
            var noRadio = BuildNoRadio(!isYesChecked);

            output.Reinitialize(TagHelperConstants.Div, TagMode.StartTagAndEndTag);

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, TagHelperConstants.NhsRadios));

            output.Content
                .AppendHtml(yesRadio)
                .AppendHtml(noRadio);
        }

        private TagBuilder BuildYesRadio(bool isChecked)
        {
            var yesBuilder = new TagBuilder(TagHelperConstants.Div);
            yesBuilder.AddCssClass(TagHelperConstants.RadioItemClass);

            var input = TagHelperBuilders.GetRadioInputBuilder(ViewContext, For, htmlGenerator, "Yes", isChecked);
            var label = TagHelperBuilders.GetRadioLabelBuilder(ViewContext, For, htmlGenerator, "Yes", "Yes");

            yesBuilder.InnerHtml
                .AppendHtml(input)
                .AppendHtml(label);

            return yesBuilder;
        }

        private TagBuilder BuildNoRadio(bool isChecked)
        {
            var yesBuilder = new TagBuilder(TagHelperConstants.Div);
            yesBuilder.AddCssClass(TagHelperConstants.RadioItemClass);

            var input = TagHelperBuilders.GetRadioInputBuilder(ViewContext, For, htmlGenerator, "No", isChecked);
            var label = TagHelperBuilders.GetRadioLabelBuilder(ViewContext, For, htmlGenerator, "No", "No");

            yesBuilder.InnerHtml
                .AppendHtml(input)
                .AppendHtml(label);

            return yesBuilder;
        }
    }
}
