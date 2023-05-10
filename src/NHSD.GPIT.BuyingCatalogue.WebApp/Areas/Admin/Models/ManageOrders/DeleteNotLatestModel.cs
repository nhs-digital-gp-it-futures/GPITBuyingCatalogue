using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders
{
    public sealed class DeleteNotLatestModel : NavBaseModel
    {
        public DeleteNotLatestModel(CallOffId callOffId)
        {
            CallOffId = callOffId;
        }

        public CallOffId CallOffId { get; set; }

        public bool IsAmendment => CallOffId.IsAmendment;
    }
}
