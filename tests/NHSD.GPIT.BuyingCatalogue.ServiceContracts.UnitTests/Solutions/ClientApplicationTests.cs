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
    public static class ClientApplicationTests
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

            clientApplication.AdditionalInformationStatus().Should().Be(expected);
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

            clientApplication.ConnectivityStatus().Should().Be(expected);
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

            clientApplication.HardwareRequirementsStatus().Should().Be(expected);
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

            clientApplication.NativeDesktopAdditionalInformationStatus().Should().Be(expected);
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

            clientApplication.NativeDesktopConnectivityStatus().Should().Be(expected);
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

            clientApplication.NativeDesktopHardwareRequirementsStatus().Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void NativeDesktopMemoryAndStorageStatus_NativeDesktopMemoryAndStorageValid_ReturnsCompleted(
            ClientApplication clientApplication)
        {
            clientApplication.NativeDesktopMemoryAndStorageStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void NativeDesktopMemoryAndStorageStatus_NativeDesktopMemoryAndStorageInvalid_ReturnsNotStarted(
            [Frozen] NativeDesktopMemoryAndStorage storage,
            ClientApplication clientApplication)
        {
            storage.MinimumCpu = null;

            clientApplication.NativeDesktopMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static void NativeDesktopMemoryAndStorageStatus_NativeDesktopMemoryAndStorageIsNull_ReturnsNotStarted()
        {
            var clientApplication = new ClientApplication
            {
                NativeDesktopMemoryAndStorage = null,
            };

            clientApplication.NativeDesktopMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void NativeMobileMemoryAndStorageStatus_MobileMemoryAndStorageValid_ReturnsCompleted(
            ClientApplication clientApplication)
        {
            clientApplication.NativeMobileMemoryAndStorageStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void NativeMobileMemoryAndStorageStatus_MobileMemoryAndStorageInvalid_ReturnsNotStarted(
            [Frozen] MobileMemoryAndStorage storage,
            ClientApplication clientApplication)
        {
            storage.Description = null;

            clientApplication.NativeMobileMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static void NativeMobileMemoryAndStorageStatus_MobileMemoryAndStorageIsNull_ReturnsNotStarted()
        {
            var clientApplication = new ClientApplication
            {
                MobileMemoryAndStorage = null,
            };

            clientApplication.NativeMobileMemoryAndStorageStatus().Should().Be(TaskProgress.NotStarted);
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

            clientApplication.NativeDesktopSupportedOperatingSystemsStatus().Should().Be(expected);
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsStatus_ValidMobileOperatingSystems_ReturnsCompleted(
            ClientApplication clientApplication)
        {
            clientApplication.MobileOperatingSystems.OperatingSystems.Should().NotBeNullOrEmpty();

            clientApplication.NativeMobileSupportedOperatingSystemsStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsStatus_OperatingSystemsEmpty_ReturnsNotStarted(
            ClientApplication clientApplication)
        {
            clientApplication.MobileOperatingSystems.OperatingSystems = new HashSet<string>();

            clientApplication.NativeMobileSupportedOperatingSystemsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsStatus_OperatingSystemsNull_ReturnsNotStarted(
            ClientApplication clientApplication)
        {
            clientApplication.MobileOperatingSystems.OperatingSystems = null;

            clientApplication.NativeMobileSupportedOperatingSystemsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsStatus_MobileOperatingSystemsNull_ReturnsNotStarted(
            ClientApplication clientApplication)
        {
            clientApplication.MobileOperatingSystems = null;

            clientApplication.NativeMobileSupportedOperatingSystemsStatus().Should().Be(TaskProgress.NotStarted);
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

            clientApplication.NativeMobileAdditionalInformationStatus().Should().Be(expected);
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

            clientApplication.NativeMobileHardwareRequirementsStatus().Should().Be(expected);
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

            clientApplication.PluginsStatus().Should().Be(TaskProgress.Completed);
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

            clientApplication.PluginsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static void PluginsStatus_PluginsIsNull_ReturnsNotStarted()
        {
            var clientApplication = new ClientApplication
            {
                Plugins = null,
            };

            clientApplication.PluginsStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersStatus_ValidValues_ReturnsCompleted(
            ClientApplication clientApplication)
        {
            clientApplication.BrowsersSupported.Should().NotBeNullOrEmpty();
            clientApplication.MobileResponsive.HasValue.Should().BeTrue();

            clientApplication.SupportedBrowsersStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersStatus_BrowsersSupportedIsEmpty_ReturnsNotStarted(
            ClientApplication clientApplication)
        {
            clientApplication.BrowsersSupported = new HashSet<SupportedBrowser>();
            clientApplication.MobileResponsive.HasValue.Should().BeTrue();

            clientApplication.SupportedBrowsersStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersStatus_BrowsersSupportedIsNull_ReturnsNotStarted(
            ClientApplication clientApplication)
        {
            clientApplication.BrowsersSupported = null;
            clientApplication.MobileResponsive.HasValue.Should().BeTrue();

            clientApplication.SupportedBrowsersStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersStatus_MobileResponsiveIsNull_ReturnsNotStarted(
            ClientApplication clientApplication)
        {
            clientApplication.BrowsersSupported.Should().NotBeNullOrEmpty();
            clientApplication.MobileResponsive = null;

            clientApplication.SupportedBrowsersStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void NativeMobileThirdPartyStatus_MobileThirdPartyValid_ReturnsCompleted(
            ClientApplication clientApplication)
        {
            clientApplication.NativeMobileThirdPartyStatus().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void TNativeMobileThirdPartyStatus_MobileThirdPartyInvalid_ReturnsNotStarted(
            [Frozen] MobileThirdParty thirdParty,
            ClientApplication clientApplication)
        {
            thirdParty.DeviceCapabilities = null;
            thirdParty.ThirdPartyComponents = null;

            clientApplication.NativeMobileThirdPartyStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Fact]
        public static void NativeMobileThirdPartyStatus_MobileThirdPartyNull_ReturnsNotStarted()
        {
            var clientApplication = new ClientApplication
            {
                MobileThirdParty = null,
            };

            clientApplication.NativeMobileThirdPartyStatus().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData(ApplicationType.BrowserBased)]
        [CommonInlineAutoData(ApplicationType.MobileTablet)]
        [CommonInlineAutoData(ApplicationType.Desktop)]
        public static void ClientApplicationTypes_IsUpdatedCorrectly(
            ApplicationType clientApplicationType,
            ClientApplication clientApplication)
        {
            clientApplication.EnsureApplicationTypePresent(clientApplicationType);

            clientApplication.ClientApplicationTypes.Should().Contain(clientApplicationType.AsString(EnumFormat.EnumMemberValue));
        }

        [Theory]
        [CommonInlineAutoData(ApplicationType.BrowserBased)]
        [CommonInlineAutoData(ApplicationType.MobileTablet)]
        [CommonInlineAutoData(ApplicationType.Desktop)]
        public static void ApplicationTypeStatus_ReturnsComplete(
            ApplicationType clientApplicationType,
            ClientApplication clientApplication)
        {
            clientApplication.ApplicationTypeStatus(clientApplicationType).Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_BrowserBased_NoPlugins_ReturnsInProgress(
            ClientApplication clientApplication)
        {
            clientApplication.Plugins = null;

            clientApplication.ApplicationTypeStatus(ApplicationType.BrowserBased).Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_BrowserBased_NoSupportedBrowsers_ReturnsInProgress(
            ClientApplication clientApplication)
        {
            clientApplication.BrowsersSupported = null;

            clientApplication.ApplicationTypeStatus(ApplicationType.BrowserBased).Should().Be(TaskProgress.InProgress);
        }

        [Fact]
        public static void ApplicationTypeStatus_BrowserBased_NothingComplete_ReturnsNotStarted()
        {
            var clientApplication = new ClientApplication();

            clientApplication.ApplicationTypeStatus(ApplicationType.BrowserBased).Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_DesktopBased_NoOperatingSystemDescription_ReturnsInProgress(
            ClientApplication clientApplication)
        {
            clientApplication.NativeDesktopOperatingSystemsDescription = null;

            clientApplication.ApplicationTypeStatus(ApplicationType.Desktop).Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_DesktopBased_NoNativeDesktopMinimumConnectionSpeed_ReturnsInProgress(
            ClientApplication clientApplication)
        {
            clientApplication.NativeDesktopMinimumConnectionSpeed = null;

            clientApplication.ApplicationTypeStatus(ApplicationType.Desktop).Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_DesktopBased_NoNativeDesktopMemoryAndStorage_ReturnsInProgress(
            ClientApplication clientApplication)
        {
            clientApplication.NativeDesktopMemoryAndStorage = null;

            clientApplication.ApplicationTypeStatus(ApplicationType.Desktop).Should().Be(TaskProgress.InProgress);
        }

        [Fact]
        public static void ApplicationTypeStatus_DesktopBased_NothingComplete_ReturnsNotStarted()
        {
            var clientApplication = new ClientApplication();

            clientApplication.ApplicationTypeStatus(ApplicationType.BrowserBased).Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_MobileTablet_NoMobileOperatingSystems_ReturnsInProgress(
            ClientApplication clientApplication)
        {
            clientApplication.MobileOperatingSystems = null;

            clientApplication.ApplicationTypeStatus(ApplicationType.MobileTablet).Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData]
        public static void ApplicationTypeStatus_MobileTablet_NoMobileMemoryAndStorage_ReturnsInProgress(
            ClientApplication clientApplication)
        {
            clientApplication.MobileMemoryAndStorage = null;

            clientApplication.ApplicationTypeStatus(ApplicationType.MobileTablet).Should().Be(TaskProgress.InProgress);
        }

        [Fact]
        public static void ApplicationTypeStatus_MobileTablet_NothingComplete_ReturnsNotStarted()
        {
            var clientApplication = new ClientApplication();

            clientApplication.ApplicationTypeStatus(ApplicationType.BrowserBased).Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void HasClientApplicationType_True(
            ClientApplication clientApplication)
        {
            clientApplication.EnsureApplicationTypePresent(ApplicationType.BrowserBased);

            clientApplication.HasApplicationType(ApplicationType.BrowserBased).Should().BeTrue();
        }

        [Fact]
        public static void HasClientApplicationType_False()
        {
            var clientApplication = new ClientApplication();

            clientApplication.HasApplicationType(ApplicationType.BrowserBased).Should().BeFalse();
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
