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

        [HtmlAttributeName(TagHelperConstants.LabelHintName)]
        public string LabelHint { get; set; }

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

            if (Status is TaskProgress.CannotStart or TaskProgress.NotApplicable)
            {
                taskNameSpan.InnerHtml.Append(LabelText);
            }
            else
            {
                taskNameSpan.InnerHtml.AppendHtml(GetLabelAnchorBuilder());
            }

            var statusTag = GetNhsTagBuilder(context);
            var labelHint = GetLabelHintBuilder();
            var breakRow = new TagBuilder("br") { TagRenderMode = TagRenderMode.SelfClosing };

            output.Content
                .AppendHtml(taskNameSpan)
                .AppendHtml(statusTag)
                .AppendHtml(breakRow)
                .AppendHtml(labelHint);
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

            builder
                .InnerHtml
                .Append(LabelText);

            return builder;
        }

        private TagBuilder GetNhsTagBuilder(TagHelperContext context)
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.AddCssClass(TagHelperContainerClass);

            var nhsTag = new NhsTagsTagHelper
            {
                ChosenTagColour = Status switch
                {
                    TaskProgress.NotApplicable => NhsTagsTagHelper.TagColour.White,
                    TaskProgress.Completed => NhsTagsTagHelper.TagColour.Green,
                    TaskProgress.InProgress => NhsTagsTagHelper.TagColour.Blue,
                    TaskProgress.Optional => NhsTagsTagHelper.TagColour.White,
                    TaskProgress.Amended => NhsTagsTagHelper.TagColour.Orange,
                    _ => NhsTagsTagHelper.TagColour.Grey,
                },

                TagText = Status switch
                {
                    TaskProgress.NotApplicable => "Not applicable",
                    TaskProgress.CannotStart => "Cannot start yet",
                    TaskProgress.Optional => "Optional",
                    TaskProgress.InProgress => "In&nbsp;progress",
                    TaskProgress.NotStarted => "Not started",
                    TaskProgress.Amended => "Amended",
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

        private TagBuilder GetLabelHintBuilder()
        {
            if (string.IsNullOrWhiteSpace(LabelHint))
                return null;

            var builder = new TagBuilder(TagHelperConstants.Span);

            const string textColour = "color: #4c6272";
            builder.MergeAttribute(TagHelperConstants.Style, textColour);
            builder.AddCssClass("bc-c-task-list__task-hint");

            builder.InnerHtml.Append(LabelHint);

            return builder;
        }
    }
}
