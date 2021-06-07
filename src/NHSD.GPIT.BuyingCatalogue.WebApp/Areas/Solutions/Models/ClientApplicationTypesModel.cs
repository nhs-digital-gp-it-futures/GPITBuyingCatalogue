using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class ClientApplicationTypesModel : SolutionDisplayBaseModel
    {
        [UIHint("DescriptionList")]
        public DescriptionListViewModel ApplicationTypes { get; set; }

        public override string Section { get; set; }

        [UIHint("DescriptionList")]
        public DescriptionListViewModel BrowserBasedApplication { get; set; }

        [UIHint("DescriptionList")]
        public DescriptionListViewModel NativeMobileApplication { get; set; }

        [UIHint("DescriptionList")]
        public DescriptionListViewModel NativeDesktopApplication { get; set; }

        public string HasApplicationType(string key) =>
            (ClientApplication?.ClientApplicationTypes?.Any(
                x => x.EqualsIgnoreCase(key))).ToYesNo();
    }
}
