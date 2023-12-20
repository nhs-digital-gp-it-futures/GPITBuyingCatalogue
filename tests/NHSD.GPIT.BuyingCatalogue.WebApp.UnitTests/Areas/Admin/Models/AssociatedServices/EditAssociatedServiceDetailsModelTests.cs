using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.AssociatedServices
{
    public static class EditAssociatedServiceDetailsModelTests
    {

        [Theory]
        [InlineAutoData(ProvisioningType.Declarative, CataloguePriceQuantityCalculationType.PerServiceRecipient, CataloguePriceCalculationType.Volume, true, CataloguePriceType.Flat, true)]
        [InlineAutoData(ProvisioningType.Patient, CataloguePriceQuantityCalculationType.PerServiceRecipient, CataloguePriceCalculationType.Volume, false, CataloguePriceType.Tiered, false)]
        [InlineAutoData(ProvisioningType.Declarative, CataloguePriceQuantityCalculationType.PerSolutionOrService, CataloguePriceCalculationType.Volume, false, CataloguePriceType.Flat, true)]
        [InlineAutoData(ProvisioningType.Declarative, CataloguePriceQuantityCalculationType.PerServiceRecipient, CataloguePriceCalculationType.Cumulative, false, CataloguePriceType.Flat, true)]
        [InlineAutoData(ProvisioningType.OnDemand, CataloguePriceQuantityCalculationType.PerSolutionOrService, CataloguePriceCalculationType.Cumulative, false, CataloguePriceType.Tiered, false)]
        public static void CreateModel_VariedProvisioningAndCalculationTypes(
            ProvisioningType provisioningType,
            CataloguePriceQuantityCalculationType cataloguePriceQuantityCalculationType,
            CataloguePriceCalculationType cataloguePriceCalculationType,
            bool expectedHaveCorrectProvisioningAndCalculationTypes,
            CataloguePriceType cataloguePriceType,
            bool expectedNotHaveTieredPrices,
            int supplierId,
            string supplierName,
            List<SolutionMergerAndSplitTypesModel> solutionMergerAndSplitTypes)
        {
            var associatedServiceItem = new CatalogueItem()
            {
                Id = new CatalogueItemId(1, "Test"),
                Name = "Test",
                AssociatedService = new AssociatedService()
                {
                    Description = "Test",
                    OrderGuidance = "Test",
                    PracticeReorganisationType = PracticeReorganisationTypeEnum.None,
                },
            };

            var cataloguePrice = new CataloguePrice();
            cataloguePrice.ProvisioningType = provisioningType;
            cataloguePrice.CataloguePriceQuantityCalculationType = cataloguePriceQuantityCalculationType;
            cataloguePrice.CataloguePriceCalculationType = cataloguePriceCalculationType;
            cataloguePrice.CataloguePriceType = cataloguePriceType;
            associatedServiceItem.CataloguePrices = new List<CataloguePrice>() { cataloguePrice };
            var model = new EditAssociatedServiceDetailsModel(supplierId, supplierName, associatedServiceItem, solutionMergerAndSplitTypes);

            model.HaveCorrectProvisioningAndCalculationTypes.Should().Be(expectedHaveCorrectProvisioningAndCalculationTypes);
            model.NotHaveTieredPrices.Should().Be(expectedNotHaveTieredPrices);
        }
    }
}
