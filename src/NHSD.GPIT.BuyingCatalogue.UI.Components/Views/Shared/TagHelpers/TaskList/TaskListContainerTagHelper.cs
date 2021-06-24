using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.TaskList
{
    [HtmlTargetElement(TagHelperName)]
    [RestrictChildren(TaskListSectionTagHelper.TagHelperName)]
    public sealed class TaskListContainerTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-task-list";

        public const string SectionNumberCounterName = "SectionNumberCounter";

        private const string TaskListClass = "bc-c-task-list";

        private TaskListContext SectionNumberCounter { get; set; }

        public override void Init(TagHelperContext context)
        {
            SectionNumberCounter = new TaskListContext();

            context.Items.Add(SectionNumberCounterName, SectionNumberCounter);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "ol";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, TaskListClass));

            var childContent = await output.GetChildContentAsync();

            output.Content.AppendHtml(childContent);
        }
    }
}
