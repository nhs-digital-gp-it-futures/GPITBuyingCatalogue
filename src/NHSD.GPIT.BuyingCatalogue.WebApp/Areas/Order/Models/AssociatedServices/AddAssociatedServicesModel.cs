using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public class AddAssociatedServicesModel : NavBaseModel
    {
        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public string AdditionalServicesRequired { get; set; }
    }
}
