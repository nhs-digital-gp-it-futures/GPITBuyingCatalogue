using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing.Base;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing
{
    public class ViewPriceModel : PricingModel
    {
        public const string AdviceText = "This is the price you originally agreed with the supplier and it cannot be changed.";

        public ViewPriceModel(IPrice price, CatalogueItem catalogueItem)
            : base(price, catalogueItem)
        {
        }

        public override string Advice => AdviceText;

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public string OnwardLink { get; set; }
    }
}
