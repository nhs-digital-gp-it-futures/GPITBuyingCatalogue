using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class MobileMemoryAndStorageTests
    {
        private static readonly Fixture Fixture = new();

        [Theory]
        [AutoData]
        public static void IsValid_BothPropertiesValid_ReturnsTrue(MobileMemoryAndStorage mobileMemoryAndStorage)
        {
            mobileMemoryAndStorage.Description.Should().NotBeNullOrWhiteSpace();
            mobileMemoryAndStorage.MinimumMemoryRequirement.Should().NotBeNullOrWhiteSpace();

            mobileMemoryAndStorage.IsValid().Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void IsValid_DescriptionIsInvalid_ReturnsFalse(string invalid)
        {
            var mobileMemoryAndStorage = Fixture.Build<MobileMemoryAndStorage>().Without(m => m.Description).Create();
            mobileMemoryAndStorage.Description = invalid;

            mobileMemoryAndStorage.IsValid().Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void IsValid_MinimumMemoryRequirementIsInvalid_ReturnsFalse(string invalid)
        {
            var mobileMemoryAndStorage =
                Fixture.Build<MobileMemoryAndStorage>().Without(m => m.MinimumMemoryRequirement).Create();
            mobileMemoryAndStorage.MinimumMemoryRequirement = invalid;

            mobileMemoryAndStorage.IsValid().Should().BeFalse();
        }
    }
}
