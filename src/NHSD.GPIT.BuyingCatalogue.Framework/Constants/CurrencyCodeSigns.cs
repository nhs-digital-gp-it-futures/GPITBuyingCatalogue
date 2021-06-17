using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
