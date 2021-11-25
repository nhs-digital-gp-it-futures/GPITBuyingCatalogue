namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public sealed class ErrorModel : NavBaseModel
    {
        public ErrorModel(string error)
        {
            BackLink = "/";
            BackLinkText = "Go to homepage";
            Error = error;
        }

        public string Error { get; }
    }
}
