using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DataProcessing;

public class BespokeDataProcessingModel : NavBaseModel
{
    public BespokeDataProcessingModel(
        string internalOrgId,
        CallOffId callOffId)
    {
        InternalOrgId = internalOrgId;
        CallOffId = callOffId;
    }

    public BespokeDataProcessingModel()
    {
    }

    public string InternalOrgId { get; set; }

    public CallOffId CallOffId { get; set; }
}
