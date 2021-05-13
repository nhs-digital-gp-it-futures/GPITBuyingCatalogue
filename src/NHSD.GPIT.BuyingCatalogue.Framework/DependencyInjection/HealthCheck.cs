using System;

namespace NHSD.GPIT.BuyingCatalogue.Framework.DependencyInjection
{
    /// <summary>
    /// Defines static values used to set up the SMTP health check.
    /// </summary>
    public static class HealthCheck
    {
        /// <summary>
        /// The name for the SMTP health check.
        /// </summary>
        public const string Name = "smtp";

        /// <summary>
        /// The default tags for the SMTP health check, currently just "ready".
        /// </summary>
        public static readonly string[] DefaultTags = { "ready" };

        /// <summary>
        /// The default timeout value for the SMTP health check, currently 10 seconds.
        /// </summary>
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);
    }
}
