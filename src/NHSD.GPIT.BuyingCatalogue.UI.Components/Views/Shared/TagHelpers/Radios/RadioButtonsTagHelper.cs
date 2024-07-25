using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Radios;
using static NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Radios.RadioButtonBuilders;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers
{
    [HtmlTargetElement(TagHelperName, ParentTag = FieldSetFormLabelTagHelper.TagHelperName)]
    public sealed class RadioButtonsTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-radio-buttons";
        private const string InlineName = "inline";

        private readonly IHtmlGenerator htmlGenerator;

        public RadioButtonsTagHelper(IHtmlGenerator htmlGenerator) => this.htmlGenerator = htmlGenerator;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(TagHelperConstants.For)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName(TagHelperConstants.Values)]
        public IEnumerable<object> Values { get; set; }

        [HtmlAttributeName(TagHelperConstants.ValueName)]
        public string ValueName { get; set; }

        [HtmlAttributeName(TagHelperConstants.DisplayName)]
        public string DisplayName { get; set; }

        [HtmlAttributeName(TagHelperConstants.DisabledName)]
        public string DisabledName { get; set; }

        [HtmlAttributeName(TagHelperConstants.HintName)]
        public string HintName { get; set; }

        [HtmlAttributeName(TagHelperConstants.Size)]
        public RadioButtonSize Size { get; set; } = RadioButtonSize.Normal;

        [HtmlAttributeName(InlineName)]
        public bool Inline { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Values.Any() || string.IsNullOrWhiteSpace(ValueName) || string.IsNullOrWhiteSpace(DisplayName))
            {
                output.SuppressOutput();
                return;
            }

            RadioButtonBuilders.UpdateRadioContainerOutput(output, context, Size == RadioButtonSize.Small, inline:Inline);

            IEnumerable<TagBuilder> radioItems = BuildRadiosFromValueList();

            foreach (var item in radioItems)
                output.Content.AppendHtml(item);

            TagHelperFunctions.TellParentTagIfThisTagIsInError(ViewContext, context, For);
        }

        private IEnumerable<TagBuilder> BuildRadiosFromValueList() =>
            Values.Select((value, index) => RadioButtonBuilders.BuildRadioItem(ViewContext, For, htmlGenerator, value, index, ValueName, DisplayName, HintName, DisabledName));
    }
}
