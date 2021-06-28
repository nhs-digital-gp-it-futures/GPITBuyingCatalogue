using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class NativeDesktopThirdPartyTests
    {
        private static readonly Fixture Fixture = new();

        [Theory]
        [AutoData]
        public static void IsValid_BothPropertiesValid_ReturnsTrue(NativeDesktopThirdParty nativeDesktopThirdParty)
        {
            nativeDesktopThirdParty.DeviceCapabilities.Should().NotBeNullOrWhiteSpace();
            nativeDesktopThirdParty.ThirdPartyComponents.Should().NotBeNullOrWhiteSpace();

            nativeDesktopThirdParty.IsValid().Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void IsValid_DeviceCapabilitiesIsInvalid_ReturnsTrue(string invalid)
        {
            var mobileMemoryAndStorage =
                Fixture.Build<NativeDesktopThirdParty>().Without(m => m.DeviceCapabilities).Create();
            mobileMemoryAndStorage.DeviceCapabilities = invalid;

            mobileMemoryAndStorage.IsValid().Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void IsValid_ThirdPartyComponentsIsInvalid_ReturnsTrue(string invalid)
        {
            var mobileMemoryAndStorage =
                Fixture.Build<NativeDesktopThirdParty>().Without(m => m.ThirdPartyComponents).Create();
            mobileMemoryAndStorage.ThirdPartyComponents = invalid;

            mobileMemoryAndStorage.IsValid().Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void IsValid_BothPropertiesInvalid_ReturnsFalse(string invalid)
        {
            var mobileMemoryAndStorage = new NativeDesktopThirdParty
            {
                DeviceCapabilities = invalid,
                ThirdPartyComponents = invalid,
            };

            mobileMemoryAndStorage.IsValid().Should().BeFalse();
        }
    }
}
