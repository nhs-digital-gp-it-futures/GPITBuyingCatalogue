using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
