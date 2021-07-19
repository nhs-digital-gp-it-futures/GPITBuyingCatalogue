using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class OdsSettings : IOdsSettings
    {
        public string ApiBaseUrl { get; set; }

        public string[] BuyerOrganisationRoleIds { get; set; }

        public string GpPracticeRoleId { get; set; }

        public int GetChildOrganisationSearchLimit { get; set; }
    }
}
