﻿using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public sealed class SelectAdditionalServicePriceModel : OrderingBaseModel
    {
        public SelectAdditionalServicePriceModel()
        {
        }

        public SelectAdditionalServicePriceModel(string odsCode, CallOffId callOffId, string solutionName, List<CataloguePrice> prices)
        {
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
                    Description = $"{CurrencyCodeSigns.Code[price.CurrencyCode]}{price.Price} {price.PricingUnit?.Description} {price.TimeUnit?.Description()}",
                });
            }
        }
    }
}
