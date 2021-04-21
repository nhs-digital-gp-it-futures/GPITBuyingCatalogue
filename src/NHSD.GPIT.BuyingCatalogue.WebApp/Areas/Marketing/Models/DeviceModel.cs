using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class DeviceModel
    {
        public DeviceModel()
        {
        }

        public DeviceModel(CatalogueItem catalogueItem)
        {
            SolutionId = catalogueItem.CatalogueItemId;
            ClientApplication = catalogueItem.Solution.GetClientApplication();
        }

        public string SolutionId { get; set; }

        public ClientApplication ClientApplication { get; set; }
    }
}
