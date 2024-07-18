using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ManageOrders
{
    public static class ViewOrderModelTests
    {
        [Theory]
        [MockAutoData]
        public static void Construct_MapsCorrectly(
            Organisation organisation,
            AspNetUser lastUpdatedBy,
            Supplier supplier,
            EntityFramework.Catalogue.Models.Framework framework,
            List<OrderItem> orderItems,
            EntityFramework.Ordering.Models.Order order)
        {
            order.LastUpdatedByUser = lastUpdatedBy;
            order.OrderingParty = organisation;
            order.Supplier = supplier;
            order.SelectedFramework = framework;
            order.OrderItems = orderItems;
            var model = new ViewOrderModel(order);

            model.CallOffId.Should().Be(order.CallOffId);
            model.Description.Should().Be(order.Description);
            model.LastUpdatedBy.Should().Be(order.LastUpdatedByUser.FullName);
            model.OrganisationName.Should().Be(order.OrderingParty.Name);
            model.OrganisationExternalIdentifier.Should().Be(order.OrderingParty.ExternalIdentifier);
            model.OrganisationInternalIdentifier.Should().Be(order.OrderingParty.InternalIdentifier);
            model.SupplierName.Should().Be(order.Supplier.Name);
            model.OrderStatus.Should().Be(order.OrderStatus);
            model.SelectedFrameworkName.Should().Be(order.SelectedFramework.ShortName);
            model.OrderItems.Should().HaveCount(orderItems.Count);
        }
    }
}
