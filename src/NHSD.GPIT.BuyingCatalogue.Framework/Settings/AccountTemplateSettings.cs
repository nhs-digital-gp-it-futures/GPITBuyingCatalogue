using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    /// <summary>
    /// Registration settings.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class AccountTemplateSettings
    {
        public string AccountApprovedTemplateId { get; set; }

        public string AccountRejectedTemplateId { get; set; }

        public string AccountDuplicateTemplateId { get; set; }

        public string InvalidOdsCodeTemplateId { get; set; }

        public string AccountSubmittedTemplateId { get; set; }
    }
}
