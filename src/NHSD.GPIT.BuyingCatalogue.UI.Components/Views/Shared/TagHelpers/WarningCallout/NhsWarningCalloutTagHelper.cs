using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.WarningCallout
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class NhsWarningCalloutTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-warning-callout";

        private const string WarningCalloutClass = "nhsuk-warning-callout";
        private const string WarningCalloutClassSmaller = "nhsuk-warning-callout--smaller";
        private const string WarningCalloutLabelClass = "nhsuk-warning-callout__label";

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var noLabelSmallerWarning = string.IsNullOrWhiteSpace(LabelText);

            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            var classToUse = noLabelSmallerWarning ? WarningCalloutClassSmaller : WarningCalloutClass;
            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, classToUse));

            var children = await output.GetChildContentAsync();

            if (noLabelSmallerWarning)
            {
                output.Content.AppendHtml(children);
            }
            else
            {
                var label = GetHeaderLabelBuilder();
                var hiddenSpan = TagHelperBuilders.GetVisuallHiddenSpanClassBuilder();
                var roleSpan = GetRoleSpanBuilder();
                var contentContainer = GetContentContainer();

                roleSpan.InnerHtml
                    .AppendHtml(hiddenSpan)
                    .Append(LabelText);
                label.InnerHtml.AppendHtml(roleSpan);

                contentContainer.InnerHtml.AppendHtml(children);
                output.Content
                    .AppendHtml(label)
                    .AppendHtml(contentContainer);
            }
        }

        private static TagBuilder GetHeaderLabelBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.HeaderThree);
            builder.AddCssClass(WarningCalloutLabelClass);

            return builder;
        }

        private static TagBuilder GetRoleSpanBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Span);
            builder.MergeAttribute(TagHelperConstants.Role, "text");

            return builder;
        }

        private static TagBuilder GetContentContainer()
        {
            return new TagBuilder(TagHelperConstants.Paragraph);
        }
    }
}
