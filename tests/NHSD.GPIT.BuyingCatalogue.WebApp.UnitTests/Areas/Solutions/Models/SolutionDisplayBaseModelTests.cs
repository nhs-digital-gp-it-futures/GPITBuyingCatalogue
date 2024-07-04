using System;
using System.Linq;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
        [CommonAutoData]
        public static void Constructor_PropertiesCorrectlySet(
        CatalogueItem catalogueItem,
        Solution solution,
        CatalogueItemContentStatus contentStatus,
        bool isSubPage)
        {
            catalogueItem.Solution = solution;
            var model = new Mock<SolutionDisplayBaseModel>(catalogueItem, contentStatus, isSubPage);
            model.Object.IsSubPage.Should().Be(isSubPage);
            model.Object.SolutionId.Should().Be(catalogueItem.Id);
            model.Object.PublicationStatus.Should().Be(catalogueItem.PublishedStatus);
            model.Object.IsPilotSolution.Should().Be(solution.IsPilotSolution);
            model.Object.Sections.Any().Should().BeTrue();
            model.Object.BreadcrumbItems.Any().Should().BeTrue();
        }
    }
}
