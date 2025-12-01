using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class SolutionDisplayBaseModelTests
    {
        [Theory]
        [InlineData(typeof(ApplicationTypesModel))]
        [InlineData(typeof(ImplementationTimescalesModel))]
        [InlineData(typeof(SolutionDescriptionModel))]
        [InlineData(typeof(SolutionFeaturesModel))]
        public static void ChildClasses_InheritFrom_SolutionDisplayBaseModel(Type childType)
        {
            childType
                .Should()
                .BeAssignableTo<SolutionDisplayBaseModel>();
        }

        [Theory]
        [MockAutoData]
        public static void Construct_PropertiesCorrectlySet(
            CatalogueItem catalogueItem,
            Solution solution,
            CatalogueItemContentStatus contentStatus,
            bool isSubPage)
        {
            catalogueItem.Solution = solution;
            var model = new SolutionDisplayStub(catalogueItem, contentStatus, isSubPage);
            model.IsSubPage.Should().Be(isSubPage);
            model.SolutionId.Should().Be(catalogueItem.Id);
            model.PublicationStatus.Should().Be(catalogueItem.PublishedStatus);
            model.IsPilotSolution.Should().Be(solution.IsPilotSolution);
            model.Sections.Any().Should().BeTrue();
            model.BreadcrumbItems.Any().Should().BeTrue();
            model.ShouldShowStandardsCategory.Should().BeFalse();
        }

        [Theory]
        [MockInlineAutoData(null, false)]
        [MockInlineAutoData(SolutionCategory.A, true)]
        [MockInlineAutoData(SolutionCategory.B, true)]
        [MockInlineAutoData(SolutionCategory.C, true)]
        public static void ShouldShowStandardsCategory_ReturnsExpected(
            SolutionCategory? category,
            bool expected,
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            solution.Category = category;

            var model = new SolutionDisplayStub(solution.CatalogueItem, contentStatus, true, false);

            model.ShouldShowStandardsCategory.Should().Be(expected);
        }

        private sealed class SolutionDisplayStub
            : SolutionDisplayBaseModel
        {
            public SolutionDisplayStub(
                CatalogueItem catalogueItem,
                CatalogueItemContentStatus contentStatus,
                bool isSubPage = false)
                : base(catalogueItem, contentStatus, isSubPage)
            {
            }

            public SolutionDisplayStub(
                CatalogueItem catalogueItem,
                CatalogueItemContentStatus contentStatus,
                bool shouldShowStandardsCategory,
                bool isSubPage = false)
                : base(catalogueItem, contentStatus, shouldShowStandardsCategory, isSubPage)
            {
            }

            public override int Index => 0;
        }
    }
}
