using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.HostingType
{
    public static class OnPremiseModelTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [InlineData(null)]
        public static void IsComplete_OnPremiseNotNull_ReturnsIsValid(bool? expected)
        {
            var mockOnPremise = new Mock<OnPremise>();
            mockOnPremise.Setup(h => h.IsValid())
                .Returns(expected);
            var model = new OnPremiseModel { OnPremise = mockOnPremise.Object };

            var actual = model.IsComplete;

            mockOnPremise.Verify(h => h.IsValid());
            actual.Should().Be(expected);
        }

        [Fact]
        public static void IsComplete_OnPremiseIsNull_ReturnsNull()
        {
            var model = new OnPremiseModel();
            model.OnPremise.Should().BeNull();

            var actual = model.IsComplete;

            actual.Should().BeNull();
        }

        [Theory(Skip = "AutoFixture not populating field correctly")]
        [CommonAutoData]
        public static void Get_RequiresHscnChecked_OnPremiseHasValidRequiresHscn_ReturnsTrue(OnPremiseModel model)
        {
            // TODO: Figure out why the RequiresHscn doesn't populate
            model.OnPremise.RequiresHscn.Should().NotBeNullOrWhiteSpace();

            var actual = model.RequiresHscnChecked;

            actual.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void Get_RequiresHscnChecked_OnPremiseHasInvalidRequiresHscn_ReturnsFalse(
            string requiresHscn)
        {
            var model = new OnPremiseModel { OnPremise = new OnPremise { RequiresHscn = requiresHscn } };

            var actual = model.RequiresHscnChecked;

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void Set_RequiresHscnChecked_TrueInput_SetsExpectedValueOnOnPremiseRequiresHscn(
            OnPremiseModel model)
        {
            model.RequiresHscnChecked = true;

            model.OnPremise.RequiresHscn.Should().Be("End user devices must be connected to HSCN/N3");
        }

        [Theory]
        [CommonAutoData]
        public static void Set_RequiresHscnChecked_FalseInput_SetsNullOnOnPremiseRequiresHscn(
            OnPremiseModel model)
        {
            model.RequiresHscnChecked = false;

            model.OnPremise.RequiresHscn.Should().BeNull();
        }
    }
}
