using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    [ExcludeFromCodeCoverage]
    public sealed class CookieExpirationSettings
    {
        public DateTime? BuyingCatalogueCookiePolicyDate { get; set; }

        public TimeSpan ConsentExpiration { get; set; }

        public TimeSpan ExpireTimeSpan { get; set; }

        public bool SlidingExpiration { get; set; }
    }
}
