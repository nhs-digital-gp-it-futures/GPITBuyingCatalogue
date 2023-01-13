using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    /// <summary>
    /// Password reset settings.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class PasswordSettings
    {
        public string EmailTemplateId { get; set; }

        public int NumOfPreviousPasswords { get; set; }

        public int LockOutTimeInMinutes { get; set; }

        public int MaxAccessFailedAttempts { get; set; }

        public int PasswordExpiryDays { get; set; }

    }
}
