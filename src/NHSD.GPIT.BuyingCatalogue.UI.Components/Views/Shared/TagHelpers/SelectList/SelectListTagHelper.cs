using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class SelectListTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-select";

        private const string ItemsName = "asp-items";

        private const string AllowMultipleName = "allow-multiple";
        private const string NhsSelect = "nhsuk-select";

        private const string DefaultSelectListMessage = "Please select";

        private readonly IHtmlGenerator htmlGenerator;

        public SelectListTagHelper(IHtmlGenerator htmlGenerator)
        {
            this.htmlGenerator = htmlGenerator;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(ItemsName)]
        public SelectList Items { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelHintName)]
        public string LabelHint { get; set; }

        [HtmlAttributeName(AllowMultipleName)]
        public bool? AllowMultiple { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var formgroup = TagHelperBuilders.GetFormGroupBuilder();
            var label = TagHelperBuilders.GetLabelBuilder(ViewContext, For, htmlGenerator, null, LabelText);
            var errorMessage = TagHelperBuilders.GetValidationBuilder(ViewContext, For, htmlGenerator);
            var hint = TagHelperBuilders.GetLabelHintBuilder(For, LabelHint, null);
            var selectlist = GetSelectListBuilder();

            formgroup.InnerHtml
                .AppendHtml(label)
                .AppendHtml(errorMessage)
                .AppendHtml(hint)
                .AppendHtml(selectlist);

            TagHelperBuilders.UpdateOutputDiv(output, For, ViewContext, formgroup, true);
        }

        private TagBuilder GetSelectListBuilder()
        {
            return htmlGenerator.GenerateSelect(
                ViewContext,
                For.ModelExplorer,
                DefaultSelectListMessage,
                For.Name,
                Items,
                AllowMultiple == true,
                new { @class = NhsSelect });
        }
    }
}
