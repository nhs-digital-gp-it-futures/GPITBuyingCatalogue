using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public class SelectSolutionPriceModel : OrderingBaseModel
    {
        public SelectSolutionPriceModel()
        {
        }

        public SelectSolutionPriceModel(string odsCode, string callOffId, string solutionName, List<CataloguePrice> prices)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution";
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
                    Description = $"£{price.Price} {price.PricingUnit?.Description} {price.TimeUnit?.Description}",
                });
            }
        }
    }
}
