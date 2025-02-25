using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
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
            const string itemListItemClass = "nhsuk-task-list__item";
            const string itemListItemLinkClass = "nhsuk-task-list__item--with-link";

            output.TagName = "li";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.AddClass(itemListItemClass, HtmlEncoder.Default);

            var shouldIncludeLink = !string.IsNullOrWhiteSpace(Url)
                && (Status is not TaskProgress.CannotStart and not TaskProgress.NotApplicable);

            if (shouldIncludeLink)
                output.AddClass(itemListItemLinkClass, HtmlEncoder.Default);

            var taskNameSpan = GetTaskNameBuilder(shouldIncludeLink);

            var statusTag = GetNhsTagBuilder(context);

            output.Content
                .AppendHtml(taskNameSpan)
                .AppendHtml(statusTag);
        }

        private TagBuilder GetTaskNameBuilder(bool shouldIncludeLink)
        {
            const string itemSpanNameClass = "nhsuk-task-list__name-and-hint";
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.AddCssClass(itemSpanNameClass);

            var labelHint = GetLabelHintBuilder();
            var labelTextBuilder = GetLabelBuilder(shouldIncludeLink, labelHint is not null);

            builder.InnerHtml
                .AppendHtml(labelTextBuilder)
                .AppendHtml(labelHint!);

            return builder;
        }

        private TagBuilder GetLabelBuilder(bool shouldIncludeLink, bool hasHint)
        {
            var labelTextBuilder = shouldIncludeLink ? GetLabelAnchorBuilder() : GetLabelBuilder();

            var describedByIds = new List<string>();

            if (hasHint)
                describedByIds.Add(GetLabelHintId());

            describedByIds.Add(GetStatusId());

            labelTextBuilder.MergeAttribute(TagHelperConstants.AriaDescribedBy, string.Join(' ', describedByIds));

            return labelTextBuilder;
        }

        private TagBuilder GetLabelBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.InnerHtml.Append(LabelText);

            return builder;
        }

        private TagBuilder GetLabelAnchorBuilder()
        {
            var builder = new TagBuilder(TagHelperConstants.Anchor);
            builder.AddCssClass("nhsuk-link nhsuk-task-list__link");

            builder.MergeAttribute("href", Url);

            builder
                .InnerHtml
                .Append(LabelText);

            return builder;
        }

        private TagBuilder GetNhsTagBuilder(TagHelperContext context)
        {
            const string statusClass = "nhsuk-task-list__status";

            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.AddCssClass(statusClass);

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

            var attributeList = new TagHelperAttributeList { new(TagHelperConstants.Id, GetStatusId()), };

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

            var builder = new TagBuilder(TagHelperConstants.Div);

            const string textColour = "color: #4c6272";
            builder.MergeAttribute(TagHelperConstants.Id, GetLabelHintId());
            builder.MergeAttribute(TagHelperConstants.Style, textColour);
            builder.AddCssClass("nhsuk-task-list__hint");

            builder.InnerHtml.Append(LabelHint);

            return builder;
        }

        private string GetLabelHintId() => GetSanitizedId("hint");

        private string GetStatusId() => GetSanitizedId("status");

        private string GetSanitizedId(string component) =>
            TagBuilder.CreateSanitizedId($"{LabelText}-{component}", "_");
    }
}
