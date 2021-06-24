using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Tags;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.TaskList
{
    [HtmlTargetElement(TagHelperName, ParentTag = TaskListSectionTagHelper.TagHelperName)]
    public sealed class TaskListItemTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-task-list-item";

        private const string ItemStatusName = "status";
        private const string ItemUrlName = "url";

        private const string ItemSpanNameClass = "bc-c-task-list__task-name";
        private const string ItemListItemClasses = "bc-c-task-list__item nhsuk-u-padding-top-3 nhsuk-u-padding-bottom-3";
        private const string TagHelperContainerClass = "bc-c-task-list__task-status";

        public enum TaskListItemStatus
        {
            Completed = 0,
            Incomplete = 1,
            InProgress = 2,
            CannotStart = 3,
        }

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(ItemUrlName)]
        public string Url { get; set; }

        [HtmlAttributeName(ItemStatusName)]
        public TaskListItemStatus Status { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "li";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, ItemListItemClasses));

            var taskNameSpan = GetTaskNameSpanBuilder();

            if (Status == TaskListItemStatus.CannotStart)
            {
                taskNameSpan.InnerHtml.Append(LabelText);
            }
            else
            {
                taskNameSpan.InnerHtml.AppendHtml(GetLabelAnchorBuilder(context));
            }

            var statusTag = GetNhsTagBuilder(context);

            output.Content
                .AppendHtml(taskNameSpan)
                .AppendHtml(statusTag);
        }

        private static TagBuilder GetTaskNameSpanBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Span);

            builder.AddCssClass(ItemSpanNameClass);

            return builder;
        }

        private TagBuilder GetLabelAnchorBuilder(TagHelperContext context)
        {
            var builder = new TagBuilder(TagHelperConstants.Anchor);

            builder.MergeAttribute("href", Url);

            builder.MergeAttribute(TagHelperConstants.AriaDescribedBy, TagBuilder.CreateSanitizedId($"{LabelText}-status", "_"));

            builder.InnerHtml.Append(LabelText);

            return builder;
        }

        private TagBuilder GetNhsTagBuilder(TagHelperContext context)
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.AddCssClass($"{TagHelperConstants.NhsMarginRight}-9");
            builder.AddCssClass(TagHelperContainerClass);

            var nhsTag = new NhsTagsTagHelper
            {
                ChosenTagColour = Status switch
                {
                    TaskListItemStatus.Completed => NhsTagsTagHelper.TagColour.Green,
                    TaskListItemStatus.InProgress => NhsTagsTagHelper.TagColour.Yellow,
                    _ => NhsTagsTagHelper.TagColour.Grey,
                },

                TagText = Status switch
                {
                    TaskListItemStatus.CannotStart => "Cannot Start Yet",
                    TaskListItemStatus.InProgress => "In Progress",
                    TaskListItemStatus.Incomplete => "Incomplete",
                    _ => "Complete",
                },
            };

            var attributeList = new TagHelperAttributeList
            {
                new(TagHelperConstants.Id, TagBuilder.CreateSanitizedId($"{LabelText}-status", "_")),
            };

            var nhsTagOutput = new TagHelperOutput(
                string.Empty,
                attributeList,
                (_, _) => Task.Factory.StartNew<TagHelperContent>(() => new DefaultTagHelperContent()));

            nhsTag.Process(context, nhsTagOutput);

            builder.InnerHtml.AppendHtml(nhsTagOutput);

            return builder;
        }
    }
}
