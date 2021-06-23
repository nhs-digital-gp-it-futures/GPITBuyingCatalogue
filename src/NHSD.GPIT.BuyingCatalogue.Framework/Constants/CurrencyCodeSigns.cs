using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Constants
{
    public static class CurrencyCodeSigns
    {
        public static Dictionary<string, string> Code { get; } = new()
        {
            { "GBP", "£" },
            { "USD", "$" },
            { "EUR", "€" },
        };
    }
}
