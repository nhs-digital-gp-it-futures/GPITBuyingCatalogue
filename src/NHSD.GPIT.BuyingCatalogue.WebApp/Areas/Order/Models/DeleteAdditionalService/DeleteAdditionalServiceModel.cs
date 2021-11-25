using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteAdditionalService
{
    public sealed class DeleteAdditionalServiceModel : OrderingBaseModel
    {
        public DeleteAdditionalServiceModel()
        {
        }

        public DeleteAdditionalServiceModel(string odsCode, CallOffId callOffId, CatalogueItemId additionalServiceId, string solutionName, string orderDescription)
        {
            Title = $"Delete {solutionName} from {callOffId}?";
            OdsCode = odsCode;
            CallOffId = callOffId;
            AdditionalServiceId = additionalServiceId;
            SolutionName = solutionName;
            OrderDescription = orderDescription;
        }

        public CallOffId CallOffId { get; set; }

        public CatalogueItemId AdditionalServiceId { get; set; }

        public string SolutionName { get; set; }

        public string OrderDescription { get; set; }
    }
}
