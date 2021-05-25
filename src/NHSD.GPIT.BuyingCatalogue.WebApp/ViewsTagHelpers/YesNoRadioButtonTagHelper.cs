using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ViewsTagHelpers
{
    [HtmlTargetElement(TagHelperName, ParentTag = FieldSetFormTagHelper.TagHelperName)]
    public class YesNoRadioButtonTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-yesno-radio-buttons";
        public const string ValueName = "value";

        private readonly IHtmlGenerator htmlGenerator;

        public YesNoRadioButtonTagHelper(IHtmlGenerator htmlGenerator) => this.htmlGenerator = htmlGenerator;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(ValueName)]
        public string Value { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                output.SuppressOutput();
                return;
            }

            var isYesChecked = For.Model?.ToString() == "Yes";

            var yesRadio = BuildYesRadio(isYesChecked);
            var noRadio = BuildNoRadio(!isYesChecked);

            output.Content.Clear();

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, TagHelperConstants.NhsRadios));

            output.Content
                .AppendHtml(yesRadio)
                .AppendHtml(noRadio);
        }

        private TagBuilder BuildYesRadio(bool isChecked)
        {
            var yesBuilder = new TagBuilder(TagHelperConstants.Div);
            yesBuilder.AddCssClass(TagHelperConstants.RadioItemClass);

            var input = TagHelperBuilders.GetRadioInputBuilder(ViewContext, For, htmlGenerator, Value, isChecked);
            var label = TagHelperBuilders.GetRadioLabelBuilder(ViewContext, For, htmlGenerator, "Yes");

            yesBuilder.InnerHtml
                .AppendHtml(input)
                .AppendHtml(label);

            return yesBuilder;
        }

        private TagBuilder BuildNoRadio(bool isChecked)
        {
            var yesBuilder = new TagBuilder(TagHelperConstants.Div);
            yesBuilder.AddCssClass(TagHelperConstants.RadioItemClass);

            var input = TagHelperBuilders.GetRadioInputBuilder(ViewContext, For, htmlGenerator, Value, isChecked);
            var label = TagHelperBuilders.GetRadioLabelBuilder(ViewContext, For, htmlGenerator, "No");

            yesBuilder.InnerHtml
                .AppendHtml(input)
                .AppendHtml(label);

            return yesBuilder;
        }
    }
}
