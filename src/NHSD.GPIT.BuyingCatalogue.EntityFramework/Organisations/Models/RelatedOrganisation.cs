using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models
{
    public sealed class RelatedOrganisation
    {
        public Guid OrganisationId { get; set; }

        public Guid RelatedOrganisationId { get; set; }

        public Organisation Organisation { get; set; }

        public Organisation RelatedOrganisationNavigation { get; set; }
    }
}
