using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class ApplicationProgressTests
    {
        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void AdditionalInformationStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var applicationTypeDetail = new ApplicationTypeDetail
            {
                AdditionalInformation = input,
            };

            var progress = new ApplicationTypeProgress(applicationTypeDetail);

            progress.AdditionalInformationStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void ConnectivityStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var applicationTypeDetail = new ApplicationTypeDetail
            {
                MinimumConnectionSpeed = input,
            };

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.ConnectivityStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void HardwareRequirementsStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var applicationTypeDetail = new ApplicationTypeDetail
            {
                HardwareRequirements = input,
            };

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.HardwareRequirementsStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeDesktopAdditionalInformationStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var applicationTypeDetail = new ApplicationTypeDetail
            {
                NativeDesktopAdditionalInformation = input,
            };

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeDesktopAdditionalInformationStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeDesktopConnectivityStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var applicationTypeDetail = new ApplicationTypeDetail
            {
                NativeDesktopMinimumConnectionSpeed = input,
            };

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeDesktopConnectivityStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeDesktopHardwareRequirementsStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var applicationTypeDetail = new ApplicationTypeDetail
            {
                NativeDesktopHardwareRequirements = input,
            };

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeDesktopHardwareRequirementsStatus().Should().Be(expected);
        }

        [Theory]
        [MockAutoData]
        public static void NativeDesktopMemoryAndStorageStatus_NativeDesktopMemoryAndStorageValid_ReturnsCompleted(
            ApplicationTypeDetail applicationTypeDetail)
        {
            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeDesktopMemoryAndStorageStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData("    ")]
        [MockInlineAutoData("\t")]
        public static void NativeDesktopMemoryAndStorageStatus_MinimumCpu_Invalid_ReturnsNotStarted(
            string minimumCpuValue,
            [Frozen] NativeDesktopMemoryAndStorage storage,
            ApplicationTypeDetail applicationTypeDetail)
        {
            storage.MinimumCpu = minimumCpuValue;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeDesktopMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData("    ")]
        [MockInlineAutoData("\t")]
        public static void NativeDesktopMemoryAndStorageStatus_MinimumMemoryRequirement_Invalid_ReturnsNotStarted(
            string value,
            [Frozen] NativeDesktopMemoryAndStorage storage,
            ApplicationTypeDetail applicationTypeDetail)
        {
            storage.MinimumMemoryRequirement = value;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeDesktopMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData("    ")]
        [MockInlineAutoData("\t")]
        public static void NativeDesktopMemoryAndStorageStatus_StorageRequirementsDescription_Invalid_ReturnsNotStarted(
            string value,
            [Frozen] NativeDesktopMemoryAndStorage storage,
            ApplicationTypeDetail applicationTypeDetail)
        {
            storage.StorageRequirementsDescription = value;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeDesktopMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static void NativeDesktopMemoryAndStorageStatus_NativeDesktopMemoryAndStorageIsNull_ReturnsNotStarted()
        {
            var applicationTypeDetail = new ApplicationTypeDetail
            {
                NativeDesktopMemoryAndStorage = null,
            };

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeDesktopMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void NativeDesktopThirdPartyStatus_Valid_ReturnsCompleted(
            ApplicationTypeDetail applicationTypeDetail)
        {
            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeDesktopThirdPartyStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockAutoData]
        public static void NativeDesktopThirdPartyStatus_Null_ReturnsNotStarted(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.NativeDesktopThirdParty = null;
            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeDesktopThirdPartyStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData("    ")]
        [MockInlineAutoData("\t")]
        public static void NativeDesktopThirdPartyStatus_DeviceCapabilities_Invalid_With_Valid_ThirdPartyComponents_ReturnsCompleted(
            string value,
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.NativeDesktopThirdParty.DeviceCapabilities = value;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeDesktopThirdPartyStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData("    ")]
        [MockInlineAutoData("\t")]
        public static void NativeDesktopThirdPartyStatus_ThirdPartyComponents_Invalid_With_Valid_DeviceCapabilities_ReturnsCompleted(
            string value,
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.NativeDesktopThirdParty.ThirdPartyComponents = value;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeDesktopThirdPartyStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData("    ")]
        [MockInlineAutoData("\t")]
        public static void NativeDesktopThirdPartyStatus_Both_ThirdPartyComponents_And_DeviceCapabilities_Invalid_ReturnsNotStarted(
            string value,
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.NativeDesktopThirdParty.ThirdPartyComponents = value;
            applicationTypeDetail.NativeDesktopThirdParty.DeviceCapabilities = value;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeDesktopThirdPartyStatus().Should().Be(Enums.TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void NativeMobileConnectivityStatus_Valid_ReturnsCompleted(
            ApplicationTypeDetail applicationTypeDetail)
        {
            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileConnectivityStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockAutoData]
        public static void NativeMobileConnectivityStatus_Null_ReturnsNotStarted(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.MobileConnectionDetails = null;
            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileConnectivityStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData("    ")]
        [MockInlineAutoData("\t")]
        public static void NativeMobileConnectivityStatus_DescriptionHasValidValue_ReturnsCompleted(
            string invalid,
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.MobileConnectionDetails.MinimumConnectionSpeed = invalid;
            applicationTypeDetail.MobileConnectionDetails.ConnectionType = null;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileConnectivityStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData("    ")]
        [MockInlineAutoData("\t")]
        public static void NativeMobileConnectivityStatus_MinimumConnectionSpeedHasValidValue_ReturnsCompleted(
            string invalid,
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.MobileConnectionDetails.Description = invalid;
            applicationTypeDetail.MobileConnectionDetails.ConnectionType = null;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileConnectivityStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData("    ")]
        [MockInlineAutoData("\t")]
        public static void NativeMobileConnectivityStatus_ConnectionTypeHasValidValue_ReturnsCompleted(
            string invalid,
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.MobileConnectionDetails.Description = invalid;
            applicationTypeDetail.MobileConnectionDetails.MinimumConnectionSpeed = invalid;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileConnectivityStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData("    ")]
        [MockInlineAutoData("\t")]
        public static void NativeMobileConnectivityStatus_AllPropertiesInvalid_ConnectionTypeNull_ReturnsNotStarted(
            string invalid,
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.MobileConnectionDetails.Description = invalid;
            applicationTypeDetail.MobileConnectionDetails.MinimumConnectionSpeed = invalid;
            applicationTypeDetail.MobileConnectionDetails.ConnectionType = null;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileConnectivityStatus().Should().Be(Enums.TaskProgress.NotStarted);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData("    ")]
        [MockInlineAutoData("\t")]
        public static void NativeMobileConnectivityStatus_AllPropertiesInvalid_ConnectionTypeEmpty_ReturnsNotStarted(
            string invalid,
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.MobileConnectionDetails.Description = invalid;
            applicationTypeDetail.MobileConnectionDetails.MinimumConnectionSpeed = invalid;
            applicationTypeDetail.MobileConnectionDetails.ConnectionType = new HashSet<string>();

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileConnectivityStatus().Should().Be(Enums.TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void NativeMobileMemoryAndStorageStatus_MobileMemoryAndStorageValid_ReturnsCompleted(
            ApplicationTypeDetail applicationTypeDetail)
        {
            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileMemoryAndStorageStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData("    ")]
        [MockInlineAutoData("\t")]
        public static void NativeMobileMemoryAndStorageStatus_Description_Invalid_ReturnsNotStarted(
            string value,
            [Frozen] MobileMemoryAndStorage storage,
            ApplicationTypeDetail applicationTypeDetail)
        {
            storage.Description = value;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData("    ")]
        [MockInlineAutoData("\t")]
        public static void NativeMobileMemoryAndStorageStatus_MinimumMemoryRequirement_Invalid_ReturnsNotStarted(
            string value,
            [Frozen] MobileMemoryAndStorage storage,
            ApplicationTypeDetail applicationTypeDetail)
        {
            storage.MinimumMemoryRequirement = value;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static void NativeMobileMemoryAndStorageStatus_MobileMemoryAndStorageIsNull_ReturnsNotStarted()
        {
            var applicationTypeDetail = new ApplicationTypeDetail
            {
                MobileMemoryAndStorage = null,
            };

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeDesktopSupportedOperatingSystemsStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var applicationTypeDetail = new ApplicationTypeDetail
            {
                NativeDesktopOperatingSystemsDescription = input,
            };

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeDesktopSupportedOperatingSystemsStatus().Should().Be(expected);
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsStatus_ValidMobileOperatingSystems_ReturnsCompleted(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.MobileOperatingSystems.OperatingSystems.Should().NotBeNullOrEmpty();

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileSupportedOperatingSystemsStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsStatus_OperatingSystemsEmpty_ReturnsNotStarted(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.MobileOperatingSystems.OperatingSystems = new HashSet<string>();

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileSupportedOperatingSystemsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsStatus_OperatingSystemsNull_ReturnsNotStarted(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.MobileOperatingSystems.OperatingSystems = null;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileSupportedOperatingSystemsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsStatus_MobileOperatingSystemsNull_ReturnsNotStarted(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.MobileOperatingSystems = null;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileSupportedOperatingSystemsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeMobileAdditionalInformationStatus_ValidNativeMobileAdditionalInformation_ReturnsTrue(
            string input,
            TaskProgress expected)
        {
            var applicationTypeDetail = new ApplicationTypeDetail
            {
                NativeMobileAdditionalInformation = input,
            };

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileAdditionalInformationStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeMobileHardwareRequirementsStatus_ValidNativeMobileHardwareRequirements_ReturnsTrue(
            string input,
            TaskProgress expected)
        {
            var applicationTypeDetail = new ApplicationTypeDetail
            {
                NativeMobileHardwareRequirements = input,
            };

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileHardwareRequirementsStatus().Should().Be(expected);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void PluginsStatus_PluginsRequiredHasValue_ReturnsCompleted(bool value)
        {
            var applicationTypeDetail = new ApplicationTypeDetail
            {
                Plugins = new Plugins
                {
                    Required = value,
                },
            };

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.PluginsStatus().Should().Be(TaskProgress.Completed);
        }

        [Fact]
        public static void PluginsStatus_PluginsRequiredHasNoValue_ReturnsNotStarted()
        {
            var applicationTypeDetail = new ApplicationTypeDetail
            {
                Plugins = new Plugins
                {
                    Required = null,
                },
            };

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.PluginsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static void PluginsStatus_PluginsIsNull_ReturnsNotStarted()
        {
            var applicationTypeDetail = new ApplicationTypeDetail
            {
                Plugins = null,
            };

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.PluginsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersStatus_ValidValues_ReturnsCompleted(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.BrowsersSupported.Should().NotBeNullOrEmpty();
            applicationTypeDetail.MobileResponsive.HasValue.Should().BeTrue();

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.SupportedBrowsersStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersStatus_BrowsersSupportedIsEmpty_ReturnsNotStarted(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.BrowsersSupported = new HashSet<SupportedBrowser>();
            applicationTypeDetail.MobileResponsive.HasValue.Should().BeTrue();

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.SupportedBrowsersStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersStatus_BrowsersSupportedIsNull_ReturnsNotStarted(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.BrowsersSupported = null;
            applicationTypeDetail.MobileResponsive.HasValue.Should().BeTrue();

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.SupportedBrowsersStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersStatus_MobileResponsiveIsNull_ReturnsNotStarted(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.BrowsersSupported.Should().NotBeNullOrEmpty();
            applicationTypeDetail.MobileResponsive = null;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.SupportedBrowsersStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void NativeMobileThirdPartyStatus_MobileThirdPartyValid_ReturnsCompleted(
            ApplicationTypeDetail applicationTypeDetail)
        {
            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileThirdPartyStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockAutoData]
        public static void TNativeMobileThirdPartyStatus_MobileThirdPartyInvalid_ReturnsNotStarted(
            [Frozen] MobileThirdParty thirdParty,
            ApplicationTypeDetail applicationTypeDetail)
        {
            thirdParty.DeviceCapabilities = null;
            thirdParty.ThirdPartyComponents = null;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileThirdPartyStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static void NativeMobileThirdPartyStatus_MobileThirdPartyNull_ReturnsNotStarted()
        {
            var applicationTypeDetail = new ApplicationTypeDetail
            {
                MobileThirdParty = null,
            };

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.NativeMobileThirdPartyStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockInlineAutoData(ApplicationType.BrowserBased)]
        [MockInlineAutoData(ApplicationType.MobileTablet)]
        [MockInlineAutoData(ApplicationType.Desktop)]
        public static void ApplicationTypeStatus_ReturnsComplete(
            ApplicationType applicationType,
            ApplicationTypeDetail applicationTypeDetail)
        {
            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.ApplicationTypeStatus(applicationType).Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockInlineAutoData]
        public static void ApplicationTypeStatus_BrowserBased_NoPlugins_ReturnsInProgress(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.Plugins = null;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.ApplicationTypeStatus(ApplicationType.BrowserBased).Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockInlineAutoData]
        public static void ApplicationTypeStatus_BrowserBased_NoSupportedBrowsers_ReturnsInProgress(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.BrowsersSupported = null;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.ApplicationTypeStatus(ApplicationType.BrowserBased).Should().Be(TaskProgress.InProgress);
        }

        [Fact]
        public static void ApplicationTypeStatus_BrowserBased_NothingComplete_ReturnsNotStarted()
        {
            var applicationTypeDetail = new ApplicationTypeDetail();

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.ApplicationTypeStatus(ApplicationType.BrowserBased).Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockInlineAutoData]
        public static void ApplicationTypeStatus_DesktopBased_NoOperatingSystemDescription_ReturnsInProgress(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.NativeDesktopOperatingSystemsDescription = null;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.ApplicationTypeStatus(ApplicationType.Desktop).Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockInlineAutoData]
        public static void ApplicationTypeStatus_DesktopBased_NoNativeDesktopMinimumConnectionSpeed_ReturnsInProgress(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.NativeDesktopMinimumConnectionSpeed = null;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.ApplicationTypeStatus(ApplicationType.Desktop).Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockInlineAutoData]
        public static void ApplicationTypeStatus_DesktopBased_NoNativeDesktopMemoryAndStorage_ReturnsInProgress(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.NativeDesktopMemoryAndStorage = null;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.ApplicationTypeStatus(ApplicationType.Desktop).Should().Be(TaskProgress.InProgress);
        }

        [Fact]
        public static void ApplicationTypeStatus_DesktopBased_NothingComplete_ReturnsNotStarted()
        {
            var applicationTypeDetail = new ApplicationTypeDetail();

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.ApplicationTypeStatus(ApplicationType.BrowserBased).Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockInlineAutoData]
        public static void ApplicationTypeStatus_MobileTablet_NoMobileOperatingSystems_ReturnsInProgress(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.MobileOperatingSystems = null;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.ApplicationTypeStatus(ApplicationType.MobileTablet).Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockInlineAutoData]
        public static void ApplicationTypeStatus_MobileTablet_NoMobileMemoryAndStorage_ReturnsInProgress(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.MobileMemoryAndStorage = null;

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.ApplicationTypeStatus(ApplicationType.MobileTablet).Should().Be(TaskProgress.InProgress);
        }

        [Fact]
        public static void ApplicationTypeStatus_MobileTablet_NothingComplete_ReturnsNotStarted()
        {
            var applicationTypeDetail = new ApplicationTypeDetail();

            var progress = new ApplicationTypeProgress(applicationTypeDetail);
            progress.ApplicationTypeStatus(ApplicationType.BrowserBased).Should().Be(TaskProgress.NotStarted);
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
