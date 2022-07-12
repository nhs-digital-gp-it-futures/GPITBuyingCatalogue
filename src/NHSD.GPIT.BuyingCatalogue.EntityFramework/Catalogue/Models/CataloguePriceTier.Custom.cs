namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed partial class CataloguePriceTier
    {
        public string GetRangeDescription()
        {
            var rangeContent = LowerRange.ToString("n0");

            rangeContent += (UpperRange is not null) ? $" to {UpperRange!.Value:n0}" : "+";

            rangeContent += $" {CataloguePrice.PricingUnit.RangeDescription}";

            return rangeContent;
        }
    }
}
