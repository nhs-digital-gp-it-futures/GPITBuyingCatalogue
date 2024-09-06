using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;

public class DataProcessingInformationModel : NavBaseModel
{
    public DataProcessingInformationModel(
        Solution solution)
    {
        Solution = solution;
    }

    public Solution Solution { get; set; }
}
