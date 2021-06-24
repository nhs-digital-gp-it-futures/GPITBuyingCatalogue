using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.TaskList
{
    [HtmlTargetElement(TagHelperName, ParentTag = TaskListContainerTagHelper.TagHelperName)]
    [RestrictChildren(TaskListItemTagHelper.TagHelperName)]
    public sealed class TaskListSectionTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-task-list-section";
        public const string SectionLabelTextName = "SectionLabelText";

        private const string TaskListHeaderClass = "bc-c-task-list__task";
        private const string TaskListHeaderSpanClass = "bc-c-task-list__task-number";
        private const string TaskListListContainerClass = "bc-c-task-list__items";

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        public override void Init(TagHelperContext context)
        {
            context.Items.Add(SectionLabelTextName, LabelText);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "li";
            output.TagMode = TagMode.StartTagAndEndTag;

            var heading = GetSectionTitleBuilder(context);
            var listContainer = GetListContainerBuilder();

            var childContent = await output.GetChildContentAsync();

            listContainer.InnerHtml.AppendHtml(childContent);

            output.Content
                .AppendHtml(heading)
                .AppendHtml(listContainer);
        }

        private static TagBuilder GetHeaderSpanBuilder(TagHelperContext context)
        {
            if (!context.Items.TryGetValue(TaskListContainerTagHelper.SectionNumberCounterName, out var taskListContext))
                return null;

            var builder = new TagBuilder(TagHelperConstants.Span);

            builder.AddCssClass(TaskListHeaderSpanClass);

            builder.InnerHtml.Append($"{((TaskListContext)taskListContext).SectionNumberCount}.");

            context.Items[TaskListContainerTagHelper.SectionNumberCounterName] = ((TaskListContext)taskListContext).SectionNumberCount++;

            return builder;
        }

        private static TagBuilder GetListContainerBuilder()
        {
            var builder = new TagBuilder("ul");

            builder.AddCssClass(TaskListListContainerClass);

            return builder;
        }

        private TagBuilder GetSectionTitleBuilder(TagHelperContext context)
        {
            var builder = new TagBuilder(TagHelperConstants.HeaderTwo);

            builder.AddCssClass(TaskListHeaderClass);

            var span = GetHeaderSpanBuilder(context);
            var label = GetLabelTextBuilder();

            builder.InnerHtml
                .AppendHtml(span)
                .AppendHtml(label);

            return builder;
        }

        private TagBuilder GetLabelTextBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.InnerHtml.Append(LabelText);

            return builder;
        }
    }
}
