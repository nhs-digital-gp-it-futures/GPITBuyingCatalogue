using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters
{
    public class ClientApplicationTypeSelectionModel
    {
        public ClientApplicationType ClientApplicationType { get; set; }

        public string ClientApplicationEnumMemberName { get; set; }

        public bool IsSelected { get; set; }
    }
}
