using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class PrivateCloudTests
    {
        [Fact]
        public static void Link_StringLengthAttribute_ExpectedMaxLength()
        {
            typeof(PrivateCloud)
                .GetProperty(nameof(PrivateCloud.Link))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should()
                .Be(1000);
        }

        [Fact]
        public static void Link_UrlAttribute_Present()
        {
            typeof(PrivateCloud)
                .GetProperty(nameof(PrivateCloud.Link))
                .GetCustomAttribute<UrlAttribute>()
                .Should().NotBeNull();
        }

        [Fact]
        public static void HostingModel_StringLengthAttribute_ExpectedMaxLength()
        {
            typeof(PrivateCloud)
                .GetProperty(nameof(PrivateCloud.HostingModel))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should()
                .Be(1000);
        }

        [Fact]
        public static void Summary_StringLengthAttribute_ExpectedMaxLength()
        {
            typeof(PrivateCloud)
                .GetProperty(nameof(PrivateCloud.Summary))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should()
                .Be(500);
        }

        [Fact]
        public static void IsValid_LinkHasValue_ReturnsTrue()
        {
            var model = new PrivateCloud { Link = "some-value" };

            var actual = model.IsValid();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void IsValid_HostingModelHasValue_ReturnsTrue()
        {
            var model = new PrivateCloud { HostingModel = "some-value" };

            var actual = model.IsValid();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void IsValid_RequiresHscnHasValue_ReturnsTrue()
        {
            var model = new PrivateCloud { RequiresHscn = "some-value" };

            var actual = model.IsValid();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void IsValid_SummaryHasValue_ReturnsTrue()
        {
            var model = new PrivateCloud { Summary = "some-value" };

            var actual = model.IsValid();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void IsValid_NoPropertyHasValue_ReturnsFalse()
        {
            var model = new PrivateCloud();

            var actual = model.IsValid();

            actual.Should().BeFalse();
        }
    }
}
