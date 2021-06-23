namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public class DeleteAssociatedServiceModel : OrderingBaseModel
    {
        public DeleteAssociatedServiceModel()
        {
        }

        public DeleteAssociatedServiceModel(string odsCode, string callOffId, string orderItemId, string solutionName, string orderDescription)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/associated-services/{orderItemId}";
            BackLinkText = "Go back";
            Title = $"Delete {solutionName} from {callOffId}?";
            OdsCode = odsCode;
            CallOffId = callOffId;
            OrderItemId = orderItemId;
            SolutionName = solutionName;
            OrderDescription = orderDescription;
        }

        public string CallOffId { get; set; }

        public string OrderItemId { get; set; }

        public string SolutionName { get; set; }

        public string OrderDescription { get; set; }
    }
}
