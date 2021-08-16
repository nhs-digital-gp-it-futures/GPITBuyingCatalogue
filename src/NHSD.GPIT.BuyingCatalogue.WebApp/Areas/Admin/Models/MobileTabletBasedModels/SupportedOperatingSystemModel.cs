using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.MobileTabletBasedModels
{
    [ExcludeFromCodeCoverage]
    public class SupportedOperatingSystemModel
    {
        public string OperatingSystemName { get; set; }

        public bool Checked { get; set; }
    }
}
