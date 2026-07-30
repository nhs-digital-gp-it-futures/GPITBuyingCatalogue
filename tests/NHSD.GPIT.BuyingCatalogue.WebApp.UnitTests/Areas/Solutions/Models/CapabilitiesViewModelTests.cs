using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class CapabilitiesViewModelTests
    {
        [Fact]
        public static void Class_Inherits_SolutionDisplayBaseModel()
        {
            typeof(CapabilitiesViewModel)
                .Should()
                .BeAssignableTo<SolutionDisplayBaseModel>();
        }

        [Fact]
        public static void Class_Inherits_INoNavModel()
        {
            typeof(CapabilitiesViewModel)
                .Should()
                .BeAssignableTo<INoNavModel>();
        }

        [Theory]
        [MockAutoData]
        public static void Rows_ShouldFilter_Expired_SolutionCapabilities(
            CatalogueItem solution,
            CatalogueItemContentStatus contentStatus,
            Solution solutionItem)
        {
            SetUp(solution, solutionItem, out CatalogueItemCapability firstCapabilityItem, out CatalogueItemCapability secondCapabilityItem);

            var model = new CapabilitiesViewModel(solution, contentStatus);

            model.RowViewModels.Should().HaveCount(1);
            model.RowViewModels.ElementAt(0).Should().BeEquivalentTo(new RowViewModel(firstCapabilityItem));
        }

        [Theory]
        [MockAutoData]
        public static void Rows_ShouldFilter_Expired_AdditionalServiceCapabilities(
            CatalogueItem additionalService,
            CatalogueItem solution,
            CatalogueItemContentStatus contentStatus,
            Solution solutionItem)
        {
            solution.Solution = solutionItem;

            SetUp(additionalService, solutionItem, out CatalogueItemCapability firstCapabilityItem, out CatalogueItemCapability secondCapabilityItem);

            var model = new CapabilitiesViewModel(solution, additionalService, contentStatus);

            model.RowViewModels.Should().HaveCount(1);
            model.RowViewModels.ElementAt(0).Should().BeEquivalentTo(new RowViewModel(firstCapabilityItem));
        }

        private static void SetUp(
            CatalogueItem solution,
            Solution solutionItem,
            out CatalogueItemCapability firstCapabilityItem,
            out CatalogueItemCapability secondCapabilityItem)
        {
            solution.Solution = solutionItem;

            firstCapabilityItem = solution.CatalogueItemCapabilities.First();
            secondCapabilityItem = solution.CatalogueItemCapabilities.ElementAt(1);
            var effectiveCapability = new Capability { Status = CapabilityStatus.Effective };
            var expiredCapability = new Capability { Status = CapabilityStatus.Expired };

            firstCapabilityItem.Capability = effectiveCapability;
            secondCapabilityItem.Capability = expiredCapability;

            solution.CatalogueItemCapabilities =
                new List<CatalogueItemCapability> { firstCapabilityItem, secondCapabilityItem };
        }
    }
}
