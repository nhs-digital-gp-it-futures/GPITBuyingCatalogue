using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class SolutionCheckEpicsModelTests
    {
        [Fact]
        public static void Class_Inherits_INoNavModel()
        {
            typeof(SolutionCheckEpicsModel)
                .Should()
                .BeAssignableTo<INoNavModel>();
        }

        [Theory]
        [CommonAutoData]
        public static void NhsDefined_NhsDefinedArrayNotEmpty_ReturnsTrue(SolutionCheckEpicsModel model)
        {
            model.NhsDefined.Should().NotBeEmpty();

            model.HasNhsDefined().Should().BeTrue();
        }

        [Fact]
        public static void NhsDefined_NhsDefinedArrayEmpty_ReturnsFalse()
        {
            var model = new SolutionCheckEpicsModel { NhsDefined = System.Array.Empty<string>() };

            model.HasNhsDefined().Should().BeFalse();
        }

        [Fact]
        public static void NhsDefined_NhsDefinedArrayIsNull_ReturnsFalse()
        {
            var model = new SolutionCheckEpicsModel { NhsDefined = null };

            model.HasNhsDefined().Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void SupplierDefined_SupplierDefinedArrayNotEmpty_ReturnsTrue(SolutionCheckEpicsModel model)
        {
            model.SupplierDefined.Should().NotBeEmpty();

            model.HasSupplierDefined().Should().BeTrue();
        }

        [Fact]
        public static void SupplierDefined_SupplierDefinedArrayEmpty_ReturnsFalse()
        {
            var model = new SolutionCheckEpicsModel { SupplierDefined = System.Array.Empty<string>() };

            model.HasSupplierDefined().Should().BeFalse();
        }

        [Fact]
        public static void SupplierDefined_SupplierDefinedArrayIsNull_ReturnsFalse()
        {
            var model = new SolutionCheckEpicsModel { SupplierDefined = null };

            model.HasSupplierDefined().Should().BeFalse();
        }

        [Fact]
        public static void HasNoEpics_NoNhsOrSupplierDefinedEpics_ReturnsTrue()
        {
            var model = new Mock<SolutionCheckEpicsModel> { CallBase = true };
            model.Setup(m => m.HasNhsDefined())
                .Returns(false);
            model.Setup(m => m.HasSupplierDefined())
                .Returns(false);

            model.Object.HasNoEpics().Should().BeTrue();

            model.Verify(m => m.HasNhsDefined());
            model.Verify(m => m.HasSupplierDefined());
        }

        [Fact]
        public static void HasNoEpics_HasNhsDefinedEpicsOnly_ReturnsFalse()
        {
            var model = new Mock<SolutionCheckEpicsModel> { CallBase = true };
            model.Setup(m => m.HasNhsDefined())
                .Returns(true);
            model.Setup(m => m.HasSupplierDefined())
                .Returns(false);

            model.Object.HasNoEpics().Should().BeFalse();
        }

        [Fact]
        public static void HasNoEpics_HasSupplierDefinedEpicsOnly_ReturnsFalse()
        {
            var model = new Mock<SolutionCheckEpicsModel> { CallBase = true };
            model.Setup(m => m.HasNhsDefined())
                .Returns(false);
            model.Setup(m => m.HasSupplierDefined())
                .Returns(true);

            model.Object.HasNoEpics().Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void WithItems_SetsCatalogueItemIdAdditional(
            CatalogueItemId catalogueItemId,
            CatalogueItemId catalogueItemIdAdditional,
            string solutionName)
        {
            var model = new SolutionCheckEpicsModel();

            var actual = model.WithItems(catalogueItemId, catalogueItemIdAdditional, solutionName);

            model.CatalogueItemIdAdditional.Should().Be(catalogueItemIdAdditional);
        }

        [Theory]
        [CommonAutoData]
        public static void WithItems_ReturnsFromSetSolutionName(
            CatalogueItemId catalogueItemId,
            CatalogueItemId catalogueItemIdAdditional,
            string solutionName)
        {
            var model = new Mock<SolutionCheckEpicsModel> { CallBase = true };
            var expected = new Mock<SolutionCheckEpicsModel>().Object;
            model.Setup(m => m.WithSolutionName(solutionName))
                .Returns(expected);

            var actual = model.Object.WithItems(catalogueItemId, catalogueItemIdAdditional, solutionName);

            model.Verify(m => m.WithSolutionName(solutionName));
            actual.Should().Be(expected);
        }

        [Theory]
        [AutoData]
        public static void WithSolutionName_InputValid_SetsSolutionName(string solutionName)
        {
            var model = new SolutionCheckEpicsModel();

            var actual = model.WithSolutionName(solutionName);

            model.SolutionName.Should().Be(solutionName);
            actual.Should().BeEquivalentTo(new SolutionCheckEpicsModel { SolutionName = solutionName });
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void WithSolutionName_InputNotValid_SolutionNameNotChanged(string invalid)
        {
            const string expected = "some-solution-name";
            var model = new SolutionCheckEpicsModel { SolutionName = expected };

            model.WithSolutionName(invalid);

            model.SolutionName.Should().Be(expected);
        }
    }
}
