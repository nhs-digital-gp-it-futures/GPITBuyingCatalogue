using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared;
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

            var model = new SelectServicesModel(new OrderWrapper(order), services, catalogueItemType);

            model.Services.Count.Should().Be(services.Count);

            for (var i = 0; i < services.Count; i++)
            {
                model.Services.Should().Contain(x => x.CatalogueItemId == services[i].Id && x.Description == services[i].Name);
            }

            model.Services.First(x => x.CatalogueItemId == existingItem.CatalogueItem.Id).IsSelected.Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void WithInvalidCatalogueItemType_ThrowsException(
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services)
        {
            FluentActions
                .Invoking(() => new SelectServicesModel(new OrderWrapper(order), services, CatalogueItemType.Solution))
                .Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
