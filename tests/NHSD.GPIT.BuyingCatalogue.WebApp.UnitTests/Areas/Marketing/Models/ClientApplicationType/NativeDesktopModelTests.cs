using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.ClientApplicationType
{
    public static class NativeDesktopModelTests
    {
        private const string Incomplete = "Incomplete";
        private const string Complete = "Complete";

        [Theory]
        [CommonInlineAutoData(null, Incomplete)]
        [CommonInlineAutoData("", Incomplete)]
        [CommonInlineAutoData("\t", Incomplete)]
        [CommonInlineAutoData("AdditionalInfo", Complete)]
        public static void AdditionalInformationStatus_Various_NativeDesktopAdditionalInformationComplete_ResultAsExpected(
            string nativeDesktopInfo,
            string expectedStatus,
            [Frozen] ClientApplication clientApplication,
            NativeDesktopModel model)
        {
            clientApplication.NativeDesktopAdditionalInformation = nativeDesktopInfo;

            model.AdditionalInformationStatus.Should().Be(expectedStatus);
        }

        [Fact]
        public static void ClientApplication_Null_ValuesFalseOrIncomplete()
        {
            var nativeDesktopModel = new NativeDesktopModel();
            nativeDesktopModel.ClientApplication.Should().BeNull();

            nativeDesktopModel.IsComplete.Should().BeFalse();
            nativeDesktopModel.ConnectivityStatus.Should().Be(Incomplete);
            nativeDesktopModel.HardwareRequirementsStatus.Should().Be(Incomplete);
            nativeDesktopModel.MemoryAndStorageStatus.Should().Be(Incomplete);
            nativeDesktopModel.SupportedOperatingSystemsStatus.Should().Be(Incomplete);
            nativeDesktopModel.ThirdPartyStatus.Should().Be(Incomplete);
        }

        [Theory]
        [CommonInlineAutoData(null, Incomplete)]
        [CommonInlineAutoData("", Incomplete)]
        [CommonInlineAutoData("\t", Incomplete)]
        [CommonInlineAutoData("ConnectionSpeed", Complete)]
        public static void ConnectivityStatus_Various_NativeDesktopConnectivityComplete_ResultAsExpected(
            string nativeDesktopMinConnectionSpeed,
            string expectedStatus,
            [Frozen] ClientApplication clientApplication,
            NativeDesktopModel model)
        {
            clientApplication.NativeDesktopMinimumConnectionSpeed = nativeDesktopMinConnectionSpeed;

            model.ConnectivityStatus.Should().Be(expectedStatus);
        }

        [Theory]
        [CommonInlineAutoData(null, Incomplete)]
        [CommonInlineAutoData("", Incomplete)]
        [CommonInlineAutoData("\t", Incomplete)]
        [CommonInlineAutoData("HardwareRequirements", Complete)]
        public static void HardwareRequirementsStatus_Various_NativeDesktopHardwareRequirementsComplete_ResultAsExpected(
            string nativeDesktopHardwareRequirements,
            string expectedStatus,
            [Frozen] ClientApplication clientApplication,
            NativeDesktopModel model)
        {
            clientApplication.NativeDesktopHardwareRequirements = nativeDesktopHardwareRequirements;

            model.HardwareRequirementsStatus.Should().Be(expectedStatus);
        }

        [Theory]
        [CommonAutoData]
        public static void IsComplete_AllValuesValid_ReturnsTrue(NativeDesktopModel model)
        {
            model.IsComplete.Should().BeTrue();
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("\t")]
        public static void IsComplete_NativeDesktopSupportedOperatingSystemsComplete_NotTrue_ReturnsFalse(
            string nativeDesktopOperatingSystemsDescription,
            [Frozen] ClientApplication clientApplication,
            NativeDesktopModel model)
        {
            clientApplication.NativeDesktopOperatingSystemsDescription = nativeDesktopOperatingSystemsDescription;

            model.IsComplete.Should().BeFalse();
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("\t")]
        public static void IsComplete_NativeDesktopConnectivityComplete_NotTrue_ReturnsFalse(
            string nativeDesktopMinConnectionSpeed,
            [Frozen] ClientApplication clientApplication,
            NativeDesktopModel model)
        {
            clientApplication.NativeDesktopMinimumConnectionSpeed = nativeDesktopMinConnectionSpeed;

            model.IsComplete.Should().BeFalse();
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("\t")]
        public static void IsComplete_NativeDesktopMemoryComplete_NotTrue_ReturnsFalse(
            string minimumMemoryRequirement,
            [Frozen] NativeDesktopMemoryAndStorage storage,
            NativeDesktopModel model)
        {
            storage.MinimumMemoryRequirement = minimumMemoryRequirement;

            model.IsComplete.Should().BeFalse();
        }

        [Theory]
        [CommonInlineAutoData(null, Incomplete)]
        [CommonInlineAutoData("", Incomplete)]
        [CommonInlineAutoData("\t", Incomplete)]
        [CommonInlineAutoData("MinCPU", Complete)]
        public static void MemoryAndStorageStatus_Various_NativeDesktopMemoryAndStorageComplete_ResultAsExpected(
            string minimumCpu,
            string expectedStatus,
            [Frozen] NativeDesktopMemoryAndStorage storage,
            NativeDesktopModel model)
        {
            storage.MinimumCpu = minimumCpu;

            model.MemoryAndStorageStatus.Should().Be(expectedStatus);
        }

        [Theory]
        [CommonInlineAutoData(null, Incomplete)]
        [CommonInlineAutoData("", Incomplete)]
        [CommonInlineAutoData("\t", Incomplete)]
        [CommonInlineAutoData("Description", Complete)]
        public static void SupportedOperatingSystemsStatus_Various_NativeDesktopSupportedOperatingSystemsComplete_ResultAsExpected(
            string osDescription,
            string expected,
            [Frozen] ClientApplication clientApplication,
            NativeDesktopModel model)
        {
            clientApplication.NativeDesktopOperatingSystemsDescription = osDescription;

            model.SupportedOperatingSystemsStatus.Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(null, Incomplete)]
        [CommonInlineAutoData("", Incomplete)]
        [CommonInlineAutoData("\t", Incomplete)]
        [CommonInlineAutoData("Description", Complete)]
        public static void ThirdPartyStatus_Various_NativeDesktopThirdPartyComplete_ResultAsExpected(
            string thirdPartyCapabilitiesAndComponents,
            string expected,
            [Frozen] NativeDesktopThirdParty thirdParty,
            NativeDesktopModel model)
        {
            thirdParty.DeviceCapabilities = thirdPartyCapabilitiesAndComponents;
            thirdParty.ThirdPartyComponents = thirdPartyCapabilitiesAndComponents;

            model.ThirdPartyStatus.Should().Be(expected);
        }
    }
}
