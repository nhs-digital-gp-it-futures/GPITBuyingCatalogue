using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    /// <summary>
    /// Password reset settings.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class PasswordResetSettings
    {
        public string EmailTemplateId { get; set; }

        public int NumOfPreviousPasswords { get; set; }
    }
}
