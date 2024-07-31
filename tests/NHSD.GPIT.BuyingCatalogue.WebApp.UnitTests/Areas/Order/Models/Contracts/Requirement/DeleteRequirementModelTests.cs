using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.Requirement;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.Requirement
{
    public static class DeleteRequirementModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            CallOffId callOffId, string internalOrgId, EntityFramework.Ordering.Models.Requirement requirement)
        {
            var model = new DeleteRequirementModel(callOffId, internalOrgId, requirement);

            model.CallOffId.Should().Be(callOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.ItemId.Should().Be(requirement.Id);
            model.AssociatedServiceName.Should().Be(requirement.OrderItem.CatalogueItem.Name);
            model.Requirement.Should().Be(requirement.Details);
        }

        [Theory]
        [MockAutoData]
        public static void NullOrderItem_AssociatedServiceNameCorrectlySet(
            CallOffId callOffId, string internalOrgId, EntityFramework.Ordering.Models.Requirement requirement)
        {
            requirement.OrderItem = null;
            var model = new DeleteRequirementModel(callOffId, internalOrgId, requirement);

            model.AssociatedServiceName.Should().Be(null);
        }

        [Theory]
        [MockAutoData]
        public static void NullCatalogueItem_AssociatedServiceNameCorrectlySet(
            CallOffId callOffId, string internalOrgId, EntityFramework.Ordering.Models.Requirement requirement)
        {
            requirement.OrderItem.CatalogueItem = null;
            var model = new DeleteRequirementModel(callOffId, internalOrgId, requirement);

            model.AssociatedServiceName.Should().Be(null);
        }
    }
}
