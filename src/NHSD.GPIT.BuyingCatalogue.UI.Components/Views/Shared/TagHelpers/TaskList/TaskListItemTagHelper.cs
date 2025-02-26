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

            var labelTextBuilder = GetTaskListItemBuilder(shouldIncludeLink);

            var statusTag = GetNhsTagBuilder(context);

            output.Content
                .AppendHtml(labelTextBuilder)
                .AppendHtml(statusTag);
        }

        private TagBuilder GetTaskListItemBuilder(bool shouldIncludeLink)
        {
            const string labelTextClassName = "nhsuk-task-list__name-and-hint";
            var builder = new TagBuilder(TagHelperConstants.Div);

            builder.AddCssClass(labelTextClassName);

            var labelTextBuilder = GetLabelTextBuilder(shouldIncludeLink);
            var labelHint = GetLabelHintBuilder();

            var ariaAttributes = new List<string>(2);
            if (labelHint is not null)
                ariaAttributes.Add(GetLabelHintId());

            ariaAttributes.Add(GetStatusId());

            builder.MergeAttribute(TagHelperConstants.AriaDescribedBy, string.Join(' ', ariaAttributes));

            builder.InnerHtml
                .AppendHtml(labelTextBuilder)
                .AppendHtml(labelHint!);

            return builder;
        }

        private TagBuilder GetLabelTextBuilder(bool shouldIncludeLink)
        {
            TagBuilder builder;

            if (shouldIncludeLink)
            {
                const string labelTextLinkClassName = "nhsuk-link nhsuk-task-list__link";

                builder = new TagBuilder(TagHelperConstants.Anchor);
                builder.AddCssClass(labelTextLinkClassName);

                builder.MergeAttribute("href", Url);
            }
            else
            {
                builder = new TagBuilder(TagHelperConstants.Div);
            }

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

            const string labelHintClassName = "nhsuk-task-list__hint";

            builder.MergeAttribute(TagHelperConstants.Id, GetLabelHintId());
            builder.AddCssClass(labelHintClassName);

            builder.InnerHtml.Append(LabelHint);

            return builder;
        }

        private string GetLabelHintId() => GetSanitizedId("hint");

        private string GetStatusId() => GetSanitizedId("status");

        private string GetSanitizedId(string component) =>
            TagBuilder.CreateSanitizedId($"{LabelText}-{component}", "_");
    }
}
