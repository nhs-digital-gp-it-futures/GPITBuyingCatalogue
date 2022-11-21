using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class AccountManagementSettings
    {
        public const string SectionName = "accountManagement";

        public int MaximumNumberOfAccountManagers { get; set; }
    }
}
