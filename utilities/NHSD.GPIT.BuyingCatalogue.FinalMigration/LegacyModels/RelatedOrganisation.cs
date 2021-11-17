using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    [ExcludeFromCodeCoverage]
    public partial class RelatedOrganisation
    {
        public Guid OrganisationId { get; set; }
        public Guid RelatedOrganisationId { get; set; }
    }
}
