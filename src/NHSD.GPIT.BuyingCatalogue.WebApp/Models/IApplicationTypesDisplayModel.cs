using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models;

public interface IApplicationTypesDisplayModel
{
    [UIHint("DescriptionList")]
    public DescriptionListViewModel ApplicationTypes { get; }

    [UIHint("DescriptionList")]
    public DescriptionListViewModel BrowserBasedApplication { get; }

    [UIHint("DescriptionList")]
    public DescriptionListViewModel NativeDesktopApplication { get; }

    [UIHint("DescriptionList")]
    public DescriptionListViewModel NativeMobileApplication { get; }
}
