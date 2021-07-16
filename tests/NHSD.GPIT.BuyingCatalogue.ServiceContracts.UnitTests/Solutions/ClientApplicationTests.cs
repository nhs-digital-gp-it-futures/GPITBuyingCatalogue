using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    public static class ClientApplicationTests
    {
        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void AdditionalInformationComplete_DifferentInputs_ResultAsExpected(
            string input,
            bool expected)
        {
            var clientApplication = new ClientApplication
            {
                AdditionalInformation = input,
            };

            clientApplication.AdditionalInformationComplete().Should().Be(expected);
        }

        [Fact]
        public static void BrowserBasedModelComplete_AllChecksTrue_ReturnsTrue()
        {
            var clientApplication = new Mock<ClientApplication> { CallBase = true };
            clientApplication.Setup(x => x.SupportedBrowsersComplete())
                .Returns(true);
            clientApplication.Setup(x => x.ConnectivityAndResolutionComplete())
                .Returns(true);
            clientApplication.Setup(x => x.MobileFirstDesignComplete())
                .Returns(true);
            clientApplication.Setup(x => x.PlugInsComplete())
                .Returns(true);

            clientApplication.Object.BrowserBasedModelComplete().Should().BeTrue();
        }

        [Fact]
        public static void BrowserBasedModelComplete_SupportedBrowsersCompleteReturnsFalse_ReturnsFalse()
        {
            var clientApplication = new Mock<ClientApplication> { CallBase = true };
            clientApplication.Setup(x => x.SupportedBrowsersComplete())
                .Returns(false);
            clientApplication.Setup(x => x.ConnectivityAndResolutionComplete())
                .Returns(true);
            clientApplication.Setup(x => x.NativeMobileFirstApproachComplete())
                .Returns(true);
            clientApplication.Setup(x => x.PlugInsComplete())
                .Returns(true);

            clientApplication.Object.BrowserBasedModelComplete().Should().BeFalse();
        }

        [Fact]
        public static void BrowserBasedModelComplete_ConnectivityAndResolutionCompleteReturnsFalse_ReturnsFalse()
        {
            var clientApplication = new Mock<ClientApplication> { CallBase = true };
            clientApplication.Setup(x => x.SupportedBrowsersComplete())
                .Returns(true);
            clientApplication.Setup(x => x.ConnectivityAndResolutionComplete())
                .Returns(false);
            clientApplication.Setup(x => x.NativeMobileFirstApproachComplete())
                .Returns(true);
            clientApplication.Setup(x => x.PlugInsComplete())
                .Returns(true);

            clientApplication.Object.BrowserBasedModelComplete().Should().BeFalse();
        }

        [Fact]
        public static void BrowserBasedModelComplete_NativeMobileFirstApproachCompleteReturnsFalse_ReturnsFalse()
        {
            var clientApplication = new Mock<ClientApplication> { CallBase = true };
            clientApplication.Setup(x => x.SupportedBrowsersComplete())
                .Returns(true);
            clientApplication.Setup(x => x.ConnectivityAndResolutionComplete())
                .Returns(true);
            clientApplication.Setup(x => x.NativeMobileFirstApproachComplete())
                .Returns(false);
            clientApplication.Setup(x => x.PlugInsComplete())
                .Returns(true);

            clientApplication.Object.BrowserBasedModelComplete().Should().BeFalse();
        }

        [Fact]
        public static void BrowserBasedModelComplete_PlugInsOrExtensionsCompleteReturnsFalse_ReturnsFalse()
        {
            var clientApplication = new Mock<ClientApplication> { CallBase = true };
            clientApplication.Setup(x => x.SupportedBrowsersComplete())
                .Returns(true);
            clientApplication.Setup(x => x.ConnectivityAndResolutionComplete())
                .Returns(true);
            clientApplication.Setup(x => x.NativeMobileFirstApproachComplete())
                .Returns(true);
            clientApplication.Setup(x => x.PlugInsComplete())
                .Returns(false);

            clientApplication.Object.BrowserBasedModelComplete().Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void ConnectivityAndResolutionComplete_DifferentInputs_ResultAsExpected(
            string input,
            bool expected)
        {
            var clientApplication = new ClientApplication
            {
                MinimumConnectionSpeed = input,
            };

            clientApplication.ConnectivityAndResolutionComplete().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void HardwareRequirementsComplete_DifferentInputs_ResultAsExpected(
            string input,
            bool expected)
        {
            var clientApplication = new ClientApplication
            {
                HardwareRequirements = input,
            };

            clientApplication.HardwareRequirementsComplete().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeDesktopAdditionalInformationComplete_DifferentInputs_ResultAsExpected(
            string input,
            bool expected)
        {
            var clientApplication = new ClientApplication
            {
                NativeDesktopAdditionalInformation = input,
            };

            clientApplication.NativeDesktopAdditionalInformationComplete().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeDesktopConnectivityComplete_DifferentInputs_ResultAsExpected(
            string input,
            bool expected)
        {
            var clientApplication = new ClientApplication
            {
                NativeDesktopMinimumConnectionSpeed = input,
            };

            clientApplication.NativeDesktopConnectivityComplete().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeDesktopHardwareRequirementsComplete_DifferentInputs_ResultAsExpected(
            string input,
            bool expected)
        {
            var clientApplication = new ClientApplication
            {
                NativeDesktopHardwareRequirements = input,
            };

            clientApplication.NativeDesktopHardwareRequirementsComplete().Should().Be(expected);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void NativeDesktopMemoryComplete_NativeDesktopMemoryAndStorageNotNull_ReturnsIsValidFromIt(
            bool expected)
        {
            var mockNativeDesktopMemoryAndStorage = new Mock<NativeDesktopMemoryAndStorage>();
            mockNativeDesktopMemoryAndStorage.Setup(x => x.IsValid())
                .Returns(expected);
            var clientApplication = new ClientApplication
            {
                NativeDesktopMemoryAndStorage = mockNativeDesktopMemoryAndStorage.Object,
            };

            clientApplication.NativeDesktopMemoryAndStorageComplete().Should().Be(expected);
            mockNativeDesktopMemoryAndStorage.Verify(x => x.IsValid());
        }

        [Fact]
        public static void NativeDesktopMemoryComplete_NativeDesktopMemoryAndStorageIsNull_ReturnsNull()
        {
            var clientApplication = new ClientApplication
            {
                NativeDesktopMemoryAndStorage = null,
            };

            clientApplication.NativeDesktopMemoryAndStorageComplete().Should().BeNull();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void NativeMobileMemoryAndStorageComplete_MobileMemoryAndStorageNotNull_ReturnsIsValidFromIt(
            bool expected)
        {
            var mockMobileMemoryAndStorage = new Mock<MobileMemoryAndStorage>();
            mockMobileMemoryAndStorage.Setup(x => x.IsValid())
                .Returns(expected);
            var clientApplication = new ClientApplication
            {
                MobileMemoryAndStorage = mockMobileMemoryAndStorage.Object,
            };

            clientApplication.NativeMobileMemoryAndStorageComplete().Should().Be(expected);
            mockMobileMemoryAndStorage.Verify(x => x.IsValid());
        }

        [Fact]
        public static void NativeMobileMemoryAndStorageComplete_MobileMemoryAndStorageIsNull_ReturnsNull()
        {
            var clientApplication = new ClientApplication
            {
                MobileMemoryAndStorage = null,
            };

            clientApplication.NativeMobileMemoryAndStorageComplete().Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeDesktopSupportedOperatingSystemsComplete_DifferentInputs_ResultAsExpected(
            string input,
            bool expected)
        {
            var clientApplication = new ClientApplication
            {
                NativeDesktopOperatingSystemsDescription = input,
            };

            clientApplication.NativeDesktopSupportedOperatingSystemsComplete().Should().Be(expected);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void NativeMobileFirstApproachComplete_MobileFirstDesignHasValue_ReturnsTrue(bool value)
        {
            var clientApplication = new ClientApplication
            {
                NativeMobileFirstDesign = value,
            };

            clientApplication.NativeMobileFirstApproachComplete().Should().BeTrue();
        }

        [Fact]
        public static void NativeMobileFirstApproachComplete_MobileFirstDesignHasNoValue_ReturnsFalse()
        {
            var clientApplication = new ClientApplication
            {
                MobileFirstDesign = null,
            };

            clientApplication.NativeMobileFirstApproachComplete().Should().BeFalse();
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsComplete_ValidMobileOperatingSystems_ReturnsTrue(
            ClientApplication clientApplication)
        {
            clientApplication.MobileOperatingSystems.OperatingSystems.Should().NotBeNullOrEmpty();

            clientApplication.NativeMobileSupportedOperatingSystemsComplete().Should().BeTrue();
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsComplete_OperatingSystemsEmpty_ReturnsFalse(
            ClientApplication clientApplication)
        {
            clientApplication.MobileOperatingSystems.OperatingSystems = new HashSet<string>();

            clientApplication.NativeMobileSupportedOperatingSystemsComplete().Should().BeFalse();
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsComplete_OperatingSystemsNull_ReturnsNull(
            ClientApplication clientApplication)
        {
            clientApplication.MobileOperatingSystems.OperatingSystems = null;

            clientApplication.NativeMobileSupportedOperatingSystemsComplete().Should().BeNull();
        }

        [Theory]
        [AutoData]
        public static void NativeMobileSupportedOperatingSystemsComplete_MobileOperatingSystemsNull_ReturnsNull(
            ClientApplication clientApplication)
        {
            clientApplication.MobileOperatingSystems = null;

            clientApplication.NativeMobileSupportedOperatingSystemsComplete().Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeMobileAdditionalInformationComplete_ValidNativeMobileAdditionalInformation_ReturnsTrue(
            string input,
            bool expected)
        {
            var clientApplication = new ClientApplication
            {
                NativeMobileAdditionalInformation = input,
            };

            clientApplication.NativeMobileAdditionalInformationComplete().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void NativeMobileHardwareRequirementsComplete_ValidNativeMobileHardwareRequirements_ReturnsTrue(
            string input,
            bool expected)
        {
            var clientApplication = new ClientApplication
            {
                NativeMobileHardwareRequirements = input,
            };

            clientApplication.NativeMobileHardwareRequirementsComplete().Should().Be(expected);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void PlugInsComplete_PluginsRequiredHasValue_ReturnsTrue(bool value)
        {
            var clientApplication = new ClientApplication
            {
                Plugins = new Plugins
                {
                    Required = value,
                },
            };

            clientApplication.PlugInsComplete().Should().BeTrue();
        }

        [Fact]
        public static void PlugInsComplete_PluginsRequiredHasNoValue_ReturnsFalse()
        {
            var clientApplication = new ClientApplication
            {
                Plugins = new Plugins
                {
                    Required = null,
                },
            };

            clientApplication.PlugInsComplete().Should().BeFalse();
        }

        [Fact]
        public static void PlugInsComplete_PluginsIsNull_ReturnsNull()
        {
            var clientApplication = new ClientApplication
            {
                Plugins = null,
            };

            clientApplication.PlugInsComplete().Should().BeNull();
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersComplete_ValidValues_ReturnsTrue(
            ClientApplication clientApplication)
        {
            clientApplication.BrowsersSupported.Should().NotBeNullOrEmpty();
            clientApplication.MobileResponsive.HasValue.Should().BeTrue();

            clientApplication.SupportedBrowsersComplete().Should().BeTrue();
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersComplete_BrowsersSupportedIsEmpty_ReturnsFalse(
            ClientApplication clientApplication)
        {
            clientApplication.BrowsersSupported = new HashSet<string>();
            clientApplication.MobileResponsive.HasValue.Should().BeTrue();

            clientApplication.SupportedBrowsersComplete().Should().BeFalse();
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersComplete_BrowsersSupportedIsNull_ReturnsFalse(
            ClientApplication clientApplication)
        {
            clientApplication.BrowsersSupported = null;
            clientApplication.MobileResponsive.HasValue.Should().BeTrue();

            clientApplication.SupportedBrowsersComplete().Should().BeFalse();
        }

        [Theory]
        [AutoData]
        public static void SupportedBrowsersComplete_MobileResponsiveIsNull_ReturnsFalse(
            ClientApplication clientApplication)
        {
            clientApplication.BrowsersSupported.Should().NotBeNullOrEmpty();
            clientApplication.MobileResponsive = null;

            clientApplication.SupportedBrowsersComplete().Should().BeFalse();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void ThirdPartyComplete_MobileThirdPartyNotNull_ReturnsIsValid(bool expected)
        {
            var mockMobileThirdParty = new Mock<MobileThirdParty>();
            mockMobileThirdParty.Setup(m => m.IsValid())
                .Returns(expected);
            var clientApplication = new ClientApplication
            {
                MobileThirdParty = mockMobileThirdParty.Object,
            };

            clientApplication.NativeMobileThirdPartyComplete().Should().Be(expected);
        }

        [Fact]
        public static void ThirdPartyComplete_MobileThirdPartyNull_ReturnsNull()
        {
            var clientApplication = new ClientApplication
            {
                MobileThirdParty = null,
            };

            clientApplication.NativeMobileThirdPartyComplete().Should().BeNull();
        }

        private static class ResultSetData
        {
            public static IEnumerable<object[]> TestData()
            {
                yield return new object[] { "some-value", true };
                yield return new object[] { null, false };
                yield return new object[] { string.Empty, false };
                yield return new object[] { "     ", false };
            }
        }
    }
}
