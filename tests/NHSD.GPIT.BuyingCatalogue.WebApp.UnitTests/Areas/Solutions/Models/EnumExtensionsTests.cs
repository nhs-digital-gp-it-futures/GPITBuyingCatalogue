using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Tags;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class EnumExtensionsTests
    {
        [Theory]
        [InlineData(0, NhsTagsTagHelper.TagColour.Grey)]
        [InlineData(1, NhsTagsTagHelper.TagColour.Yellow)]
        [InlineData(2, NhsTagsTagHelper.TagColour.Green)]
        [InlineData(42, NhsTagsTagHelper.TagColour.Grey)]
        public static void TagColour_EnumInput_ReturnsExpectedColour(int status, NhsTagsTagHelper.TagColour expected)
        {
            var actual = ((FeatureCompletionStatus)status).TagColour();

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(FeatureCompletionStatus.NotStarted, "Not started")]
        [InlineData(FeatureCompletionStatus.InProgress, "In progress")]
        [InlineData(FeatureCompletionStatus.Completed, "Completed")]
        public static void Name_EnumInput_ReturnsExpectedColour(FeatureCompletionStatus status, string expected)
        {
            status.Name().Should().Be(expected);
        }
    }
}
