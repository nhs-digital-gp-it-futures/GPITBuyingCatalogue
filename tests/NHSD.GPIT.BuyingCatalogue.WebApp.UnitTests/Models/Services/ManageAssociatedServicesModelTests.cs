using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
        string catalogueItemName,
        List<OrderSublocationRecipient> recipients,
        CallOffId callOffId,
        string internalOrgId,
        CatalogueItemId additionalServiceId,
        bool unselectedAssociatedServicesAvailable)
    {
        var model = new ManageAssociatedServicesModel(associatedServices, catalogueItemName, recipients, callOffId, internalOrgId, additionalServiceId)
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
    }

    [Theory]
    [MockAutoData]
    public static void Should_Build_AssociatedServiceOrderItemModel(
        List<OrderItem> associatedServices,
        OrderItem associatedServiceOrderItem,
        string catalogueItemName,
        List<OrderSublocationRecipient> recipients,
        CallOffId callOffId,
        string internalOrgId,
        CatalogueItemId additionalServiceId,
        bool unselectedAssociatedServicesAvailable)
    {
        var model = new ManageAssociatedServicesModel(associatedServices, catalogueItemName, recipients, callOffId, internalOrgId, additionalServiceId)
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
        };

        var result = model.BuildAssociatedServiceOrderItemModel(associatedServiceOrderItem);

        result.Should().BeEquivalentTo(expected);
    }
}
