using AutoFixture;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AdditionalServices
{
    public static class EditAdditionalServiceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            CreateOrderItemModel state)
        {
            var model = new EditAdditionalServiceModel(odsCode, state);

            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"{state.CatalogueItemName} information for {state.CallOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.OrderItem.Should().Be(state);

            // TODO: ServiceRecipients
        }

        [Theory]
        [CommonAutoData]
        public static void WhenEditingExistingSolution_BackLinkCorrectlySet(
            string odsCode,
            CreateOrderItemModel state)
        {
            state.IsNewSolution = false;

            var model = new EditAdditionalServiceModel(odsCode, state);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{state.CallOffId}/additional-services");
        }

        [Theory]
        [InlineData(ProvisioningType.Declarative, "/order/organisation/{0}/order/{1}/additional-services/select/additional-service/price/flat/declarative")]
        [InlineData(ProvisioningType.OnDemand, "/order/organisation/{0}/order/{1}/additional-services/select/additional-service/price/flat/ondemand")]
        [InlineData(ProvisioningType.Patient, "/order/organisation/{0}/order/{1}/additional-services/select/additional-service/price/recipients/date")]
        public static void WhenEditingNewSolution_BackLinkCorrectlySet(
            ProvisioningType provisioningType,
            string expectedBackLink)
        {
            Fixture fixture = new Fixture();

            var odsCode = fixture.Create<string>();
            fixture.Customize(new CallOffIdCustomization());
            fixture.Customize(new CatalogueItemIdCustomization());
            fixture.Customize(new IgnoreCircularReferenceCustomisation());

            var state = fixture.Create<CreateOrderItemModel>();

            state.IsNewSolution = true;
            state.CataloguePrice.ProvisioningType = provisioningType;

            var model = new EditAdditionalServiceModel(odsCode, state);

            model.BackLink.Should().Be(string.Format(expectedBackLink, odsCode, state.CallOffId));
        }
    }
}
