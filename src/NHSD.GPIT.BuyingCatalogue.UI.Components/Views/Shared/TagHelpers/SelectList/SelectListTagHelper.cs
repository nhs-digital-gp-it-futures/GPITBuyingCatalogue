using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName)]
    public class SelectListTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-select";

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

        [HtmlAttributeName(TagHelperConstants.ItemsName)]
        public IEnumerable<SelectOption<string>> Items { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(TagHelperConstants.LabelHintName)]
        public string LabelHint { get; set; }

        [HtmlAttributeName(TagHelperConstants.AllowMultipleName)]
        public bool? AllowMultiple { get; set; }

        [HtmlAttributeName(TagHelperConstants.UseDefaultValue)]
        public bool? UseDefaultValue { get; set; } = true;

        [HtmlAttributeName(TagHelperConstants.DefaultValue)]
        public string DefaultValue { get; set; } = "Please select";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var formGroup = BuildSelect();

            TagHelperBuilders.UpdateOutputDiv(output, For, ViewContext, formGroup, true);
        }

        protected TagBuilder BuildSelect()
        {
            var formGroup = TagHelperBuilders.GetFormGroupBuilder();
            var label = TagHelperBuilders.GetLabelBuilder(ViewContext, For, htmlGenerator, null, LabelText);
            var errorMessage = TagHelperBuilders.GetValidationBuilder(ViewContext, For, htmlGenerator);
            var hint = TagHelperBuilders.GetLabelHintBuilder(For, LabelHint);

            var selectList = TagHelperBuilders.GetSelectListBuilder(
                htmlGenerator,
                ViewContext,
                For,
                new SelectList(Items, "Value", "Text", Items.FirstOrDefault(x => x.Selected)),
                DefaultValue,
                AllowMultiple,
                UseDefaultValue);

            formGroup.InnerHtml
                .AppendHtml(label)
                .AppendHtml(errorMessage)
                .AppendHtml(hint)
                .AppendHtml(selectList);

            return formGroup;
        }
    }
}
