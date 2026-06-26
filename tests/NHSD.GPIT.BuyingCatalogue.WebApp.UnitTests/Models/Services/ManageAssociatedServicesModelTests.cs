using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.TaskList;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Services;

public class ManageAssociatedServicesModelTests
{
    [Theory]
    [MockAutoData]
    public static void Constructor_Sets_ExpectedProperties(
        List<OrderItem> associatedServices,
        OrderWrapper wrapper,
        string catalogueItemName,
        List<OrderSublocationRecipient> recipients,
        CallOffId callOffId,
        string internalOrgId,
        CatalogueItemId additionalServiceId,
        bool unselectedAssociatedServicesAvailable)
    {
        var model = new ManageAssociatedServicesModel(associatedServices, catalogueItemName, recipients, callOffId, internalOrgId, additionalServiceId, wrapper)
        {
            UnselectedAssociatedServicesAvailable = unselectedAssociatedServicesAvailable,
        };

        model.AssociatedServices.Should().BeEquivalentTo(associatedServices);
        model.CatalogueItemName.Should().Be(catalogueItemName);
        model.Recipients.Should().BeEquivalentTo(recipients);
        model.CallOffId.Should().Be(callOffId);
        model.InternalOrgId.Should().Be(internalOrgId);
        model.AdditionalServiceId.Should().Be(additionalServiceId);
        model.UnselectedAssociatedServicesAvailable.Should().Be(unselectedAssociatedServicesAvailable);
        model.IsAmendment.Should().Be(wrapper.IsAmendment);
        model.HasNewRecipients.Should().Be(wrapper.HasNewOrderRecipients);
    }

    [Theory]
    [MockAutoData]
    public static void Should_Build_AssociatedServiceOrderItemModel(
        List<OrderItem> associatedServices,
        OrderItem associatedServiceOrderItem,
        OrderWrapper wrapper,
        string catalogueItemName,
        List<OrderSublocationRecipient> recipients,
        CallOffId callOffId,
        string internalOrgId,
        CatalogueItemId additionalServiceId,
        bool unselectedAssociatedServicesAvailable)
    {
        var model = new ManageAssociatedServicesModel(associatedServices, catalogueItemName, recipients, callOffId, internalOrgId, additionalServiceId, wrapper)
        {
            UnselectedAssociatedServicesAvailable = unselectedAssociatedServicesAvailable,
        };

        associatedServiceOrderItem.CatalogueItem.CataloguePrices = new List<CataloguePrice>();

        var expected = new TaskListOrderItemModel(internalOrgId, callOffId, OrderTypeEnum.Solution, recipients, associatedServiceOrderItem)
        {
            CanBeRemoved = true,
            OrderItemId = associatedServiceOrderItem.Id,
            Source = RoutingSource.ManageAssociatedServices,
            NumberOfPrices = associatedServiceOrderItem.CatalogueItem.CataloguePrices.Count,
            PriceId = 0,
            HasNewRecipients = wrapper.HasNewOrderRecipients,
        };

        var result = model.BuildAssociatedServiceOrderItemModel(associatedServiceOrderItem);

        result.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MockAutoData]
    public static void Should_Return_Expected_PreviousAssociatedServices(
        List<OrderItem> associatedServices,
        string catalogueItemName,
        int orderNumber,
        List<OrderSublocationRecipient> recipients,
        CallOffId callOffId,
        string internalOrgId,
        Order order,
        OrderItem associatedServiceOrderItem)
    {
        var solution = order.OrderItems.First();
        var additionalService = order.OrderItems.ElementAt(1);
        var associatedService = order.OrderItems.ElementAt(2);

        order.OrderNumber = orderNumber;
        order.Revision = 1;

        additionalService.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
        additionalService.Services = [associatedService];

        var amendedOrder1 = order.Clone();
        amendedOrder1.OrderNumber = orderNumber + 1;
        amendedOrder1.Revision = 2;

        var amendedAdditionalService = additionalService.Clone();
        amendedAdditionalService.Order = amendedOrder1;
        amendedAdditionalService.Services = [associatedService, associatedServiceOrderItem];

        amendedOrder1.OrderItems = [solution, amendedAdditionalService, associatedService, associatedServiceOrderItem];

        var amendedOrder2 = order.Clone();
        amendedOrder2.OrderNumber = orderNumber + 2;
        amendedOrder2.Revision = 3;

        var orderWrapper = new OrderWrapper(amendedOrder2, [order, amendedOrder1]);

        var expected = new Dictionary<CallOffId, List<OrderItem>>
        {
            { order.CallOffId, [associatedService] },
            { amendedOrder1.CallOffId, [associatedService, associatedServiceOrderItem] },
        };

        var model = new ManageAssociatedServicesModel(
            associatedServices,
            catalogueItemName,
            recipients,
            callOffId,
            internalOrgId,
            additionalService.CatalogueItemId,
            orderWrapper);

        model.PreviousAssociatedServices.Should().BeEquivalentTo(expected);
    }
}
