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
        public static void Status_BothPropertiesValid_ReturnsCompleted(NativeDesktopThirdParty nativeDesktopThirdParty)
        {
            nativeDesktopThirdParty.DeviceCapabilities.Should().NotBeNullOrWhiteSpace();
            nativeDesktopThirdParty.ThirdPartyComponents.Should().NotBeNullOrWhiteSpace();

            nativeDesktopThirdParty.Status().Should().Be(Enums.TaskProgress.Completed);
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void Status_DeviceCapabilitiesIsInvalid_ReturnsCompleted(string invalid)
        {
            var nativeDesktopThirdParty =
                Fixture.Build<NativeDesktopThirdParty>().Without(m => m.DeviceCapabilities).Create();
            nativeDesktopThirdParty.DeviceCapabilities = invalid;

            nativeDesktopThirdParty.Status().Should().Be(Enums.TaskProgress.Completed);
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void Status_ThirdPartyComponentsIsInvalid_ReturnsCompleted(string invalid)
        {
            var nativeDesktopThirdParty =
                Fixture.Build<NativeDesktopThirdParty>().Without(m => m.ThirdPartyComponents).Create();
            nativeDesktopThirdParty.ThirdPartyComponents = invalid;

            nativeDesktopThirdParty.Status().Should().Be(Enums.TaskProgress.Completed);
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void Status_BothPropertiesInvalid_ReturnsNotStarted(string invalid)
        {
            var nativeDesktopThirdParty = new NativeDesktopThirdParty
            {
                DeviceCapabilities = invalid,
                ThirdPartyComponents = invalid,
            };

            nativeDesktopThirdParty.Status().Should().Be(Enums.TaskProgress.NotStarted);
        }
    }
}
