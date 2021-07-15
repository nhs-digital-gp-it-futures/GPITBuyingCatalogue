using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Tags;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public static class EnumExtensions
    {
        public static NhsTagsTagHelper.TagColour TagColour(this FeatureCompletionStatus status)
            => status switch
            {
                FeatureCompletionStatus.NotStarted => NhsTagsTagHelper.TagColour.Grey,
                FeatureCompletionStatus.InProgress => NhsTagsTagHelper.TagColour.Yellow,
                FeatureCompletionStatus.Completed => NhsTagsTagHelper.TagColour.Green,
                _ => NhsTagsTagHelper.TagColour.Grey,
            };

        public static string Name(this FeatureCompletionStatus status) => status.AsString(EnumFormat.DisplayName);
    }
}
