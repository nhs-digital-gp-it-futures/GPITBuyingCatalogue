using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
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
        private const string SummaryListActionsClass = "nhsuk-summary-list__actions";

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(TagHelperConstants.SummaryListAction)]
        public (string Text, string Link) Action { get; set; }

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

            if (Action is not (null, null))
                output.Content.AppendHtml(GetActionRowBuilder());
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

        private TagBuilder GetActionRowBuilder()
        {
            var builder = new TagBuilder("dd");

            builder.AddCssClass(SummaryListActionsClass);

            var hyperlink = new TagBuilder(TagHelperConstants.Anchor);
            hyperlink.MergeAttribute("href", Action.Link);
            hyperlink.InnerHtml.Append(Action.Text);

            builder.InnerHtml.AppendHtml(hyperlink);

            return builder;
        }
    }
}
