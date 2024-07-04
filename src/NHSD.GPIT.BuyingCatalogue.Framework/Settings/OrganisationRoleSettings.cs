using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class OrganisationRoleSettings
    {
        public OrganisationType OrganisationType { get; set; }

        public string PrimaryRoleId { get; set; }

        public string SecondaryRoleId { get; set; }
    }
}
