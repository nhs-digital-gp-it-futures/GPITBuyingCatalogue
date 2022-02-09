using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    /// <summary>
    /// Password reset settings.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class PasswordResetSettings
    {
        public string EmailTemplateId { get; set; }
    }
}
