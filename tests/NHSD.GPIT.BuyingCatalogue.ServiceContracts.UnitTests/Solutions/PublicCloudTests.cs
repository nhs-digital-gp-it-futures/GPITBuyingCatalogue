using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class PublicCloudTests
    {
        [Fact]
        public static void Link_StringLengthAttribute_ExpectedMaxLength()
        {
            typeof(PublicCloud)
                .GetProperty(nameof(PublicCloud.Link))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should()
                .Be(1000);
        }

        [Fact]
        public static void Link_UrlAttribute_Present()
        {
            typeof(PublicCloud)
                .GetProperty(nameof(PublicCloud.Link))
                .GetCustomAttribute<UrlAttribute>()
                .Should().NotBeNull();
        }

        [Fact]
        public static void Summary_StringLengthAttribute_ExpectedMaxLength()
        {
            typeof(PublicCloud)
                .GetProperty(nameof(PublicCloud.Summary))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should()
                .Be(500);
        }

        [Theory]
        [InlineData("", "", "", TaskProgress.NotStarted)]
        [InlineData("some-link", "", "", TaskProgress.NotStarted)]
        [InlineData("", "requires-hscn", "", TaskProgress.NotStarted)]
        [InlineData("", "", "summary", TaskProgress.Completed)]
        public static void Status_ReturnsExpected(string link, string requiresHscn, string summary, TaskProgress expected)
        {
            var model = new PublicCloud { Link = link, RequiresHscn = requiresHscn, Summary = summary };

            var actual = model.Status();

            actual.Should().Be(expected);
        }
    }
}
