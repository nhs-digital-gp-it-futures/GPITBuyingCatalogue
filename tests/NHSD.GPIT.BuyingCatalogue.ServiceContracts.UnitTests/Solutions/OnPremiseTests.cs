using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class OnPremiseTests
    {
        [Fact]
        public static void Link_StringLengthAttribute_ExpectedMaxLength()
        {
            typeof(OnPremise)
                .GetProperty(nameof(OnPremise.Link))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should()
                .Be(1000);
        }

        [Fact]
        public static void Link_UrlAttribute_Present()
        {
            typeof(OnPremise)
                .GetProperty(nameof(OnPremise.Link))
                .GetCustomAttribute<UrlAttribute>()
                .Should().NotBeNull();
        }

        [Fact]
        public static void HostingModel_StringLengthAttribute_ExpectedMaxLength()
        {
            typeof(OnPremise)
                .GetProperty(nameof(OnPremise.HostingModel))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should()
                .Be(1000);
        }

        [Fact]
        public static void Summary_StringLengthAttribute_ExpectedMaxLength()
        {
            typeof(OnPremise)
                .GetProperty(nameof(OnPremise.Summary))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should()
                .Be(500);
        }

        [Fact]
        public static void IsValid_LinkHasValue_ReturnsTrue()
        {
            var model = new OnPremise { Link = "some-value", };

            var actual = model.IsValid();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void IsValid_HostingModelHasValue_ReturnsTrue()
        {
            var model = new OnPremise { HostingModel = "some-value", };

            var actual = model.IsValid();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void IsValid_RequiresHscnHasValue_ReturnsTrue()
        {
            var model = new OnPremise { RequiresHscn = "some-value", };

            var actual = model.IsValid();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void IsValid_SummaryHasValue_ReturnsTrue()
        {
            var model = new OnPremise { Summary = "some-value", };

            var actual = model.IsValid();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void IsValid_NoPropertyHasValue_ReturnsFalse()
        {
            var model = new OnPremise();

            var actual = model.IsValid();

            actual.Should().BeFalse();
        }
    }
}
