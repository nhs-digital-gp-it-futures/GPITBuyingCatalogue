using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.AssociatedServices
{
    public static class EditAssociatedServiceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void EditAssociatedServices_ValidCatalogueItem_NoRelatedServices_PropertiesSetAsExpected(
            CatalogueItem solution,
            CatalogueItem associatedService)
        {
            var actual = new EditAssociatedServiceModel(solution, associatedService);

            actual.Solution.Should().Be(solution);
            actual.AssociatedService.Should().Be(associatedService);
            actual.BackLinkText.Should().Be("Go back");
            actual.BackLink.Should().Be($"/admin/catalogue-solutions/manage/{solution.Id}/associated-services");
        }
    }
}
