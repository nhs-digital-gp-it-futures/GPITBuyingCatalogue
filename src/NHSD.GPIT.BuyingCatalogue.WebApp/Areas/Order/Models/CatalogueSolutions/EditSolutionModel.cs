namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public class EditSolutionModel : OrderingBaseModel
    {
        public EditSolutionModel(string odsCode, string callOffId, string solutionName)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions";
            BackLinkText = "Go back";
            Title = $"{solutionName} information for {callOffId}";
            OdsCode = odsCode;
            CallOffId = callOffId;
            SolutionName = solutionName;
        }

        public string CallOffId { get; set; }

        public string SolutionName { get; set; }
    }
}
