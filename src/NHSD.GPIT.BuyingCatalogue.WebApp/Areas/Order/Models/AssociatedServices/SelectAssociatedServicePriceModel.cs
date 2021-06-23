using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public class SelectAssociatedServicePriceModel : OrderingBaseModel
    {
        public SelectAssociatedServicePriceModel()
        {
        }

        public SelectAssociatedServicePriceModel(string odsCode, string callOffId, string solutionName, List<CataloguePrice> prices)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/associated-services/select/associated-service";
            BackLinkText = "Go back";
            Title = $"List price for {solutionName}";
            OdsCode = odsCode;
            SetPrices(prices);
        }

        public List<PriceModel> Prices { get; set; }

        public int? SelectedPrice { get; set; }

        public void SetPrices(List<CataloguePrice> prices)
        {
            Prices = new List<PriceModel>();

            foreach (var price in prices)
            {
                Prices.Add(new PriceModel
                {
                    CataloguePriceId = price.CataloguePriceId,
                    Description = $"{CurrencyCodeSigns.Code[price.CurrencyCode]}{price.Price} {price.PricingUnit?.Description} {price.TimeUnit?.Description}",
                });
            }
        }
    }
}
