namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public class NewOrderItemModel : OrderingBaseModel
    {
        public NewOrderItemModel()
        {
        }

        public NewOrderItemModel(string odsCode, string callOffId, string solutionName)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution/price/flat/declarative";
            BackLinkText = "Go back";
            Title = $"{solutionName} information for {callOffId}";
        }
    }
}
