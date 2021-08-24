using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.ClientApplicationType
{
    public static class NativeMobileModelTests
    {
        private const string Incomplete = "Incomplete";
        private const string Complete = "Complete";

        [Theory]
        [CommonInlineAutoData(null, Incomplete)]
        [CommonInlineAutoData("", Incomplete)]
        [CommonInlineAutoData("\t", Incomplete)]
        [CommonInlineAutoData("AdditionalInfo", Complete)]
        public static void AdditionalInformationStatus_Various_NativeMobileAdditionalInformationComplete_ResultAsExpected(
            string nativeMobileAdditionalInformation,
            string expectedStatus,
            [Frozen] ClientApplication clientApplication,
            NativeMobileModel model)
        {
            clientApplication.NativeMobileAdditionalInformation = nativeMobileAdditionalInformation;

            model.AdditionalInformationStatus.Should().Be(expectedStatus);
        }

        [Fact]
        public static void ClientApplication_Null_ValuesFalseOrIncomplete()
        {
            var nativeMobileModel = new NativeMobileModel();
            nativeMobileModel.ClientApplication.Should().BeNull();

            nativeMobileModel.IsComplete.Should().BeFalse();
            nativeMobileModel.ConnectivityStatus.Should().Be(Incomplete);
            nativeMobileModel.HardwareRequirementsStatus.Should().Be(Incomplete);
            nativeMobileModel.MemoryAndStorageStatus.Should().Be(Incomplete);
            nativeMobileModel.SupportedOperatingSystemsStatus.Should().Be(Incomplete);
            nativeMobileModel.ThirdPartyStatus.Should().Be(Incomplete);
        }

        [Theory]
        [CommonInlineAutoData(null, Incomplete)]
        [CommonInlineAutoData("", Incomplete)]
        [CommonInlineAutoData("\t", Incomplete)]
        [CommonInlineAutoData("ConnectionDetails", Complete)]
        public static void ConnectivityStatus_Various_NativeMobileConnectivityComplete_ResultAsExpected(
            string connectionDetailsInfo,
            string expectedStatus,
            [Frozen] MobileConnectionDetails connectionDetails,
            NativeMobileModel model)
        {
            connectionDetails.ConnectionType.Clear();
            connectionDetails.Description = connectionDetailsInfo;
            connectionDetails.MinimumConnectionSpeed = connectionDetailsInfo;

            model.ConnectivityStatus.Should().Be(expectedStatus);
        }

        [Theory]
        [CommonInlineAutoData(null, Incomplete)]
        [CommonInlineAutoData("", Incomplete)]
        [CommonInlineAutoData("\t", Incomplete)]
        [CommonInlineAutoData("HardwareRequirements", Complete)]
        public static void HardwareRequirementsStatus_Various_NativeMobileHardwareRequirementsComplete_ResultAsExpected(
            string nativeMobileHardwareRequirements,
            string expectedStatus,
            [Frozen] ClientApplication clientApplication,
            NativeMobileModel model)
        {
            clientApplication.NativeMobileHardwareRequirements = nativeMobileHardwareRequirements;

            model.HardwareRequirementsStatus.Should().Be(expectedStatus);
        }

        [Theory]
        [CommonAutoData]
        public static void IsComplete_AllValuesValid_ReturnsTrue(NativeMobileModel model)
        {
            model.IsComplete.Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void IsComplete_NativeMobileSupportedOperatingSystemsComplete_NotTrue_ReturnsFalse(
            [Frozen] ClientApplication clientApplication,
            NativeMobileModel model)
        {
            clientApplication.MobileOperatingSystems.OperatingSystems.Clear();

            model.IsComplete.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void IsComplete_NativeMobileFirstApproachComplete_NotTrue_ReturnsFalse(
            [Frozen] ClientApplication clientApplication,
            NativeMobileModel model)
        {
            clientApplication.NativeMobileFirstDesign = null;

            model.IsComplete.Should().BeFalse();
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("\t")]
        public static void IsComplete_NativeMobileMemoryComplete_NotTrue_ReturnsFalse(
            string description,
            [Frozen] MobileMemoryAndStorage storage,
            NativeMobileModel model)
        {
            storage.Description = description;

            model.IsComplete.Should().BeFalse();
        }

        [Theory]
        [CommonInlineAutoData(null, Incomplete)]
        [CommonInlineAutoData("", Incomplete)]
        [CommonInlineAutoData("\t", Incomplete)]
        [CommonInlineAutoData("StorageDescription", Complete)]
        public static void MemoryAndStorageStatus_Various_NativeMobileMemoryAndStorageComplete_ResultAsExpected(
            string description,
            string expectedStatus,
            [Frozen] MobileMemoryAndStorage storage,
            NativeMobileModel model)
        {
            storage.Description = description;

            model.MemoryAndStorageStatus.Should().Be(expectedStatus);
        }

        [Theory]
        [CommonAutoData]
        public static void SupportedOperatingSystemsStatus_HasMobileOperatingSystem_NativeMobileSupportedOperatingSystemsComplete_ResultAsExpected(
            NativeMobileModel model)
        {
            model.SupportedOperatingSystemsStatus.Should().Be(Complete);
        }

        [Theory]
        [CommonAutoData]
        public static void SupportedOperatingSystemsStatus_DoesNotHaveMobileOperatingSystem_NativeMobileSupportedOperatingSystemsComplete_ResultAsExpected(
            [Frozen] ClientApplication clientApplication,
            NativeMobileModel model)
        {
            clientApplication.MobileOperatingSystems.OperatingSystems.Clear();

            model.SupportedOperatingSystemsStatus.Should().Be(Incomplete);
        }

        [Theory]
        [CommonInlineAutoData(null, Incomplete)]
        [CommonInlineAutoData("", Incomplete)]
        [CommonInlineAutoData("\t", Incomplete)]
        [CommonInlineAutoData("ThirdParty", Complete)]
        public static void ThirdPartyStatus_Various_NativeMobileThirdPartyComplete_ResultAsExpected(
            string thirdPartyDetails,
            string expectedStatus,
            [Frozen] MobileThirdParty thirdParty,
            NativeMobileModel model)
        {
            thirdParty.DeviceCapabilities = thirdPartyDetails;
            thirdParty.ThirdPartyComponents = thirdPartyDetails;

            model.ThirdPartyStatus.Should().Be(expectedStatus);
        }
    }
}
