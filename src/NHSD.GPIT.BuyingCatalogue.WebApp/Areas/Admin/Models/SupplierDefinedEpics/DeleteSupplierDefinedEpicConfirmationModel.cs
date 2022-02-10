using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics
{
    public sealed class DeleteSupplierDefinedEpicConfirmationModel : NavBaseModel
    {
        public DeleteSupplierDefinedEpicConfirmationModel()
        {
        }

        public DeleteSupplierDefinedEpicConfirmationModel(
            string id,
            string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}
