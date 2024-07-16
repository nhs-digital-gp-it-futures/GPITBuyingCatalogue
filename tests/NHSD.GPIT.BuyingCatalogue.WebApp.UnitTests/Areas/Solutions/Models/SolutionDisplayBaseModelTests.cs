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
        public static void Constructor_PropertiesCorrectlySet(
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
        }

        private sealed class SolutionDisplayStub(
            CatalogueItem catalogueItem,
            CatalogueItemContentStatus contentStatus,
            bool isSubPage = false)
            : SolutionDisplayBaseModel(catalogueItem, contentStatus, isSubPage)
        {
            public override int Index => 0;
        }
    }
}
