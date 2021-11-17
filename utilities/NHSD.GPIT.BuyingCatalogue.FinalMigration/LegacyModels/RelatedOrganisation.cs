using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    public partial class RelatedOrganisation
    {
        public Guid OrganisationId { get; set; }
        public Guid RelatedOrganisationId { get; set; }
    }
}
