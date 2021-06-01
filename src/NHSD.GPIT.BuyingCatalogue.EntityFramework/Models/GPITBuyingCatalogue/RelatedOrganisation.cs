using System;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public partial class RelatedOrganisation
    {
        public Guid OrganisationId { get; set; }

        public Guid RelatedOrganisationId { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual Organisation RelatedOrganisationNavigation { get; set; }
    }
}
