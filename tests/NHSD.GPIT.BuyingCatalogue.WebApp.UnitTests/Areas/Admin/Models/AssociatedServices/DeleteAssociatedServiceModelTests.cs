using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.AssociatedServices
{
    public static class DeleteAssociatedServiceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Constructor_NullAssociatedService_ThrowsException(
            CatalogueItemId catalogueItemId)
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new DeleteAssociatedServiceModel(catalogueItemId, null));

            actual.ParamName.Should().Be("associatedService");
        }

        [Theory]
        [CommonAutoData]
        public static void DeleteAssociatedService_ValidArguments_PropertiesSetAsExpected(
            CatalogueItemId catalogueItemId,
            CatalogueItem associatedService)
        {
            var actual = new DeleteAssociatedServiceModel(catalogueItemId, associatedService);

            actual.AssociatedService.Should().Be(associatedService);
            actual.BackLinkText.Should().Be("Go back");
            actual.BackLink.Should().Be($"/admin/catalogue-solutions/manage/{catalogueItemId}/associated-services/{associatedService.Id}/edit-associated-service");
        }
    }
}
