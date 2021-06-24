using Microsoft.AspNetCore.Razor.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Tags
{
    [HtmlTargetElement(TagHelperName)]
    public sealed class NhsTagsTagHelper : TagHelper
    {
        public const string TagHelperName = "nhs-tag";

        private const string TagTextName = "text";
        private const string TagColourName = "colour";

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
            DarkBlue = 0,
            White = 1,
            Grey = 2,
            Green = 3,
            AquaGreen = 4,
            Blue = 5,
            Purple = 6,
            Pink = 7,
            Red = 8,
            Orange = 9,
            Yellow = 10,
        }

        [HtmlAttributeName(TagTextName)]
        public string TagText { get; set; }

        [HtmlAttributeName(TagColourName)]
        public TagColour ChosenTagColour { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "strong";
            output.TagMode = TagMode.StartTagAndEndTag;

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
    }
}
