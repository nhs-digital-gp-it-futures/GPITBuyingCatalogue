using System.Collections.Generic;
using AutoFixture.Xunit2;
using EnumsNET;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class ApplicationTypeTests
    {
        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void AdditionalInformationStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var applicationTypes = new ApplicationTypes
            {
                AdditionalInformation = input,
            };

            applicationTypes.AdditionalInformationStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void ConnectivityStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var applicationTypes = new ApplicationTypes
            {
                MinimumConnectionSpeed = input,
            };

            applicationTypes.ConnectivityStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void HardwareRequirementsStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var applicationTypes = new ApplicationTypes
            {
                HardwareRequirements = input,
            };

            applicationTypes.HardwareRequirementsStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeDesktopAdditionalInformationStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var applicationTypes = new ApplicationTypes
            {
                NativeDesktopAdditionalInformation = input,
            };

            applicationTypes.NativeDesktopAdditionalInformationStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeDesktopConnectivityStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var applicationTypes = new ApplicationTypes
            {
                NativeDesktopMinimumConnectionSpeed = input,
            };

            applicationTypes.NativeDesktopConnectivityStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeDesktopHardwareRequirementsStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var applicationTypes = new ApplicationTypes
            {
                NativeDesktopHardwareRequirements = input,
            };

            applicationTypes.NativeDesktopHardwareRequirementsStatus().Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void NativeDesktopMemoryAndStorageStatus_NativeDesktopMemoryAndStorageValid_ReturnsCompleted(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.NativeDesktopMemoryAndStorageStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void NativeDesktopMemoryAndStorageStatus_NativeDesktopMemoryAndStorageInvalid_ReturnsNotStarted(
            [Frozen] NativeDesktopMemoryAndStorage storage,
            ApplicationTypes applicationTypes)
        {
            storage.MinimumCpu = null;

            applicationTypes.NativeDesktopMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static void NativeDesktopMemoryAndStorageStatus_NativeDesktopMemoryAndStorageIsNull_ReturnsNotStarted()
        {
            var applicationTypes = new ApplicationTypes
            {
                NativeDesktopMemoryAndStorage = null,
            };

            applicationTypes.NativeDesktopMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void NativeMobileMemoryAndStorageStatus_MobileMemoryAndStorageValid_ReturnsCompleted(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.NativeMobileMemoryAndStorageStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void NativeMobileMemoryAndStorageStatus_MobileMemoryAndStorageInvalid_ReturnsNotStarted(
            [Frozen] MobileMemoryAndStorage storage,
            ApplicationTypes applicationTypes)
        {
            storage.Description = null;

            applicationTypes.NativeMobileMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static void NativeMobileMemoryAndStorageStatus_MobileMemoryAndStorageIsNull_ReturnsNotStarted()
        {
            var applicationTypes = new ApplicationTypes
            {
                MobileMemoryAndStorage = null,
            };

            applicationTypes.NativeMobileMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeDesktopSupportedOperatingSystemsStatus_DifferentInputs_ResultAsExpected(
            string input,
            TaskProgress expected)
        {
            var applicationTypes = new ApplicationTypes
            {
                NativeDesktopOperatingSystemsDescription = input,
            };

            applicationTypes.NativeDesktopSupportedOperatingSystemsStatus().Should().Be(expected);
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsStatus_ValidMobileOperatingSystems_ReturnsCompleted(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.MobileOperatingSystems.OperatingSystems.Should().NotBeNullOrEmpty();

            applicationTypes.NativeMobileSupportedOperatingSystemsStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsStatus_OperatingSystemsEmpty_ReturnsNotStarted(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.MobileOperatingSystems.OperatingSystems = new HashSet<string>();

            applicationTypes.NativeMobileSupportedOperatingSystemsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsStatus_OperatingSystemsNull_ReturnsNotStarted(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.MobileOperatingSystems.OperatingSystems = null;

            applicationTypes.NativeMobileSupportedOperatingSystemsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsStatus_MobileOperatingSystemsNull_ReturnsNotStarted(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.MobileOperatingSystems = null;

            applicationTypes.NativeMobileSupportedOperatingSystemsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeMobileAdditionalInformationStatus_ValidNativeMobileAdditionalInformation_ReturnsTrue(
            string input,
            TaskProgress expected)
        {
            var applicationTypes = new ApplicationTypes
            {
                NativeMobileAdditionalInformation = input,
            };

            applicationTypes.NativeMobileAdditionalInformationStatus().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeMobileHardwareRequirementsStatus_ValidNativeMobileHardwareRequirements_ReturnsTrue(
            string input,
            TaskProgress expected)
        {
            var applicationTypes = new ApplicationTypes
            {
                NativeMobileHardwareRequirements = input,
            };

            applicationTypes.NativeMobileHardwareRequirementsStatus().Should().Be(expected);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void PluginsStatus_PluginsRequiredHasValue_ReturnsCompleted(bool value)
        {
            var applicationTypes = new ApplicationTypes
            {
                Plugins = new Plugins
                {
                    Required = value,
                },
            };

            applicationTypes.PluginsStatus().Should().Be(TaskProgress.Completed);
        }

        [Fact]
        public static void PluginsStatus_PluginsRequiredHasNoValue_ReturnsNotStarted()
        {
            var applicationTypes = new ApplicationTypes
            {
                Plugins = new Plugins
                {
                    Required = null,
                },
            };

            applicationTypes.PluginsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static void PluginsStatus_PluginsIsNull_ReturnsNotStarted()
        {
            var applicationTypes = new ApplicationTypes
            {
                Plugins = null,
            };

            applicationTypes.PluginsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersStatus_ValidValues_ReturnsCompleted(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.BrowsersSupported.Should().NotBeNullOrEmpty();
            applicationTypes.MobileResponsive.HasValue.Should().BeTrue();

            applicationTypes.SupportedBrowsersStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersStatus_BrowsersSupportedIsEmpty_ReturnsNotStarted(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.BrowsersSupported = new HashSet<SupportedBrowser>();
            applicationTypes.MobileResponsive.HasValue.Should().BeTrue();

            applicationTypes.SupportedBrowsersStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersStatus_BrowsersSupportedIsNull_ReturnsNotStarted(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.BrowsersSupported = null;
            applicationTypes.MobileResponsive.HasValue.Should().BeTrue();

            applicationTypes.SupportedBrowsersStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersStatus_MobileResponsiveIsNull_ReturnsNotStarted(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.BrowsersSupported.Should().NotBeNullOrEmpty();
            applicationTypes.MobileResponsive = null;

            applicationTypes.SupportedBrowsersStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void NativeMobileThirdPartyStatus_MobileThirdPartyValid_ReturnsCompleted(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.NativeMobileThirdPartyStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void TNativeMobileThirdPartyStatus_MobileThirdPartyInvalid_ReturnsNotStarted(
            [Frozen] MobileThirdParty thirdParty,
            ApplicationTypes applicationTypes)
        {
            thirdParty.DeviceCapabilities = null;
            thirdParty.ThirdPartyComponents = null;

            applicationTypes.NativeMobileThirdPartyStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static void NativeMobileThirdPartyStatus_MobileThirdPartyNull_ReturnsNotStarted()
        {
            var applicationTypes = new ApplicationTypes
            {
                MobileThirdParty = null,
            };

            applicationTypes.NativeMobileThirdPartyStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData(ApplicationType.BrowserBased)]
        [CommonInlineAutoData(ApplicationType.MobileTablet)]
        [CommonInlineAutoData(ApplicationType.Desktop)]
        public static void ApplicationTypes_IsUpdatedCorrectly(
            ApplicationType applicationType,
            ApplicationTypes applicationTypes)
        {
            applicationTypes.EnsureApplicationTypePresent(applicationType);

            applicationTypes.ClientApplicationTypes.Should().Contain(applicationType.AsString(EnumFormat.EnumMemberValue));
        }

        [Theory]
        [CommonInlineAutoData(ApplicationType.BrowserBased)]
        [CommonInlineAutoData(ApplicationType.MobileTablet)]
        [CommonInlineAutoData(ApplicationType.Desktop)]
        public static void ApplicationTypeStatus_ReturnsComplete(
            ApplicationType applicationType,
            ApplicationTypes applicationTypes)
        {
            applicationTypes.ApplicationTypeStatus(applicationType).Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_BrowserBased_NoPlugins_ReturnsInProgress(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.Plugins = null;

            applicationTypes.ApplicationTypeStatus(ApplicationType.BrowserBased).Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_BrowserBased_NoSupportedBrowsers_ReturnsInProgress(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.BrowsersSupported = null;

            applicationTypes.ApplicationTypeStatus(ApplicationType.BrowserBased).Should().Be(TaskProgress.InProgress);
        }

        [Fact]
        public static void ApplicationTypeStatus_BrowserBased_NothingComplete_ReturnsNotStarted()
        {
            var applicationType = new ApplicationTypes();

            applicationType.ApplicationTypeStatus(ApplicationType.BrowserBased).Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_DesktopBased_NoOperatingSystemDescription_ReturnsInProgress(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.NativeDesktopOperatingSystemsDescription = null;

            applicationTypes.ApplicationTypeStatus(ApplicationType.Desktop).Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_DesktopBased_NoNativeDesktopMinimumConnectionSpeed_ReturnsInProgress(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.NativeDesktopMinimumConnectionSpeed = null;

            applicationTypes.ApplicationTypeStatus(ApplicationType.Desktop).Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_DesktopBased_NoNativeDesktopMemoryAndStorage_ReturnsInProgress(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.NativeDesktopMemoryAndStorage = null;

            applicationTypes.ApplicationTypeStatus(ApplicationType.Desktop).Should().Be(TaskProgress.InProgress);
        }

        [Fact]
        public static void ApplicationTypeStatus_DesktopBased_NothingComplete_ReturnsNotStarted()
        {
            var applicationTypes = new ApplicationTypes();

            applicationTypes.ApplicationTypeStatus(ApplicationType.BrowserBased).Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_MobileTablet_NoMobileOperatingSystems_ReturnsInProgress(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.MobileOperatingSystems = null;

            applicationTypes.ApplicationTypeStatus(ApplicationType.MobileTablet).Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_MobileTablet_NoMobileMemoryAndStorage_ReturnsInProgress(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.MobileMemoryAndStorage = null;

            applicationTypes.ApplicationTypeStatus(ApplicationType.MobileTablet).Should().Be(TaskProgress.InProgress);
        }

        [Fact]
        public static void ApplicationTypeStatus_MobileTablet_NothingComplete_ReturnsNotStarted()
        {
            var applicationTypes = new ApplicationTypes();

            applicationTypes.ApplicationTypeStatus(ApplicationType.BrowserBased).Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void HasApplicationType_True(
            ApplicationTypes applicationTypes)
        {
            applicationTypes.EnsureApplicationTypePresent(ApplicationType.BrowserBased);

            applicationTypes.HasApplicationType(ApplicationType.BrowserBased).Should().BeTrue();
        }

        [Fact]
        public static void HasApplicationType_False()
        {
            var applicationTypes = new ApplicationTypes();

            applicationTypes.HasApplicationType(ApplicationType.BrowserBased).Should().BeFalse();
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
