using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models
{
    [ExcludeFromCodeCoverage]
    public class TemplateOptions
    {
        public string ContractExpiryTemplateId { get; set; } = string.Empty;

        public string PasswordExpiryTemplateId { get; set; } = string.Empty;
    }
}
