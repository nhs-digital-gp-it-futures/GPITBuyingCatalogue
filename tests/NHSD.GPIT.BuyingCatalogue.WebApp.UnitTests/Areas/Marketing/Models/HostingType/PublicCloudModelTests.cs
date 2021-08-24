using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.HostingType
{
    public static class PublicCloudModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void IsComplete_PublicCloudNotNull_ReturnsValid(
            PublicCloud publicCloud)
        {
            var model = new PublicCloudModel { PublicCloud = publicCloud };

            var actual = model.IsComplete;

            actual.Should().Be(true);
        }

        [Fact]
        public static void IsComplete_PublicCloudBlank_ReturnsInValid()
        {
            var model = new PublicCloudModel { PublicCloud = new PublicCloud() };

            var actual = model.IsComplete;

            actual.Should().Be(false);
        }

        [Fact]
        public static void IsComplete_PublicCloudIsNull_ReturnsNull()
        {
            var model = new PublicCloudModel();
            model.PublicCloud.Should().BeNull();

            var actual = model.IsComplete;

            actual.Should().BeNull();
        }

        [Theory(Skip = "AutoFixture not populating field correctly")]
        [CommonAutoData]
        public static void Get_RequiresHscnChecked_PublicCloudHasValidRequiresHscn_ReturnsTrue(PublicCloudModel model)
        {
            // TODO: Figure out why the RequiresHscn doesn't populate
            model.PublicCloud.RequiresHscn.Should().NotBeNullOrWhiteSpace();

            var actual = model.RequiresHscnChecked;

            actual.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void Get_RequiresHscnChecked_PublicCloudHasInvalidRequiresHscn_ReturnsFalse(
            string requiresHscn)
        {
            var model = new PublicCloudModel { PublicCloud = new PublicCloud { RequiresHscn = requiresHscn } };

            var actual = model.RequiresHscnChecked;

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void Set_RequiresHscnChecked_TrueInput_SetsExpectedValueOnPublicCloudRequiresHscn(
            PublicCloudModel model)
        {
            model.RequiresHscnChecked = true;

            model.PublicCloud.RequiresHscn.Should().Be("End user devices must be connected to HSCN/N3");
        }

        [Theory]
        [CommonAutoData]
        public static void Set_RequiresHscnChecked_FalseInput_SetsNullOnPublicCloudRequiresHscn(
            PublicCloudModel model)
        {
            model.RequiresHscnChecked = false;

            model.PublicCloud.RequiresHscn.Should().BeNull();
        }
    }
}
