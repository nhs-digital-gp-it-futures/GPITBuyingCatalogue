using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.FrameworkModels;

public class AddEditFrameworkModel : NavBaseModel
{
    public string FrameworkId { get; set; }

    public string Name { get; set; }

    public bool IsLocalFundingOnly { get; set; }
}
