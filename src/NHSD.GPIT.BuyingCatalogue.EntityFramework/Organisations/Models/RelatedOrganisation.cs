using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models
{
    public sealed class RelatedOrganisation : IAudited
    {
        public RelatedOrganisation()
        {
        }

        public RelatedOrganisation(int organisationId, int relatedOrganisationId)
        {
            OrganisationId = organisationId;
            RelatedOrganisationId = relatedOrganisationId;
        }

        public int OrganisationId { get; set; }

        public int RelatedOrganisationId { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public Organisation Organisation { get; set; }

        public Organisation RelatedOrganisationNavigation { get; set; }
    }
}
