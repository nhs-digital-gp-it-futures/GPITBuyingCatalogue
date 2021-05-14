using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ViewsTagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    public class ValidationSelectListTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-validation-select";
        public const string ItemsName = "asp-items";
        public const string AllowMultipleName = "allow-multiple";

        private readonly IHtmlGenerator htmlGenerator;

        public ValidationSelectListTagHelper(IHtmlGenerator htmlGenerator)
        {
            this.htmlGenerator = htmlGenerator;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(TagHelperConstants.Items)]
        public SelectList Items { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelHintName)]
        public string LabelHint { get; set; }

        [HtmlAttributeName(TagHelperConstants.DisableLabelAndHint)]
        public bool? DisableLabelAndHint { get; set; }

        [HtmlAttributeName(AllowMultipleName)]
        public bool? AllowMultiple { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var formgroup = TagHelperBuilders.GetFormGroupBuilder(For.Name);
            var label = TagHelperBuilders.GetLabelBuilder(ViewContext, For, htmlGenerator, null, LabelText, DisableLabelAndHint);
            var hint = TagHelperBuilders.GetLabelHintBuilder(For, LabelHint, null, DisableLabelAndHint);
            var selectlist = GetSelectListBuilder();

            formgroup.InnerHtml.AppendHtml(label);
            formgroup.InnerHtml.AppendHtml(hint);
            formgroup.InnerHtml.AppendHtml(selectlist);

            TagHelperBuilders.UpdateOutputDiv(output, For, ViewContext, formgroup, true);
        }

        private TagBuilder GetSelectListBuilder()
        {
            return htmlGenerator.GenerateSelect(
                ViewContext,
                For.ModelExplorer,
                TagHelperConstants.DefaultSelectListMessage,
                For.Name,
                Items,
                AllowMultiple == true,
                new { @class = TagHelperConstants.NhsSelect });
        }
    }
}
