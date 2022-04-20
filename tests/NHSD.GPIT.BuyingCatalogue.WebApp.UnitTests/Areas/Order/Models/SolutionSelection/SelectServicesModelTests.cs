using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection
{
    public static class SelectServicesModelTests
    {
        [Theory]
        [CommonInlineAutoData(CatalogueItemType.AdditionalService)]
        [CommonInlineAutoData(CatalogueItemType.AssociatedService)]
        public static void WithValidArguments_PropertiesCorrectlySet(
            CatalogueItemType catalogueItemType,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services)
        {
            var existingItem = order.OrderItems.First();

            existingItem.CatalogueItem.Id = services.First().Id;
            existingItem.CatalogueItem.CatalogueItemType = catalogueItemType;

            var model = new SelectServicesModel(order, services, catalogueItemType);

            model.ExistingServices.Should().ContainSingle(existingItem.CatalogueItem.Name);

            model.Services.Count.Should().Be(2);
            model.Services.Should().Contain(x => x.CatalogueItemId == services[1].Id && x.Description == services[1].Name);
            model.Services.Should().Contain(x => x.CatalogueItemId == services[2].Id && x.Description == services[2].Name);
        }

        [Theory]
        [CommonAutoData]
        public static void WithInvalidCatalogueItemType_ThrowsException(
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services)
        {
            FluentActions
                .Invoking(() => new SelectServicesModel(order, services, CatalogueItemType.Solution))
                .Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
