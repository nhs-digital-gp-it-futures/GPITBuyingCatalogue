using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity
{
    public partial class RelatedOrganisation
    {
        public Guid OrganisationId { get; set; }
        public Guid RelatedOrganisationId { get; set; }

        public virtual Organisation Organisation { get; set; }
        public virtual Organisation RelatedOrganisationNavigation { get; set; }
    }
}
