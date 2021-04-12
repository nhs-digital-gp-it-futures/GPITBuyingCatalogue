using System;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    public sealed class CookieExpirationSettings
    {
        public TimeSpan ExpireTimeSpan { get; set; }

        public bool SlidingExpiration { get; set; }
    }
}
