using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.ClientApplicationType
{
    public static class NativeMobileModelTests
    {
        private const string KeyIncomplete = "INCOMPLETE";

        [Theory]
        [InlineData(false, KeyIncomplete)]
        [InlineData(true, "COMPLETE")]
        public static void AdditionalInformationStatus_Various_NativeMobileAdditionalInformationComplete_ResultAsExpected(
            bool complete,
            string expected)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileAdditionalInformationComplete())
                .Returns(complete);
            var nativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeMobileModel.AdditionalInformationStatus.Should().Be(expected);
            mockClientApplication.Verify(c => c.NativeMobileAdditionalInformationComplete());
        }

        [Fact]
        public static void ClientApplication_Null_ValuesFalseOrIncomplete()
        {
            var nativeMobileModel = new NativeMobileModel();
            nativeMobileModel.ClientApplication.Should().BeNull();

            nativeMobileModel.IsComplete.Should().BeFalse();
            nativeMobileModel.ConnectivityStatus.Should().Be(KeyIncomplete);
            nativeMobileModel.HardwareRequirementsStatus.Should().Be(KeyIncomplete);
            nativeMobileModel.MemoryAndStorageStatus.Should().Be(KeyIncomplete);
            nativeMobileModel.SupportedOperatingSystemsStatus.Should().Be(KeyIncomplete);
            nativeMobileModel.ThirdPartyStatus.Should().Be(KeyIncomplete);
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void ConnectivityStatus_Various_NativeMobileConnectivityComplete_ResultAsExpected(
            bool? complete,
            string expected)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileConnectivityComplete())
                .Returns(complete);
            var nativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeMobileModel.ConnectivityStatus.Should().Be(expected);
            mockClientApplication.Verify(c => c.NativeMobileConnectivityComplete());
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void HardwareRequirementsStatus_Various_NativeMobileHardwareRequirementsComplete_ResultAsExpected(
            bool? complete,
            string expected)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileHardwareRequirementsComplete())
                .Returns(complete);
            var nativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeMobileModel.HardwareRequirementsStatus.Should().Be(expected);
            mockClientApplication.Verify(c => c.NativeMobileHardwareRequirementsComplete());
        }

        [Fact]
        public static void IsComplete_AllValuesValid_ReturnsTrue()
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileSupportedOperatingSystemsComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeMobileFirstApproachComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeMobileMemoryAndStorageComplete())
                .Returns(true);
            var nativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeMobileModel.IsComplete.Should().BeTrue();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(null)]
        public static void IsComplete_NativeMobileSupportedOperatingSystemsComplete_NotTrue_ReturnsFalse(bool? value)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileSupportedOperatingSystemsComplete())
                .Returns(value);
            mockClientApplication.Setup(c => c.NativeMobileFirstApproachComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeMobileMemoryAndStorageComplete())
                .Returns(true);
            var nativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeMobileModel.IsComplete.Should().BeFalse();
        }

        [Fact]
        public static void IsComplete_NativeMobileFirstApproachComplete_NotTrue_ReturnsFalse()
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileSupportedOperatingSystemsComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeMobileFirstApproachComplete())
                .Returns(false);
            mockClientApplication.Setup(c => c.NativeMobileMemoryAndStorageComplete())
                .Returns(true);
            var nativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeMobileModel.IsComplete.Should().BeFalse();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(null)]
        public static void IsComplete_NativeMobileMemoryComplete_NotTrue_ReturnsFalse(bool? value)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileSupportedOperatingSystemsComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeMobileFirstApproachComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeMobileMemoryAndStorageComplete())
                .Returns(value);
            var nativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeMobileModel.IsComplete.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void MemoryAndStorageStatus_Various_NativeMobileMemoryAndStorageComplete_ResultAsExpected(
            bool? complete,
            string expected)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileMemoryAndStorageComplete())
                .Returns(complete);
            var nativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeMobileModel.MemoryAndStorageStatus.Should().Be(expected);
            mockClientApplication.Verify(c => c.NativeMobileMemoryAndStorageComplete());
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void SupportedOperatingSystemsStatus_Various_NativeMobileSupportedOperatingSystemsComplete_ResultAsExpected(
            bool? complete,
            string expected)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileSupportedOperatingSystemsComplete())
                .Returns(complete);
            var nativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeMobileModel.SupportedOperatingSystemsStatus.Should().Be(expected);
            mockClientApplication.Verify(c => c.NativeMobileSupportedOperatingSystemsComplete());
        }

        [Theory]
        [MemberData(nameof(ResultSetData.TestData), MemberType = typeof(ResultSetData))]
        public static void ThirdPartyStatus_Various_NativeMobileThirdPartyComplete_ResultAsExpected(
            bool? complete,
            string expected)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileThirdPartyComplete())
                .Returns(complete);
            var nativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            nativeMobileModel.ThirdPartyStatus.Should().Be(expected);
            mockClientApplication.Verify(c => c.NativeMobileThirdPartyComplete());
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
