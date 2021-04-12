using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    [ExcludeFromCodeCoverage]
    public sealed class CookieExpirationSettings
    {
        public TimeSpan ExpireTimeSpan { get; set; }

        public bool SlidingExpiration { get; set; }
    }
}
