using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.MobileTabletBasedModels
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public class ConnectionTypeModel
    {
        public string ConnectionType { get; set; }

        public bool Checked { get; set; }
    }
}
