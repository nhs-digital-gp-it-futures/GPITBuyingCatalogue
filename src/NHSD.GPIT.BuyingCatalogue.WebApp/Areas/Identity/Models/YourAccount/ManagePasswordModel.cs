namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.YourAccount
{
    public class ManagePasswordModel : YourAccountBaseModel
    {
        public override int Index => 2;

        public UpdatePasswordViewModel UpdatePasswordViewModel { get; set; } = new UpdatePasswordViewModel();

        public bool Saved { get; set; }
    }
}
