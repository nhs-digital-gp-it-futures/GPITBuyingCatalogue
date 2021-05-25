using System;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.ClientApplicationType

{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class NativeMobileModelTests
    {
        private const string KeyIncomplete = "INCOMPLETE";

        private static readonly object[] ResultSets =
        {
            new object[]{null, KeyIncomplete},
            new object[]{false, KeyIncomplete},
            new object[]{true, "COMPLETE"},
        };
        
        [TestCase(false, KeyIncomplete)]
        [TestCase(true, "COMPLETE")]
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

        [Test]
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

        [TestCaseSource(nameof(ResultSets))]
        public static void ConnectivityStatus_Various_NativeMobileConnectivityComplete_ResultAsExpected(
            bool? complete,
            string expected)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileConnectivityComplete())
                .Returns(complete);
            var NativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            NativeMobileModel.ConnectivityStatus.Should().Be(expected);
            mockClientApplication.Verify(c => c.NativeMobileConnectivityComplete());
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void HardwareRequirementsStatus_Various_NativeMobileHardwareRequirementsComplete_ResultAsExpected(
            bool? complete,
            string expected)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileHardwareRequirementsComplete())
                .Returns(complete);
            var NativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            NativeMobileModel.HardwareRequirementsStatus.Should().Be(expected);
            mockClientApplication.Verify(c => c.NativeMobileHardwareRequirementsComplete());
        }

        [Test]
        public static void IsComplete_AllValuesValid_ReturnsTrue()
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileSupportedOperatingSystemsComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeMobileFirstApproachComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeMobileMemoryAndStorageComplete())
                .Returns(true);
            var NativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            NativeMobileModel.IsComplete.Should().BeTrue();
        }

        [TestCase(false)]
        [TestCase(null)]
        public static void IsComplete_NativeMobileSupportedOperatingSystemsComplete_NotTrue_ReturnsFalse(bool value)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileSupportedOperatingSystemsComplete())
                .Returns(value);
            mockClientApplication.Setup(c => c.NativeMobileFirstApproachComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeMobileMemoryAndStorageComplete())
                .Returns(true);
            var NativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            NativeMobileModel.IsComplete.Should().BeFalse();
        }

        [TestCase(false)]
        [TestCase(null)]
        public static void IsComplete_NativeMobileFirstApproachComplete_NotTrue_ReturnsFalse(bool value)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileSupportedOperatingSystemsComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeMobileFirstApproachComplete())
                .Returns(value);
            mockClientApplication.Setup(c => c.NativeMobileMemoryAndStorageComplete())
                .Returns(true);
            var NativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            NativeMobileModel.IsComplete.Should().BeFalse();
        }

        [TestCase(false)]
        [TestCase(null)]
        public static void IsComplete_NativeMobileMemoryComplete_NotTrue_ReturnsFalse(bool value)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileSupportedOperatingSystemsComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeMobileFirstApproachComplete())
                .Returns(true);
            mockClientApplication.Setup(c => c.NativeMobileMemoryAndStorageComplete())
                .Returns(value);
            var NativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            NativeMobileModel.IsComplete.Should().BeFalse();
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void MemoryAndStorageStatus_Various_NativeMobileMemoryAndStorageComplete_ResultAsExpected(
            bool? complete,
            string expected)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileMemoryAndStorageComplete())
                .Returns(complete);
            var NativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            NativeMobileModel.MemoryAndStorageStatus.Should().Be(expected);
            mockClientApplication.Verify(c => c.NativeMobileMemoryAndStorageComplete());
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void SupportedOperatingSystemsStatus_Various_NativeMobileSupportedOperatingSystemsComplete_ResultAsExpected(
            bool? complete,
            string expected)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileSupportedOperatingSystemsComplete())
                .Returns(complete);
            var NativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            NativeMobileModel.SupportedOperatingSystemsStatus.Should().Be(expected);
            mockClientApplication.Verify(c => c.NativeMobileSupportedOperatingSystemsComplete());
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void ThirdPartyStatus_Various_NativeMobileThirdPartyComplete_ResultAsExpected(
            bool? complete,
            string expected)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            mockClientApplication.Setup(c => c.NativeMobileThirdPartyComplete())
                .Returns(complete);
            var NativeMobileModel = new NativeMobileModel
            {
                ClientApplication = mockClientApplication.Object,
            };

            NativeMobileModel.ThirdPartyStatus.Should().Be(expected);
            mockClientApplication.Verify(c => c.NativeMobileThirdPartyComplete());
        }
    }
}
