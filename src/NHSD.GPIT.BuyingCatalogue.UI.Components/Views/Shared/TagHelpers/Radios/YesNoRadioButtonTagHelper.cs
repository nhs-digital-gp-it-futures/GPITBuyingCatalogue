using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Radios;

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
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            var isYesChecked = For.Model?.ToString() == "Yes";

            var yesRadio = BuildYesRadio(isYesChecked);
            var noRadio = BuildNoRadio(!isYesChecked);

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, TagHelperConstants.NhsRadios));

            output.Content
                .AppendHtml(yesRadio)
                .AppendHtml(noRadio);

            TagHelperFunctions.TellParentTagIfThisTagIsInError(ViewContext, context, For);
        }

        private TagBuilder BuildYesRadio(bool isChecked)
        {
            var yesBuilder = new TagBuilder(TagHelperConstants.Div);
            yesBuilder.AddCssClass(TagHelperConstants.RadioItemClass);

            var input = RadioButtonBuilders.GetRadioInputBuilder(ViewContext, For, htmlGenerator, "Yes", 0, isChecked);
            var label = RadioButtonBuilders.GetRadioLabelBuilder(ViewContext, For, htmlGenerator, "Yes", 0);

            yesBuilder.InnerHtml
                .AppendHtml(input)
                .AppendHtml(label);

            return yesBuilder;
        }

        private TagBuilder BuildNoRadio(bool isChecked)
        {
            var yesBuilder = new TagBuilder(TagHelperConstants.Div);
            yesBuilder.AddCssClass(TagHelperConstants.RadioItemClass);

            var input = RadioButtonBuilders.GetRadioInputBuilder(ViewContext, For, htmlGenerator, "No", 1, isChecked);
            var label = RadioButtonBuilders.GetRadioLabelBuilder(ViewContext, For, htmlGenerator, "No", 1);

            yesBuilder.InnerHtml
                .AppendHtml(input)
                .AppendHtml(label);

            return yesBuilder;
        }
    }
}
