using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class PriceTiersCapSettings
    {
        public const string SectionName = "priceTiersCap";

        public int MaximumNumberOfPriceTiers { get; set; }
    }
}
