using Microsoft.AspNetCore.Razor.TagHelpers;
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
        public TaskProgress? TagStatus { get; set; }

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

            output.Content.Append(TagText);
        }

        private (TagColour SelectedColourClass, string TagText) GetStatusFromEnum()
        {
            var selectedColourClass = TagStatus.Value switch
            {
                TaskProgress.NotStarted => TagColour.Grey,
                TaskProgress.CannotStartYet => TagColour.Grey,
                TaskProgress.Optional => TagColour.White,
                TaskProgress.InProgress => TagColour.Yellow,
                TaskProgress.Completed => TagColour.Green,
                _ => TagColour.Grey,
            };

            var tagText = TagStatus.Value switch
            {
                TaskProgress.NotStarted => "Not started",
                TaskProgress.CannotStartYet => "Cannot start yet",
                TaskProgress.Optional => "Optional",
                TaskProgress.InProgress => "In progress",
                TaskProgress.Completed => "Completed",
                _ => "Not started",
            };

            return (selectedColourClass, tagText);
        }
    }
}
