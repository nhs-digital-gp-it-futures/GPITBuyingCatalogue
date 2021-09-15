using System;
using System.Globalization;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed partial class CataloguePrice
    {
        private static readonly RegionInfo[] RegionInfoSet = CultureInfo.GetCultures(CultureTypes.AllCultures)
                   .Where(culture => culture.Name.Length > 0 && !culture.IsNeutralCulture)
                   .Select(culture => new RegionInfo(culture.LCID))
               .ToArray();

        public string GetCurrencySymbol()
        {
            var regionInfo = RegionInfoSet
                .Where(region => string.Equals(region.ISOCurrencySymbol, CurrencyCode, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            return regionInfo?.CurrencySymbol;
        }
    }
}
