using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentAssertions;
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

        [Fact]
        public static void IsValid_LinkHasValue_ReturnsTrue()
        {
            var model = new HybridHostingType { Link = "some-value" };

            var actual = model.IsValid();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void IsValid_HostingModelHasValue_ReturnsTrue()
        {
            var model = new HybridHostingType { HostingModel = "some-value" };

            var actual = model.IsValid();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void IsValid_RequiresHscnHasValue_ReturnsTrue()
        {
            var model = new HybridHostingType { RequiresHscn = "some-value" };

            var actual = model.IsValid();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void IsValid_SummaryHasValue_ReturnsTrue()
        {
            var model = new HybridHostingType { Summary = "some-value" };

            var actual = model.IsValid();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void IsValid_NoPropertyHasValue_ReturnsFalse()
        {
            var model = new HybridHostingType();

            var actual = model.IsValid();

            actual.Should().BeFalse();
        }
    }
}
