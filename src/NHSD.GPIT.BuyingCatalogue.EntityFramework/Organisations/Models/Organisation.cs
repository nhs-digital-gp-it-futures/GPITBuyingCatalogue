using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models
{
    [Serializable]
    public sealed class Organisation : IAudited
    {
        public Organisation()
        {
            Orders = new HashSet<Order>();
            RelatedOrganisationOrganisations = new HashSet<RelatedOrganisation>();
            RelatedOrganisationRelatedOrganisationNavigations = new HashSet<RelatedOrganisation>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }

        public string ExternalIdentifier { get; set; }

        public string InternalIdentifier { get; set; }

        public OrganisationType? OrganisationType { get; set; }

        public string PrimaryRoleId { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<RelatedOrganisation> RelatedOrganisationOrganisations { get; set; }

        public ICollection<RelatedOrganisation> RelatedOrganisationRelatedOrganisationNavigations { get; set; }
    }
}
