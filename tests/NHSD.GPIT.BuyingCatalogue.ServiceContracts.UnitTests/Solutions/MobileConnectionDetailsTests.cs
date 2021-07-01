using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class MobileConnectionDetailsTests
    {
        private static readonly Fixture Fixture = new();

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void IsValid_DescriptionHasValidValue_ReturnsTrue(string invalid)
        {
            var mobileConnectionDetails = Fixture.Build<MobileConnectionDetails>()
                .With(m => m.MinimumConnectionSpeed, invalid)
                .Without(m => m.ConnectionType)
                .Create();
            mobileConnectionDetails.Description.Should().NotBeNullOrWhiteSpace();

            mobileConnectionDetails.IsValid().Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void IsValid_MinimumConnectionSpeedHasValidValue_ReturnsTrue(string invalid)
        {
            var mobileConnectionDetails = Fixture.Build<MobileConnectionDetails>()
                .With(m => m.Description, invalid)
                .Without(m => m.ConnectionType)
                .Create();
            mobileConnectionDetails.MinimumConnectionSpeed.Should().NotBeNullOrWhiteSpace();

            mobileConnectionDetails.IsValid().Should().BeTrue();
        }

        [Fact]
        public static void IsValid_ConnectionTypeHasValidValue_ReturnsTrue()
        {
            var mobileConnectionDetails = new MobileConnectionDetails
            {
                ConnectionType = Fixture.Create<HashSet<string>>(),
            };
            mobileConnectionDetails.Description.Should().BeNullOrEmpty();
            mobileConnectionDetails.MinimumConnectionSpeed.Should().BeNullOrEmpty();

            mobileConnectionDetails.IsValid().Should().BeTrue();
        }

        [Fact]
        public static void IsValid_AllPropertiesInvalid_ConnectionTypeNull_ReturnsNull()
        {
            var mobileConnectionDetails = new MobileConnectionDetails();
            mobileConnectionDetails.ConnectionType.Should().BeNull();
            mobileConnectionDetails.Description.Should().BeNullOrEmpty();
            mobileConnectionDetails.MinimumConnectionSpeed.Should().BeNullOrEmpty();

            mobileConnectionDetails.IsValid().Should().BeNull();
        }

        [Fact]
        public static void IsValid_AllPropertiesInvalid_ConnectionTypeEmpty_ReturnsFalse()
        {
            var mobileConnectionDetails = new MobileConnectionDetails
            {
                ConnectionType = new HashSet<string>(),
            };
            mobileConnectionDetails.Description.Should().BeNullOrEmpty();
            mobileConnectionDetails.MinimumConnectionSpeed.Should().BeNullOrEmpty();

            mobileConnectionDetails.IsValid().Should().BeFalse();
        }
    }
}
