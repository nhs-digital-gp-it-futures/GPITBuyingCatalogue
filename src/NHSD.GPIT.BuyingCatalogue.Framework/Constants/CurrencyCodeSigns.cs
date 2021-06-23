using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Constants
{
    public static class CurrencyCodeSigns
    {
        private static readonly Dictionary<string, string> SignCode = new Dictionary<string, string>
        {
            { "GBP", "£" },
            { "USD", "$" },
            { "EUR", "€" },
        };

        public static Dictionary<string, string> Code
        {
            get { return SignCode; }
        }
    }
}
