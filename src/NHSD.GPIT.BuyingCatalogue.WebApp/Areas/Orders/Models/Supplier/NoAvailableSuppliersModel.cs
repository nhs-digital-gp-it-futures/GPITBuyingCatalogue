using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier
{
    public class NoAvailableSuppliersModel : OrderingBaseModel
    {
        public NoAvailableSuppliersModel(string internalOrgId, CallOffId callOffId)
        {
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
        }

        public CallOffId CallOffId { get; set; }
    }
}
