using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels
{
    public sealed class FindOrganisationModel : NavBaseModel
    {
        public FindOrganisationModel()
        {
        }

        public FindOrganisationModel(string odsCode)
        {
            OdsCode = odsCode;
        }

        public string OdsCode { get; set; }
    }
}
