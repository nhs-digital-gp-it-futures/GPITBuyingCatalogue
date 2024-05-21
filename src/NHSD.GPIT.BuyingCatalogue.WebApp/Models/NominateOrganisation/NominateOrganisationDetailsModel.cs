using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.NominateOrganisation
{
    public class NominateOrganisationDetailsModel : NavBaseModel
    {
        [StringLength(500)]
        public string OrganisationName { get; set; }

        [StringLength(10)]
        public string OdsCode { get; set; }
    }
}
