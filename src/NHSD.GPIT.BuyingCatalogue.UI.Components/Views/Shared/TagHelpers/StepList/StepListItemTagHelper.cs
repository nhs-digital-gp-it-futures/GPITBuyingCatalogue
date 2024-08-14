using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.StepList
{
    [HtmlTargetElement(TagHelperName, ParentTag = StepListContainerTagHelper.TagHelperName)]
    public sealed class StepListItemTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-step-list-item";

        private const string ItemHeaderClass = "bc-step-list__header";

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "li";
            output.TagMode = TagMode.StartTagAndEndTag;

            var header = GetStepHeaderBuilder();
            header.InnerHtml.Append(LabelText);

            var body = GetBodyBuilder();
            var childContent = await output.GetChildContentAsync();
            body.InnerHtml.AppendHtml(childContent);

            output.Content
                .AppendHtml(header)
                .AppendHtml(body);
        }

        private static TagBuilder GetStepHeaderBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.AddCssClass(ItemHeaderClass);

            return builder;
        }

        private static TagBuilder GetBodyBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Paragraph);

            builder.AddCssClass(TagHelperConstants.NhsBody);

            return builder;
        }
    }
}
