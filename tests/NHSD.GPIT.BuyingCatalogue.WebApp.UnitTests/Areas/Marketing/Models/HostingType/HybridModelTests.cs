using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.HostingType
{
    public static class HybridModelTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [InlineData(null)]
        public static void IsComplete_HybridHostingTypeNotNull_ReturnsIsValid(bool? expected)
        {
            var mockHybridHostingType = new Mock<HybridHostingType>();
            mockHybridHostingType.Setup(h => h.IsValid())
                .Returns(expected);
            var model = new HybridModel { HybridHostingType = mockHybridHostingType.Object };

            var actual = model.IsComplete;

            mockHybridHostingType.Verify(h => h.IsValid());
            actual.Should().Be(expected);
        }

        [Fact]
        public static void IsComplete_HybridHostingTypeIsNull_ReturnsNull()
        {
            var model = new HybridModel();
            model.HybridHostingType.Should().BeNull();

            var actual = model.IsComplete;

            actual.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Get_RequiresHscnChecked_HybridHostingTypeHasValidRequiresHscn_ReturnsTrue(HybridModel model)
        {
            model.HybridHostingType.RequiresHscn.Should().NotBeNullOrWhiteSpace();

            var actual = model.RequiresHscnChecked;

            actual.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void Get_RequiresHscnChecked_HybridHostingTypeHasInvalidRequiresHscn_ReturnsFalse(
            string requiresHscn)
        {
            var model = new HybridModel { HybridHostingType = new HybridHostingType { RequiresHscn = requiresHscn } };

            var actual = model.RequiresHscnChecked;

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void Set_RequiresHscnChecked_TrueInput_SetsExpectedValueOnHybridHostingTypeRequiresHscn(
            HybridModel model)
        {
            model.RequiresHscnChecked = true;

            model.HybridHostingType.RequiresHscn.Should().Be("End user devices must be connected to HSCN/N3");
        }

        [Theory]
        [CommonAutoData]
        public static void Set_RequiresHscnChecked_FalseInput_SetsNullOnHybridHostingTypeRequiresHscn(
            HybridModel model)
        {
            model.RequiresHscnChecked = false;

            model.HybridHostingType.RequiresHscn.Should().BeNull();
        }
    }
}
