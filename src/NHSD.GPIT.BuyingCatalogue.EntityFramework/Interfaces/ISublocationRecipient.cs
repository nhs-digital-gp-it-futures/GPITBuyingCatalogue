using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces
{
    public interface ISublocationRecipient
    {
        public string RecipientOdsCode { get; set; }

        public string ParentSublocationOdsCode { get; set; }

        public OdsOrganisation RecipientOdsOrganisation { get; set; }
    }
}
