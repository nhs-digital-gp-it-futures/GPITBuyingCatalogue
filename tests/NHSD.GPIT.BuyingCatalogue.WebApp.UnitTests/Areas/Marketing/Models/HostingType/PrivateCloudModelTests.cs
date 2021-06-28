using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.HostingType

{
    public static class PrivateCloudModelTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [InlineData(null)]
        public static void IsComplete_PrivateCloudNotNull_ReturnsIsValid(bool? expected)
        {
            var mockPrivateCloud = new Mock<PrivateCloud>();
            mockPrivateCloud.Setup(h => h.IsValid())
                .Returns(expected);
            var model = new PrivateCloudModel { PrivateCloud = mockPrivateCloud.Object };

            var actual = model.IsComplete;

            mockPrivateCloud.Verify(h => h.IsValid());
            actual.Should().Be(expected);
        }

        [Fact]
        public static void IsComplete_PrivateCloudIsNull_ReturnsNull()
        {
            var model = new PrivateCloudModel();
            model.PrivateCloud.Should().BeNull();

            var actual = model.IsComplete;

            actual.Should().BeNull();
        }

        // TODO: fix
        [Theory(Skip = "Broken")]
        [CommonAutoData]
        public static void Get_RequiresHscnChecked_PrivateCloudHasValidRequiresHscn_ReturnsTrue(PrivateCloudModel model)
        {
            model.PrivateCloud.RequiresHscn.Should().NotBeNullOrWhiteSpace();

            var actual = model.RequiresHscnChecked;

            actual.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void Get_RequiresHscnChecked_PrivateCloudHasInvalidRequiresHscn_ReturnsFalse(
            string requiresHscn)
        {
            var model = new PrivateCloudModel { PrivateCloud = new PrivateCloud { RequiresHscn = requiresHscn } };

            var actual = model.RequiresHscnChecked;

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void Set_RequiresHscnChecked_TrueInput_SetsExpectedValueOnPrivateCloudRequiresHscn(
            PrivateCloudModel model)
        {
            model.RequiresHscnChecked = true;

            model.PrivateCloud.RequiresHscn.Should().Be("End user devices must be connected to HSCN/N3");
        }

        [Theory]
        [CommonAutoData]
        public static void Set_RequiresHscnChecked_FalseInput_SetsNullOnPrivateCloudRequiresHscn(
            PrivateCloudModel model)
        {
            model.RequiresHscnChecked = false;

            model.PrivateCloud.RequiresHscn.Should().BeNull();
        }
    }
}
