using System;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity
{
    public partial class Organisation
    {
        public Guid OrganisationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string OdsCode { get; set; }
        public string PrimaryRoleId { get; set; }
        public bool CatalogueAgreementSigned { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
