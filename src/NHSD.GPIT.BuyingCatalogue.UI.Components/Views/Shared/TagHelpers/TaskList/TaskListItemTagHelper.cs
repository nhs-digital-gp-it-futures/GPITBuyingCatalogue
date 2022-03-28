using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
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

        [HtmlAttributeName(TagHelperConstants.LabelTextName)]
        public string LabelText { get; set; }

        [HtmlAttributeName(ItemUrlName)]
        public string Url { get; set; }

        [HtmlAttributeName(ItemStatusName)]
        public TaskProgress Status { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "li";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, ItemListItemClasses));

            var taskNameSpan = GetTaskNameSpanBuilder();

            if (Status == TaskProgress.CannotStart)
            {
                taskNameSpan.InnerHtml.Append(LabelText);
            }
            else
            {
                taskNameSpan.InnerHtml.AppendHtml(GetLabelAnchorBuilder());
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

        private TagBuilder GetLabelAnchorBuilder()
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
                    TaskProgress.Completed => NhsTagsTagHelper.TagColour.Green,
                    TaskProgress.InProgress => NhsTagsTagHelper.TagColour.Yellow,
                    TaskProgress.Optional => NhsTagsTagHelper.TagColour.White,
                    _ => NhsTagsTagHelper.TagColour.Grey,
                },

                TagText = Status switch
                {
                    TaskProgress.CannotStart => "Cannot start yet",
                    TaskProgress.Optional => "Optional",
                    TaskProgress.InProgress => "In&nbsp;progress",
                    TaskProgress.NotStarted => "Not started",
                    _ => "Completed",
                },
            };

            var attributeList = new TagHelperAttributeList
            {
                new(TagHelperConstants.Id, TagBuilder.CreateSanitizedId($"{LabelText}-status", "_")),
            };

            var nhsTagOutput = new TagHelperOutput(
                string.Empty,
                attributeList,
                (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

            nhsTag.Process(context, nhsTagOutput);

            builder.InnerHtml.AppendHtml(nhsTagOutput);

            return builder;
        }
    }
}
