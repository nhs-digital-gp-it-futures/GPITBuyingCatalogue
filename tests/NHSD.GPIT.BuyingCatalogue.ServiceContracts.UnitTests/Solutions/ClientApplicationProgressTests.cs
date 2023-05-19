using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class ClientApplicationProgressTests
    {
        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void AdditionalInformationStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var clientApplication = new ClientApplication
            {
                AdditionalInformation = input,
            };

            var progress = new ClientApplicationProgress(clientApplication);

            progress.AdditionalInformationStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void ConnectivityStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var clientApplication = new ClientApplication
            {
                MinimumConnectionSpeed = input,
            };

            var progress = new ClientApplicationProgress(clientApplication);
            progress.ConnectivityStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void HardwareRequirementsStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var clientApplication = new ClientApplication
            {
                HardwareRequirements = input,
            };

            var progress = new ClientApplicationProgress(clientApplication);
            progress.HardwareRequirementsStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeDesktopAdditionalInformationStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var clientApplication = new ClientApplication
            {
                NativeDesktopAdditionalInformation = input,
            };

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeDesktopAdditionalInformationStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeDesktopConnectivityStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var clientApplication = new ClientApplication
            {
                NativeDesktopMinimumConnectionSpeed = input,
            };

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeDesktopConnectivityStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeDesktopHardwareRequirementsStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var clientApplication = new ClientApplication
            {
                NativeDesktopHardwareRequirements = input,
            };

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeDesktopHardwareRequirementsStatus().Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void NativeDesktopMemoryAndStorageStatus_NativeDesktopMemoryAndStorageValid_ReturnsCompleted(
            ClientApplication clientApplication)
        {
            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeDesktopMemoryAndStorageStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("    ")]
        [CommonInlineAutoData("\t")]
        public static void NativeDesktopMemoryAndStorageStatus_MinimumCpu_Invalid_ReturnsNotStarted(
            string minimumCpuValue,
            [Frozen] NativeDesktopMemoryAndStorage storage,
            ClientApplication clientApplication)
        {
            storage.MinimumCpu = minimumCpuValue;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeDesktopMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("    ")]
        [CommonInlineAutoData("\t")]
        public static void NativeDesktopMemoryAndStorageStatus_MinimumMemoryRequirement_Invalid_ReturnsNotStarted(
            string value,
            [Frozen] NativeDesktopMemoryAndStorage storage,
            ClientApplication clientApplication)
        {
            storage.MinimumMemoryRequirement = value;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeDesktopMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("    ")]
        [CommonInlineAutoData("\t")]
        public static void NativeDesktopMemoryAndStorageStatus_StorageRequirementsDescription_Invalid_ReturnsNotStarted(
            string value,
            [Frozen] NativeDesktopMemoryAndStorage storage,
            ClientApplication clientApplication)
        {
            storage.StorageRequirementsDescription = value;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeDesktopMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static void NativeDesktopMemoryAndStorageStatus_NativeDesktopMemoryAndStorageIsNull_ReturnsNotStarted()
        {
            var clientApplication = new ClientApplication
            {
                NativeDesktopMemoryAndStorage = null,
            };

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeDesktopMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void NativeDesktopThirdPartyStatus_Valid_ReturnsCompleted(
            ClientApplication clientApplication)
        {
            // TODO: MJK
            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeDesktopThirdPartyStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void NativeDesktopThirdPartyStatus_Null_ReturnsNotStarted(
            ClientApplication clientApplication)
        {
            // TODO: MJK
            clientApplication.NativeDesktopThirdParty = null;
            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeDesktopThirdPartyStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("    ")]
        [CommonInlineAutoData("\t")]
        public static void NativeDesktopThirdPartyStatus_DeviceCapabilities_Invalid_With_Valid_ThirdPartyComponents_ReturnsCompleted(
            string value,
            ClientApplication clientApplication)
        {
            clientApplication.NativeDesktopThirdParty.DeviceCapabilities = value;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeDesktopThirdPartyStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("    ")]
        [CommonInlineAutoData("\t")]
        public static void NativeDesktopThirdPartyStatus_ThirdPartyComponents_Invalid_With_Valid_DeviceCapabilities_ReturnsCompleted(
            string value,
            ClientApplication clientApplication)
        {
            clientApplication.NativeDesktopThirdParty.ThirdPartyComponents = value;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeDesktopThirdPartyStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("    ")]
        [CommonInlineAutoData("\t")]
        public static void NativeDesktopThirdPartyStatus_Both_ThirdPartyComponents_And_DeviceCapabilities_Invalid_ReturnsNotStarted(
            string value,
            ClientApplication clientApplication)
        {
            clientApplication.NativeDesktopThirdParty.ThirdPartyComponents = value;
            clientApplication.NativeDesktopThirdParty.DeviceCapabilities = value;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeDesktopThirdPartyStatus().Should().Be(Enums.TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void NativeMobileConnectivityStatus_Valid_ReturnsCompleted(
            ClientApplication clientApplication)
        {
            // TODO: MJK
            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileConnectivityStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void NativeMobileConnectivityStatus_Null_ReturnsNotStarted(
            ClientApplication clientApplication)
        {
            // TODO: MJK
            clientApplication.MobileConnectionDetails = null;
            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileConnectivityStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("    ")]
        [CommonInlineAutoData("\t")]
        public static void NativeMobileConnectivityStatus_DescriptionHasValidValue_ReturnsCompleted(
            string invalid,
            ClientApplication clientApplication)
        {
            clientApplication.MobileConnectionDetails.MinimumConnectionSpeed = invalid;
            clientApplication.MobileConnectionDetails.ConnectionType = null;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileConnectivityStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("    ")]
        [CommonInlineAutoData("\t")]
        public static void NativeMobileConnectivityStatus_MinimumConnectionSpeedHasValidValue_ReturnsCompleted(
            string invalid,
            ClientApplication clientApplication)
        {
            clientApplication.MobileConnectionDetails.Description = invalid;
            clientApplication.MobileConnectionDetails.ConnectionType = null;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileConnectivityStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("    ")]
        [CommonInlineAutoData("\t")]
        public static void NativeMobileConnectivityStatus_ConnectionTypeHasValidValue_ReturnsCompleted(
            string invalid,
            ClientApplication clientApplication)
        {
            clientApplication.MobileConnectionDetails.Description = invalid;
            clientApplication.MobileConnectionDetails.MinimumConnectionSpeed = invalid;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileConnectivityStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("    ")]
        [CommonInlineAutoData("\t")]
        public static void NativeMobileConnectivityStatus_AllPropertiesInvalid_ConnectionTypeNull_ReturnsNotStarted(
            string invalid,
            ClientApplication clientApplication)
        {
            clientApplication.MobileConnectionDetails.Description = invalid;
            clientApplication.MobileConnectionDetails.MinimumConnectionSpeed = invalid;
            clientApplication.MobileConnectionDetails.ConnectionType = null;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileConnectivityStatus().Should().Be(Enums.TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("    ")]
        [CommonInlineAutoData("\t")]
        public static void NativeMobileConnectivityStatus_AllPropertiesInvalid_ConnectionTypeEmpty_ReturnsNotStarted(
            string invalid,
            ClientApplication clientApplication)
        {
            clientApplication.MobileConnectionDetails.Description = invalid;
            clientApplication.MobileConnectionDetails.MinimumConnectionSpeed = invalid;
            clientApplication.MobileConnectionDetails.ConnectionType = new HashSet<string>();

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileConnectivityStatus().Should().Be(Enums.TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void NativeMobileMemoryAndStorageStatus_MobileMemoryAndStorageValid_ReturnsCompleted(
            ClientApplication clientApplication)
        {
            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileMemoryAndStorageStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("    ")]
        [CommonInlineAutoData("\t")]
        public static void NativeMobileMemoryAndStorageStatus_Description_Invalid_ReturnsNotStarted(
            string value,
            [Frozen] MobileMemoryAndStorage storage,
            ClientApplication clientApplication)
        {
            storage.Description = value;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData("    ")]
        [CommonInlineAutoData("\t")]
        public static void NativeMobileMemoryAndStorageStatus_MinimumMemoryRequirement_Invalid_ReturnsNotStarted(
            string value,
            [Frozen] MobileMemoryAndStorage storage,
            ClientApplication clientApplication)
        {
            storage.MinimumMemoryRequirement = value;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static void NativeMobileMemoryAndStorageStatus_MobileMemoryAndStorageIsNull_ReturnsNotStarted()
        {
            var clientApplication = new ClientApplication
            {
                MobileMemoryAndStorage = null,
            };

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeDesktopSupportedOperatingSystemsStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var clientApplication = new ClientApplication
            {
                NativeDesktopOperatingSystemsDescription = input,
            };

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeDesktopSupportedOperatingSystemsStatus().Should().Be(expected);
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsStatus_ValidMobileOperatingSystems_ReturnsCompleted(
            ClientApplication clientApplication)
        {
            clientApplication.MobileOperatingSystems.OperatingSystems.Should().NotBeNullOrEmpty();

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileSupportedOperatingSystemsStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsStatus_OperatingSystemsEmpty_ReturnsNotStarted(
            ClientApplication clientApplication)
        {
            clientApplication.MobileOperatingSystems.OperatingSystems = new HashSet<string>();

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileSupportedOperatingSystemsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsStatus_OperatingSystemsNull_ReturnsNotStarted(
            ClientApplication clientApplication)
        {
            clientApplication.MobileOperatingSystems.OperatingSystems = null;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileSupportedOperatingSystemsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsStatus_MobileOperatingSystemsNull_ReturnsNotStarted(
            ClientApplication clientApplication)
        {
            clientApplication.MobileOperatingSystems = null;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileSupportedOperatingSystemsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeMobileAdditionalInformationStatus_ValidNativeMobileAdditionalInformation_ReturnsTrue(
            string input,
            TaskProgress expected)
        {
            var clientApplication = new ClientApplication
            {
                NativeMobileAdditionalInformation = input,
            };

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileAdditionalInformationStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeMobileHardwareRequirementsStatus_ValidNativeMobileHardwareRequirements_ReturnsTrue(
            string input,
            TaskProgress expected)
        {
            var clientApplication = new ClientApplication
            {
                NativeMobileHardwareRequirements = input,
            };

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileHardwareRequirementsStatus().Should().Be(expected);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void PluginsStatus_PluginsRequiredHasValue_ReturnsCompleted(bool value)
        {
            var clientApplication = new ClientApplication
            {
                Plugins = new Plugins
                {
                    Required = value,
                },
            };

            var progress = new ClientApplicationProgress(clientApplication);
            progress.PluginsStatus().Should().Be(TaskProgress.Completed);
        }

        [Fact]
        public static void PluginsStatus_PluginsRequiredHasNoValue_ReturnsNotStarted()
        {
            var clientApplication = new ClientApplication
            {
                Plugins = new Plugins
                {
                    Required = null,
                },
            };

            var progress = new ClientApplicationProgress(clientApplication);
            progress.PluginsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static void PluginsStatus_PluginsIsNull_ReturnsNotStarted()
        {
            var clientApplication = new ClientApplication
            {
                Plugins = null,
            };

            var progress = new ClientApplicationProgress(clientApplication);
            progress.PluginsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersStatus_ValidValues_ReturnsCompleted(
            ClientApplication clientApplication)
        {
            clientApplication.BrowsersSupported.Should().NotBeNullOrEmpty();
            clientApplication.MobileResponsive.HasValue.Should().BeTrue();

            var progress = new ClientApplicationProgress(clientApplication);
            progress.SupportedBrowsersStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersStatus_BrowsersSupportedIsEmpty_ReturnsNotStarted(
            ClientApplication clientApplication)
        {
            clientApplication.BrowsersSupported = new HashSet<SupportedBrowser>();
            clientApplication.MobileResponsive.HasValue.Should().BeTrue();

            var progress = new ClientApplicationProgress(clientApplication);
            progress.SupportedBrowsersStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersStatus_BrowsersSupportedIsNull_ReturnsNotStarted(
            ClientApplication clientApplication)
        {
            clientApplication.BrowsersSupported = null;
            clientApplication.MobileResponsive.HasValue.Should().BeTrue();

            var progress = new ClientApplicationProgress(clientApplication);
            progress.SupportedBrowsersStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersStatus_MobileResponsiveIsNull_ReturnsNotStarted(
            ClientApplication clientApplication)
        {
            clientApplication.BrowsersSupported.Should().NotBeNullOrEmpty();
            clientApplication.MobileResponsive = null;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.SupportedBrowsersStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void NativeMobileThirdPartyStatus_MobileThirdPartyValid_ReturnsCompleted(
            ClientApplication clientApplication)
        {
            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileThirdPartyStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void TNativeMobileThirdPartyStatus_MobileThirdPartyInvalid_ReturnsNotStarted(
            [Frozen] MobileThirdParty thirdParty,
            ClientApplication clientApplication)
        {
            thirdParty.DeviceCapabilities = null;
            thirdParty.ThirdPartyComponents = null;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileThirdPartyStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static void NativeMobileThirdPartyStatus_MobileThirdPartyNull_ReturnsNotStarted()
        {
            var clientApplication = new ClientApplication
            {
                MobileThirdParty = null,
            };

            var progress = new ClientApplicationProgress(clientApplication);
            progress.NativeMobileThirdPartyStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData(ClientApplicationType.BrowserBased)]
        [CommonInlineAutoData(ClientApplicationType.MobileTablet)]
        [CommonInlineAutoData(ClientApplicationType.Desktop)]
        public static void ApplicationTypeStatus_ReturnsComplete(
            ClientApplicationType clientApplicationType,
            ClientApplication clientApplication)
        {
            var progress = new ClientApplicationProgress(clientApplication);
            progress.ApplicationTypeStatus(clientApplicationType).Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_BrowserBased_NoPlugins_ReturnsInProgress(
            ClientApplication clientApplication)
        {
            clientApplication.Plugins = null;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.ApplicationTypeStatus(ClientApplicationType.BrowserBased).Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_BrowserBased_NoSupportedBrowsers_ReturnsInProgress(
            ClientApplication clientApplication)
        {
            clientApplication.BrowsersSupported = null;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.ApplicationTypeStatus(ClientApplicationType.BrowserBased).Should().Be(TaskProgress.InProgress);
        }

        [Fact]
        public static void ApplicationTypeStatus_BrowserBased_NothingComplete_ReturnsNotStarted()
        {
            var clientApplication = new ClientApplication();

            var progress = new ClientApplicationProgress(clientApplication);
            progress.ApplicationTypeStatus(ClientApplicationType.BrowserBased).Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_DesktopBased_NoOperatingSystemDescription_ReturnsInProgress(
            ClientApplication clientApplication)
        {
            clientApplication.NativeDesktopOperatingSystemsDescription = null;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.ApplicationTypeStatus(ClientApplicationType.Desktop).Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_DesktopBased_NoNativeDesktopMinimumConnectionSpeed_ReturnsInProgress(
            ClientApplication clientApplication)
        {
            clientApplication.NativeDesktopMinimumConnectionSpeed = null;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.ApplicationTypeStatus(ClientApplicationType.Desktop).Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_DesktopBased_NoNativeDesktopMemoryAndStorage_ReturnsInProgress(
            ClientApplication clientApplication)
        {
            clientApplication.NativeDesktopMemoryAndStorage = null;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.ApplicationTypeStatus(ClientApplicationType.Desktop).Should().Be(TaskProgress.InProgress);
        }

        [Fact]
        public static void ApplicationTypeStatus_DesktopBased_NothingComplete_ReturnsNotStarted()
        {
            var clientApplication = new ClientApplication();

            var progress = new ClientApplicationProgress(clientApplication);
            progress.ApplicationTypeStatus(ClientApplicationType.BrowserBased).Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_MobileTablet_NoMobileOperatingSystems_ReturnsInProgress(
            ClientApplication clientApplication)
        {
            clientApplication.MobileOperatingSystems = null;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.ApplicationTypeStatus(ClientApplicationType.MobileTablet).Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_MobileTablet_NoMobileMemoryAndStorage_ReturnsInProgress(
            ClientApplication clientApplication)
        {
            clientApplication.MobileMemoryAndStorage = null;

            var progress = new ClientApplicationProgress(clientApplication);
            progress.ApplicationTypeStatus(ClientApplicationType.MobileTablet).Should().Be(TaskProgress.InProgress);
        }

        [Fact]
        public static void ApplicationTypeStatus_MobileTablet_NothingComplete_ReturnsNotStarted()
        {
            var clientApplication = new ClientApplication();

            var progress = new ClientApplicationProgress(clientApplication);
            progress.ApplicationTypeStatus(ClientApplicationType.BrowserBased).Should().Be(TaskProgress.NotStarted);
        }

        private static class ResultSetData
        {
            public static IEnumerable<object[]> TestData()
            {
                yield return new object[] { "some-value", TaskProgress.Completed };
                yield return new object[] { null, TaskProgress.NotStarted };
                yield return new object[] { string.Empty, TaskProgress.NotStarted };
                yield return new object[] { "     ", TaskProgress.NotStarted };
            }
        }
    }
}
