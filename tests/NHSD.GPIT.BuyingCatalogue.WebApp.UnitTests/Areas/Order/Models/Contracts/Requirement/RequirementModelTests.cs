using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.Requirement;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.Requirement
{
    public static class RequirementModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.ContractBilling contractBilling)
        {
            contractBilling.Requirements.Add(new EntityFramework.Ordering.Models.Requirement());

            var model = new RequirementModel(contractBilling);

            model.ContractBilling.Should().Be(contractBilling);
            model.HasRequirements.Should().BeTrue();
        }

        [Fact]
        public static void NullContractBilling_PropertiesCorrectlySet()
        {
            var model = new RequirementModel(null);

            model.HasRequirements.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void NoRequirements_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.ContractBilling contractBilling)
        {
            contractBilling.Requirements.Clear();

            var model = new RequirementModel(contractBilling);

            model.HasRequirements.Should().BeFalse();
        }
    }
}
