using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Services
{
    public static class SelectServicesModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidOrder_BasicPropertiesCorrectlySet(
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var existingItem = order.OrderItems.First();

            existingItem.CatalogueItem.Id = services.First().Id;
            existingItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var currentItems = order.OrderItems.Select(x => x.CatalogueItem);

            var model = new SelectServicesModel(currentItems, services)
            {
                InternalOrgId = order.OrderingParty.InternalIdentifier,
                AssociatedServicesOnly = order.AssociatedServicesOnly,
                IsAmendment = order.IsAmendment,
            };

            model.InternalOrgId.Should().Be(order.OrderingParty.InternalIdentifier);
            model.IsAmendment.Should().BeFalse();
            model.AssociatedServicesOnly.Should().Be(order.AssociatedServicesOnly);
        }

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

            var currentItems = order.OrderItems.Select(x => x.CatalogueItem);

            var model = new SelectServicesModel(currentItems, services);

            model.ExistingServices.Should().BeEmpty();
            model.Services.Count.Should().Be(services.Count);

            for (var i = 0; i < services.Count; i++)
            {
                model.Services.Should().Contain(x => x.CatalogueItemId == services[i].Id && x.Description == services[i].Name);
            }

            model.Services.First(x => x.CatalogueItemId == existingItem.CatalogueItem.Id).IsSelected.Should().BeTrue();
        }
    }
}
