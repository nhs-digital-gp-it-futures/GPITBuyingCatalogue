using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public partial class Organisation
    {
        public Organisation()
        {
            RelatedOrganisationOrganisations = new HashSet<RelatedOrganisation>();
            RelatedOrganisationRelatedOrganisationNavigations = new HashSet<RelatedOrganisation>();
        }

        public Guid OrganisationId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string OdsCode { get; set; }

        public string PrimaryRoleId { get; set; }

        public bool CatalogueAgreementSigned { get; set; }

        public DateTime LastUpdated { get; set; }

        public virtual ICollection<RelatedOrganisation> RelatedOrganisationOrganisations { get; set; }

        public virtual ICollection<RelatedOrganisation> RelatedOrganisationRelatedOrganisationNavigations { get; set; }
    }
}
