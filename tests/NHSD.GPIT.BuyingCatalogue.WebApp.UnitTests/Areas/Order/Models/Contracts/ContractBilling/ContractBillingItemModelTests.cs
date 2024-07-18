using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ContractBilling;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.ContractBilling
{
    public static class ContractBillingItemModelTests
    {
        [Fact]
        public static void NullAssociatedServices_PropertiesCorrectlySet()
        {
            var model = new ContractBillingItemModel();

            model.AssociatedServices.Should().BeEquivalentTo(Enumerable.Empty<OrderItem>());
        }

        [Theory]
        [MockAutoData]
        public static void WithValidArguments_Add_PropertiesCorrectlySet(
            IEnumerable<OrderItem> associatedServices,
            CallOffId callOffId,
            string internalOrgId)
        {
            var model = new ContractBillingItemModel(callOffId, internalOrgId, associatedServices);

            model.CallOffId.Should().Be(callOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.AssociatedServices.Should().BeEquivalentTo(associatedServices);
            model.IsEdit.Should().BeFalse();
            model.Advice.Should().Be("Add an Associated Service milestone.");
        }

        [Theory]
        [MockAutoData]
        public static void WithValidArguments_Edit_PropertiesCorrectlySet(
            IEnumerable<OrderItem> associatedServices,
            CallOffId callOffId,
            string internalOrgId,
            ContractBillingItem item)
        {
            item.OrderItem = new OrderItem();
            var model = new ContractBillingItemModel(item, callOffId, internalOrgId, associatedServices);

            model.Name.Should().Be(item.Milestone?.Title);
            model.PaymentTrigger.Should().Be(item.Milestone?.PaymentTrigger);
            model.SelectedOrderItemId.Should().Be(item.OrderItem.CatalogueItemId);
            model.Quantity.Should().Be(item.Quantity);
            model.IsEdit.Should().BeTrue();
            model.Advice.Should().Be("Edit this Associated Service milestone.");
        }
    }
}
