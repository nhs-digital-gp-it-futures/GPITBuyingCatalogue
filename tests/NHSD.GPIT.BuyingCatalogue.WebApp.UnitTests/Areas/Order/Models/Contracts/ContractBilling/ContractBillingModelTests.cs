using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ContractBilling;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.ContractBilling
{
    public static class ContractBillingModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.ContractBilling contractBilling)
        {
            contractBilling.ContractBillingItems.Add(new ContractBillingItem());

            var model = new ContractBillingModel(contractBilling);

            model.BespokeBilling.Should().Be(contractBilling);
            model.HasBespokeBilling.Should().BeTrue();
        }

        [Fact]
        public static void NullBespokePlan_PropertiesCorrectlySet()
        {
            var model = new ContractBillingModel(null);

            model.HasBespokeBilling.Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void NoMilestones_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.ContractBilling contractBilling)
        {
            contractBilling.ContractBillingItems.Clear();

            var model = new ContractBillingModel(contractBilling);

            model.HasBespokeBilling.Should().BeFalse();
        }
    }
}
