using System;
using EnumsNET;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Tags
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class NhsTagsTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-tag";

        private const string TagTextName = "text";
        private const string TagColourName = "colour";
        private const string TagStatusEnum = "status-enum";

        private const string NhsTagClass = "nhsuk-tag";

        private const string White = "nhsuk-tag--white";
        private const string Grey = "nhsuk-tag--grey";
        private const string Green = "nhsuk-tag--green";
        private const string AquaGreen = "nhsuk-tag--aqua-green";
        private const string Blue = "nhsuk-tag--blue";
        private const string Purple = "nhsuk-tag--purple";
        private const string Pink = "nhsuk-tag--pink";
        private const string Red = "nhsuk-tag--red";
        private const string Orange = "nhsuk-tag--orange";
        private const string Yellow = "nhsuk-tag--yellow";

        public enum TagColour
        {
            None = 0,
            DarkBlue = 1,
            White = 2,
            Grey = 3,
            Green = 4,
            AquaGreen = 5,
            Blue = 6,
            Purple = 7,
            Pink = 8,
            Red = 9,
            Orange = 10,
            Yellow = 11,
        }

        [HtmlAttributeName(TagTextName)]
        public string TagText { get; set; }

        [HtmlAttributeName(TagColourName)]
        public TagColour ChosenTagColour { get; set; }

        [HtmlAttributeName(TagStatusEnum)]
        public Enum TagStatus { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "strong";
            output.TagMode = TagMode.StartTagAndEndTag;

            if (TagStatus is null && (string.IsNullOrWhiteSpace(TagText) || ChosenTagColour == TagColour.None))
            {
                output.SuppressOutput();
                return;
            }

            if (TagStatus is not null)
            {
                (ChosenTagColour, TagText) = GetStatusFromEnum();
            }

            var selectedColourClass = ChosenTagColour switch
            {
                TagColour.White => White,
                TagColour.Grey => Grey,
                TagColour.Green => Green,
                TagColour.AquaGreen => AquaGreen,
                TagColour.Blue => Blue,
                TagColour.Purple => Purple,
                TagColour.Pink => Pink,
                TagColour.Red => Red,
                TagColour.Orange => Orange,
                TagColour.Yellow => Yellow,
                TagColour.DarkBlue or _ => string.Empty,
            };

            output.Attributes.Add(new TagHelperAttribute(TagHelperConstants.Class, $"{NhsTagClass} {selectedColourClass}"));

            output.Content.AppendHtml(TagText);
        }

        private static (TagColour SelectedColourClass, string TagText) GetAccountStatus(AccountStatus status)
        {
            var tagColour = status switch
            {
                AccountStatus.Active => TagColour.Green,
                AccountStatus.Inactive => TagColour.Grey,
                _ => TagColour.Grey,
            };

            var text = status switch
            {
                AccountStatus.Active => "Active",
                AccountStatus.Inactive => "Inactive",
                _ => string.Empty,
            };

            return (tagColour, text);
        }

        private static (TagColour SelectedColourClass, string TagText) GetTaskProgressStatus(TaskProgress progress)
        {
            var selectedColourClass = progress switch
            {
                TaskProgress.NotApplicable => TagColour.White,
                TaskProgress.NotStarted => TagColour.Grey,
                TaskProgress.CannotStart => TagColour.Grey,
                TaskProgress.Optional => TagColour.White,
                TaskProgress.InProgress => TagColour.Yellow,
                TaskProgress.Completed => TagColour.Green,
                TaskProgress.Amended => TagColour.Orange,
                _ => TagColour.Grey,
            };

            var tagText = progress switch
            {
                TaskProgress.NotApplicable => "Not&nbsp;applicable",
                TaskProgress.NotStarted => "Not&nbsp;started",
                TaskProgress.CannotStart => "Cannot&nbsp;start&nbsp;yet",
                TaskProgress.Optional => "Optional",
                TaskProgress.InProgress => "In&nbsp;progress",
                TaskProgress.Completed => "Completed",
                TaskProgress.Amended => "Amended",
                _ => "Not&nbsp;started",
            };

            return (selectedColourClass, tagText);
        }

        private static (TagColour SelectedColourClass, string TagText) GetPublicationStatus(PublicationStatus publicationStatus)
        {
            var selectedColourClass = publicationStatus switch
            {
                PublicationStatus.Draft => TagColour.White,
                PublicationStatus.InRemediation => TagColour.Yellow,
                PublicationStatus.Suspended => TagColour.Red,
                PublicationStatus.Published => TagColour.Green,
                PublicationStatus.Unpublished or _ => TagColour.Grey,
            };

            var tagText = publicationStatus.AsString(EnumFormat.DisplayName);

            return (selectedColourClass, tagText);
        }

        private static (TagColour SelectedColourClass, string TagText) GetOrderStatus(OrderStatus orderStatus)
        {
            var selectedColourClass = orderStatus switch
            {
                OrderStatus.Completed => TagColour.Green,
                OrderStatus.InProgress => TagColour.Yellow,
                OrderStatus.Deleted => TagColour.Red,
                _ => TagColour.Grey,
            };

            var tagText = orderStatus.AsString(EnumFormat.EnumMemberValue);

            return (selectedColourClass, tagText);
        }

        private (TagColour SelectedColourClass, string TagText) GetStatusFromEnum()
        {
            return TagStatus switch
            {
                OrderStatus orderStatus => GetOrderStatus(orderStatus),
                AccountStatus status => GetAccountStatus(status),
                TaskProgress progress => GetTaskProgressStatus(progress),
                PublicationStatus publicationStatus => GetPublicationStatus(publicationStatus),
                _ => throw new ArgumentOutOfRangeException(nameof(PublicationStatus)),
            };
        }
    }
}
