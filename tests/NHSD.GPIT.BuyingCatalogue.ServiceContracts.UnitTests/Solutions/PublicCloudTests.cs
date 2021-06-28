using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentAssertions;
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

        [Fact]
        public static void IsValid_LinkHasValue_ReturnsTrue()
        {
            var model = new PublicCloud { Link = "some-value" };

            var actual = model.IsValid();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void IsValid_RequiresHscnHasValue_ReturnsTrue()
        {
            var model = new PublicCloud { RequiresHscn = "some-value" };

            var actual = model.IsValid();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void IsValid_SummaryHasValue_ReturnsTrue()
        {
            var model = new PublicCloud { Summary = "some-value" };

            var actual = model.IsValid();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void IsValid_NoPropertyHasValue_ReturnsFalse()
        {
            var model = new PublicCloud();

            var actual = model.IsValid();

            actual.Should().BeFalse();
        }
    }
}
