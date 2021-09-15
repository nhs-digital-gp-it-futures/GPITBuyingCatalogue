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
        public static void Status_DescriptionHasValidValue_ReturnsCompleted(string invalid)
        {
            var mobileConnectionDetails = Fixture.Build<MobileConnectionDetails>()
                .With(m => m.MinimumConnectionSpeed, invalid)
                .Without(m => m.ConnectionType)
                .Create();
            mobileConnectionDetails.Description.Should().NotBeNullOrWhiteSpace();

            mobileConnectionDetails.Status().Should().Be(Enums.TaskProgress.Completed);
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void Status_MinimumConnectionSpeedHasValidValue_ReturnsCompleted(string invalid)
        {
            var mobileConnectionDetails = Fixture.Build<MobileConnectionDetails>()
                .With(m => m.Description, invalid)
                .Without(m => m.ConnectionType)
                .Create();
            mobileConnectionDetails.MinimumConnectionSpeed.Should().NotBeNullOrWhiteSpace();

            mobileConnectionDetails.Status().Should().Be(Enums.TaskProgress.Completed);
        }

        [Fact]
        public static void Status_ConnectionTypeHasValidValue_ReturnsCompleted()
        {
            var mobileConnectionDetails = new MobileConnectionDetails
            {
                ConnectionType = Fixture.Create<HashSet<string>>(),
            };
            mobileConnectionDetails.Description.Should().BeNullOrEmpty();
            mobileConnectionDetails.MinimumConnectionSpeed.Should().BeNullOrEmpty();

            mobileConnectionDetails.Status().Should().Be(Enums.TaskProgress.Completed);
        }

        [Fact]
        public static void Status_AllPropertiesInvalid_ConnectionTypeNull_ReturnsNotStarted()
        {
            var mobileConnectionDetails = new MobileConnectionDetails();
            mobileConnectionDetails.ConnectionType.Should().BeNull();
            mobileConnectionDetails.Description.Should().BeNullOrEmpty();
            mobileConnectionDetails.MinimumConnectionSpeed.Should().BeNullOrEmpty();

            mobileConnectionDetails.Status().Should().Be(Enums.TaskProgress.NotStarted);
        }

        [Fact]
        public static void Status_AllPropertiesInvalid_ConnectionTypeEmpty_ReturnsNotStarted()
        {
            var mobileConnectionDetails = new MobileConnectionDetails
            {
                ConnectionType = new HashSet<string>(),
            };
            mobileConnectionDetails.Description.Should().BeNullOrEmpty();
            mobileConnectionDetails.MinimumConnectionSpeed.Should().BeNullOrEmpty();

            mobileConnectionDetails.Status().Should().Be(Enums.TaskProgress.NotStarted);
        }
    }
}
