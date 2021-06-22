namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public class DeleteAdditionalServiceModel : OrderingBaseModel
    {
        public DeleteAdditionalServiceModel()
        {
        }

        public DeleteAdditionalServiceModel(string odsCode, string callOffId, string orderItemId, string solutionName, string orderDescription)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services/{orderItemId}";
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
