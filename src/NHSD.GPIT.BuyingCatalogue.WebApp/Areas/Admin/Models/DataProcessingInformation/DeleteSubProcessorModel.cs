using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;

public class DeleteSubProcessorModel : NavBaseModel
{
    public DeleteSubProcessorModel()
    {
    }

    public DeleteSubProcessorModel(
        DataProtectionSubProcessor subProcessor)
    {
        OrganisationName = subProcessor.OrganisationName;
    }

    public string OrganisationName { get; set; }
}
