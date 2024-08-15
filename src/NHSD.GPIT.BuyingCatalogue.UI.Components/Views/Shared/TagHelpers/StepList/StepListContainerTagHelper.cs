using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.StepList
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(StepListItemTagHelper.TagHelperName)]
    public sealed class StepListContainerTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-step-list";

        private const string StepListContainerClass = "bc-step-container";
        private const string StepListClass = "bc-step-list__items";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = TagHelperConstants.Div;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, StepListContainerClass));

            var listContainer = GetListContainerBuilder();
            var childContent = await output.GetChildContentAsync();
            listContainer.InnerHtml.AppendHtml(childContent);

            output.Content.AppendHtml(listContainer);
        }

        private static TagBuilder GetListContainerBuilder()
        {
            var builder = new TagBuilder("ul");

            builder.AddCssClass(StepListClass);

            return builder;
        }
    }
}
