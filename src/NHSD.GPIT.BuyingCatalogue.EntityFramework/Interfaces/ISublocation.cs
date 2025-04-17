using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces
{
    public interface ISublocation
    {
        public string SublocationOdsCode { get; set; }

        public string OwnerOdsCode { get; set; }

        public OdsOrganisation SublocationOrganisation { get; set; }
    }
}
