﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.AssociatedServices
{
    public static class AddAssociatedServiceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void AddAssociatedService_ValidCatalogueItem_NoRelatedServices_PropertiesSetAsExpected(
            CatalogueItem catalogueItem)
        {
            var actual = new AddAssociatedServiceModel(catalogueItem);

            actual.SolutionId.Should().Be(catalogueItem.Id);
            actual.SupplierName.Should().Be(catalogueItem.Supplier.Name);
        }
    }
}
