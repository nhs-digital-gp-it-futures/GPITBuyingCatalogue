using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public class DeleteItemConfirmationModel : NavBaseModel
    {
        public DeleteItemConfirmationModel()
        {
        }

        public DeleteItemConfirmationModel(
            string title,
            string catalogueItemName,
            string caption)
        {
            Title = title;
            CatalogueItemName = catalogueItemName;
            Advice = caption;
        }

        public string CatalogueItemName { get; set; }
    }
}
