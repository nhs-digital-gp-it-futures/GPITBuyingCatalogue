using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class NativeDesktopThirdPartyTests
    {
        private static readonly Fixture Fixture = new();
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };

        [Test, AutoData]
        public static void IsValid_BothPropertiesValid_ReturnsTrue(NativeDesktopThirdParty nativeDesktopThirdParty)
        {
            nativeDesktopThirdParty.DeviceCapabilities.Should().NotBeNullOrWhiteSpace();
            nativeDesktopThirdParty.ThirdPartyComponents.Should().NotBeNullOrWhiteSpace();

            nativeDesktopThirdParty.IsValid().Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void IsValid_DeviceCapabilitiesIsInvalid_ReturnsTrue(string invalid)
        {
            var mobileMemoryAndStorage =
                Fixture.Build<NativeDesktopThirdParty>().Without(m => m.DeviceCapabilities).Create();
            mobileMemoryAndStorage.DeviceCapabilities = invalid;

            mobileMemoryAndStorage.IsValid().Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void IsValid_ThirdPartyComponentsIsInvalid_ReturnsTrue(string invalid)
        {
            var mobileMemoryAndStorage =
                Fixture.Build<NativeDesktopThirdParty>().Without(m => m.ThirdPartyComponents).Create();
            mobileMemoryAndStorage.ThirdPartyComponents = invalid;

            mobileMemoryAndStorage.IsValid().Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
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
