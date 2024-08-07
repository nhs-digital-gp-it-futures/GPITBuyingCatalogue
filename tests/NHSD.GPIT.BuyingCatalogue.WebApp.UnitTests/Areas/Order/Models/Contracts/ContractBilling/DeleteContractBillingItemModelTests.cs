using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ContractBilling;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.ContractBilling
{
    public static class DeleteContractBillingItemModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            CallOffId callOffId, string internalOrgId, ContractBillingItem item)
        {
            var model = new DeleteContractBillingItemModel(callOffId, internalOrgId, item);

            model.CallOffId.Should().Be(callOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.ItemId.Should().Be(item.Id);
            model.ItemName.Should().Be(item.Milestone.Title);
            model.ItemPaymentTrigger.Should().Be(item.Milestone.PaymentTrigger);
        }
    }
}
