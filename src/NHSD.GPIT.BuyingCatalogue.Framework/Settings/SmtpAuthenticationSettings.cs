namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    /// <summary>
    /// SMTP authentication settings.
    /// </summary>
    public sealed class SmtpAuthenticationSettings
    {
        /// <summary>
        /// Gets a value indicating whether the SMTP server requires authentication.
        /// </summary>
        public bool IsRequired { get; init; }

        /// <summary>
        /// Gets the user name to authenticate with.
        /// </summary>
        public string? UserName { get; init; }

        /// <summary>
        /// Gets the password for authentication.
        /// </summary>
        public string? Password { get; init; }
    }
}
