using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.SummaryList
{
    [HtmlTargetElement(TagHelperName, ParentTag = SummaryListTagHelper.TagHelperName)]
    public sealed class SummaryListRowTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-summary-list-row";

        private const string SummaryListRowClass = "nhsuk-summary-list__row";
        private const string SummaryListKeyClass = "nhsuk-summary-list__key";
        private const string SummaryListValueClass = "nhsuk-summary-list__value";

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, SummaryListRowClass));

            var key = GetKeyRowBuilder();

            var value = GetValueRowBuilder();

            var children = await output.GetChildContentAsync();

            value.InnerHtml.AppendHtml(children);

            output.Content
                .AppendHtml(key)
                .AppendHtml(value);
        }

        private static TagBuilder GetValueRowBuilder()
        {
            var builder = new TagBuilder("dd");

            builder.AddCssClass(SummaryListValueClass);

            return builder;
        }

        private TagBuilder GetKeyRowBuilder()
        {
            var builder = new TagBuilder("dt");

            builder.AddCssClass(SummaryListKeyClass);

            builder.InnerHtml.Append(LabelText);

            return builder;
        }
    }
}
