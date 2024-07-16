using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.Requirement;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.Requirement
{
    public static class RequirementDetailsModelTests
    {
        [Fact]
        public static void NullAssociatedServices_PropertiesCorrectlySet()
        {
            var model = new RequirementDetailsModel();

            model.AssociatedServices.Should().BeEquivalentTo(Enumerable.Empty<OrderItem>());
        }

        [Theory]
        [MockAutoData]
        public static void WithValidArguments_Add_PropertiesCorrectlySet(
            IEnumerable<OrderItem> associatedServices,
            CallOffId callOffId,
            string internalOrgId)
        {
            var model = new RequirementDetailsModel(callOffId, internalOrgId, associatedServices);

            model.CallOffId.Should().Be(callOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.AssociatedServices.Should().BeEquivalentTo(associatedServices);
            model.IsEdit.Should().BeFalse();
            model.Advice.Should().Be("Add Associated Service requirement.");
        }

        [Theory]
        [MockAutoData]
        public static void WithValidArguments_Edit_PropertiesCorrectlySet(
            IEnumerable<OrderItem> associatedServices,
            CallOffId callOffId,
            string internalOrgId,
            EntityFramework.Ordering.Models.Requirement item)
        {
            item.OrderItem = new OrderItem();
            var model = new RequirementDetailsModel(item, callOffId, internalOrgId, associatedServices);

            model.Details.Should().Be(item.Details);
            model.SelectedOrderItemId.Should().Be(item.OrderItem.CatalogueItemId);
            model.IsEdit.Should().BeTrue();
            model.Advice.Should().Be("Edit Associated Service requirement.");
        }
    }
}
