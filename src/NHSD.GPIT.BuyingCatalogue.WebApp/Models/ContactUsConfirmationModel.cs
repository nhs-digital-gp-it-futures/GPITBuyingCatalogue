namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public class ContactUsConfirmationModel : NavBaseModel
    {
        public ContactUsConfirmationModel()
        {
            BackLinkText = "Go back to homepage";
        }

        public string ContactTeam { get; set; }
    }
}
