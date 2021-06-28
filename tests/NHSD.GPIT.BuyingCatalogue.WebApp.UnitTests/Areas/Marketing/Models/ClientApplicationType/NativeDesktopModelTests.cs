using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.ClientApplicationType
{
    public static class NativeDesktopModelTests
    {
        private const string KeyIncomplete = "INCOMPLETE";

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void AdditionalInformationStatus_Various_NativeDesktopAdditionalInformationComplete_ResultAsExpected(
            bool? complete,
            string expected)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeDesktopAdditionalInformationComplete())
                .Returns(complete);
            var nativeDesktopModel = new NativeDesktopModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeDesktopModel.AdditionalInformationStatus.Should().Be(expected);
            mockClientApplication.Verify(c => c.NativeDesktopAdditionalInformationComplete());
        }

        [Fact]
        public static void ClientApplication_Null_ValuesFalseOrIncomplete()
        {
            var nativeDesktopModel = new NativeDesktopModel();
            nativeDesktopModel.ClientApplication.Should().BeNull();

            nativeDesktopModel.IsComplete.Should().BeFalse();
            nativeDesktopModel.ConnectivityStatus.Should().Be(KeyIncomplete);
            nativeDesktopModel.HardwareRequirementsStatus.Should().Be(KeyIncomplete);
            nativeDesktopModel.MemoryAndStorageStatus.Should().Be(KeyIncomplete);
            nativeDesktopModel.SupportedOperatingSystemsStatus.Should().Be(KeyIncomplete);
            nativeDesktopModel.ThirdPartyStatus.Should().Be(KeyIncomplete);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void ConnectivityStatus_Various_NativeDesktopConnectivityComplete_ResultAsExpected(
            bool? complete,
            string expected)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeDesktopConnectivityComplete())
                .Returns(complete);
            var nativeDesktopModel = new NativeDesktopModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeDesktopModel.ConnectivityStatus.Should().Be(expected);
            mockClientApplication.Verify(c => c.NativeDesktopConnectivityComplete());
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void HardwareRequirementsStatus_Various_NativeDesktopHardwareRequirementsComplete_ResultAsExpected(
            bool? complete,
            string expected)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeDesktopHardwareRequirementsComplete())
                .Returns(complete);
            var nativeDesktopModel = new NativeDesktopModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeDesktopModel.HardwareRequirementsStatus.Should().Be(expected);
            mockClientApplication.Verify(c => c.NativeDesktopHardwareRequirementsComplete());
        }

        [Fact]
        public static void IsComplete_AllValuesValid_ReturnsTrue()
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeDesktopSupportedOperatingSystemsComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeDesktopConnectivityComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeDesktopMemoryAndStorageComplete())
                .Returns(true);
            var nativeDesktopModel = new NativeDesktopModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeDesktopModel.IsComplete.Should().BeTrue();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(null)]
        public static void IsComplete_NativeDesktopSupportedOperatingSystemsComplete_NotTrue_ReturnsFalse(bool? value)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeDesktopSupportedOperatingSystemsComplete())
                .Returns(value);
            mockClientApplication.Setup(c => c.NativeDesktopConnectivityComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeDesktopMemoryAndStorageComplete())
                .Returns(true);
            var nativeDesktopModel = new NativeDesktopModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeDesktopModel.IsComplete.Should().BeFalse();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(null)]
        public static void IsComplete_NativeDesktopConnectivityComplete_NotTrue_ReturnsFalse(bool? value)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeDesktopSupportedOperatingSystemsComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeDesktopConnectivityComplete())
                .Returns(value);
            mockClientApplication.Setup(c => c.NativeDesktopMemoryAndStorageComplete())
                .Returns(true);
            var nativeDesktopModel = new NativeDesktopModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeDesktopModel.IsComplete.Should().BeFalse();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(null)]
        public static void IsComplete_NativeDesktopMemoryComplete_NotTrue_ReturnsFalse(bool? value)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeDesktopSupportedOperatingSystemsComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeDesktopConnectivityComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeDesktopMemoryAndStorageComplete())
                .Returns(value);
            var nativeDesktopModel = new NativeDesktopModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeDesktopModel.IsComplete.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void MemoryAndStorageStatus_Various_NativeDesktopMemoryAndStorageComplete_ResultAsExpected(
            bool? complete,
            string expected)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeDesktopMemoryAndStorageComplete())
                .Returns(complete);
            var nativeDesktopModel = new NativeDesktopModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeDesktopModel.MemoryAndStorageStatus.Should().Be(expected);
            mockClientApplication.Verify(c => c.NativeDesktopMemoryAndStorageComplete());
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void SupportedOperatingSystemsStatus_Various_NativeDesktopSupportedOperatingSystemsComplete_ResultAsExpected(
            bool? complete,
            string expected)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeDesktopSupportedOperatingSystemsComplete())
                .Returns(complete);
            var nativeDesktopModel = new NativeDesktopModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeDesktopModel.SupportedOperatingSystemsStatus.Should().Be(expected);
            mockClientApplication.Verify(c => c.NativeDesktopSupportedOperatingSystemsComplete());
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void ThirdPartyStatus_Various_NativeDesktopThirdPartyComplete_ResultAsExpected(
            bool? complete,
            string expected)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeDesktopThirdPartyComplete())
                .Returns(complete);
            var nativeDesktopModel = new NativeDesktopModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeDesktopModel.ThirdPartyStatus.Should().Be(expected);
            mockClientApplication.Verify(c => c.NativeDesktopThirdPartyComplete());
        }

        private static class ResultSetData
        {
            public static IEnumerable<object[]> TestData()
            {
                yield return new object[] { null, KeyIncomplete };
                yield return new object[] { false, KeyIncomplete };
                yield return new object[] { true, "COMPLETE" };
            }
        }
    }
}
