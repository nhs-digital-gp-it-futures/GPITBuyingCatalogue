using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.BrowserBasedModels
{
    public class PlugInsOrExtensionsModel : ApplicationTypeBaseModel
    {
        public PlugInsOrExtensionsModel()
            : base()
        {
        }

        public PlugInsOrExtensionsModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            AdditionalInformation = ApplicationTypes?.Plugins?.AdditionalInformation;
            PlugInsRequired = ApplicationTypes?.Plugins?.Required.ToYesNo();
        }

        [StringLength(500)]
        public string AdditionalInformation { get; set; }

        public string PlugInsRequired { get; set; }
    }
}
