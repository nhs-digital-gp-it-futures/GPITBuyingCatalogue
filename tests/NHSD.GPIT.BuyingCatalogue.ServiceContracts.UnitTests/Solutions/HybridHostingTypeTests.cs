using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class HybridHostingTypeTests
    {
        [Fact]
        public static void Link_StringLengthAttribute_ExpectedMaxLength()
        {
            typeof(HybridHostingType)
                .GetProperty(nameof(HybridHostingType.Link))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should()
                .Be(1000);
        }

        [Fact]
        public static void Link_UrlAttribute_Present()
        {
            typeof(HybridHostingType)
                .GetProperty(nameof(HybridHostingType.Link))
                .GetCustomAttribute<UrlAttribute>()
                .Should().NotBeNull();
        }

        [Fact]
        public static void HostingModel_StringLengthAttribute_ExpectedMaxLength()
        {
            typeof(HybridHostingType)
                .GetProperty(nameof(HybridHostingType.HostingModel))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should()
                .Be(1000);
        }

        [Fact]
        public static void Summary_StringLengthAttribute_ExpectedMaxLength()
        {
            typeof(HybridHostingType)
                .GetProperty(nameof(HybridHostingType.Summary))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should()
                .Be(500);
        }

        [Theory]
        [InlineData("", "", "", "", TaskProgress.NotStarted)]
        [InlineData("some-link", "", "", "", TaskProgress.NotStarted)]
        [InlineData("", "", "requires-hscn", "", TaskProgress.NotStarted)]
        [InlineData("", "hosting-model", "", "", TaskProgress.NotStarted)]
        [InlineData("", "", "", "summary", TaskProgress.NotStarted)]
        [InlineData("", "hosting-model", "", "summary", TaskProgress.Completed)]
        public static void Status_ReturnsExpected(string link, string hostingModel, string requiresHscn, string summary, TaskProgress expected)
        {
            var model = new HybridHostingType { Link = link, HostingModel = hostingModel, RequiresHscn = requiresHscn, Summary = summary };

            var actual = model.Status();

            actual.Should().Be(expected);
        }
    }
}
