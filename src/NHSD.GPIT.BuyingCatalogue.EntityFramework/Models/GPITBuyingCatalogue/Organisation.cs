using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public sealed class Organisation
    {
        public Organisation()
        {
            Orders = new HashSet<Order>();
            RelatedOrganisationOrganisations = new HashSet<RelatedOrganisation>();
            RelatedOrganisationRelatedOrganisationNavigations = new HashSet<RelatedOrganisation>();
        }

        public Guid OrganisationId { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }

        public string OdsCode { get; set; }

        public string PrimaryRoleId { get; set; }

        public bool CatalogueAgreementSigned { get; set; }

        public DateTime LastUpdated { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<RelatedOrganisation> RelatedOrganisationOrganisations { get; set; }

        public ICollection<RelatedOrganisation> RelatedOrganisationRelatedOrganisationNavigations { get; set; }
    }
}
