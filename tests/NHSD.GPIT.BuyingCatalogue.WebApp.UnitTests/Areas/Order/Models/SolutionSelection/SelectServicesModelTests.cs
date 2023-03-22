using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MoreLinq;
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
        [CommonAutoData]
        public static void WithInvalidCatalogueItemType_ThrowsException(
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services)
        {
            FluentActions
                .Invoking(() => new SelectServicesModel(new OrderWrapper(order), services, CatalogueItemType.Solution))
                .Should().Throw<ArgumentOutOfRangeException>();
        }

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

            var model = new SelectServicesModel(new OrderWrapper(order), services, CatalogueItemType.AdditionalService)
            {
                InternalOrgId = order.OrderingParty.InternalIdentifier,
                CallOffId = order.CallOffId,
                AssociatedServicesOnly = order.AssociatedServicesOnly,
            };

            model.InternalOrgId.Should().Be(order.OrderingParty.InternalIdentifier);
            model.CallOffId.Should().Be(order.CallOffId);
            model.IsAmendment.Should().BeFalse();
            model.AssociatedServicesOnly.Should().Be(order.AssociatedServicesOnly);
            model.SolutionName.Should().Be(existingItem.CatalogueItem.Name);
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_ForSolution_SolutionNameCorrectlySet(
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services)
        {
            var existingItem = order.OrderItems.First();
            var wrapper = new OrderWrapper(order);

            existingItem.CatalogueItem.Id = services.First().Id;
            existingItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            wrapper.Order.AssociatedServicesOnly = false;

            var model = new SelectServicesModel(wrapper, services, CatalogueItemType.AdditionalService);

            model.SolutionName.Should().Be(wrapper.RolledUp.GetSolution()?.CatalogueItem.Name);
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_ForAssociatedService_AssociatedServiceNameCorrectlySet(
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services)
        {
            var existingItem = order.OrderItems.First();
            var wrapper = new OrderWrapper(order);

            existingItem.CatalogueItem.Id = services.First().Id;
            existingItem.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            wrapper.Order.AssociatedServicesOnly = true;

            var model = new SelectServicesModel(wrapper, services, CatalogueItemType.AssociatedService);

            model.SolutionName.Should().Be(wrapper.RolledUp.Solution.Name);
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

            var model = new SelectServicesModel(new OrderWrapper(order), services, catalogueItemType);

            model.IsAmendment.Should().BeFalse();
            model.ExistingServices.Should().BeEmpty();
            model.Services.Count.Should().Be(services.Count);

            for (var i = 0; i < services.Count; i++)
            {
                model.Services.Should().Contain(x => x.CatalogueItemId == services[i].Id && x.Description == services[i].Name);
            }

            model.Services.First(x => x.CatalogueItemId == existingItem.CatalogueItem.Id).IsSelected.Should().BeTrue();
        }

        [Theory]
        [CommonInlineAutoData(CatalogueItemType.AdditionalService)]
        [CommonInlineAutoData(CatalogueItemType.AssociatedService)]
        public static void WithValidArguments_ForAmendment_PropertiesCorrectlySet(
            CatalogueItemType catalogueItemType,
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Ordering.Models.Order amendment,
            List<CatalogueItem> services)
        {
            order.OrderItems.ForEach(x => x.OrderItemPrice = null);
            amendment.OrderItems.ForEach(x => x.OrderItemPrice = null);

            order.Revision = 1;
            amendment.OrderNumber = order.OrderNumber;
            amendment.Revision = 2;

            var existingItem = order.OrderItems.First();

            existingItem.CatalogueItem.Id = services.First().Id;
            existingItem.CatalogueItem.CatalogueItemType = catalogueItemType;

            var newItem = amendment.OrderItems.First();

            newItem.CatalogueItem.Id = services.ElementAt(1).Id;
            newItem.CatalogueItem.CatalogueItemType = catalogueItemType;

            var model = new SelectServicesModel(new OrderWrapper(new[] { order, amendment }), services, catalogueItemType);

            model.IsAmendment.Should().BeTrue();
            model.ExistingServices.Should().BeEquivalentTo(existingItem.CatalogueItem.Name);
            model.Services.Count.Should().Be(2);

            for (var i = 1; i < services.Count; i++)
            {
                model.Services.Should().Contain(x => x.CatalogueItemId == services[i].Id && x.Description == services[i].Name);
            }

            model.Services.First(x => x.CatalogueItemId == newItem.CatalogueItem.Id).IsSelected.Should().BeTrue();
        }
    }
}
