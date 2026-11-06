using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models;

public interface ICapabilitiesModel
{
    public string Name { get; set; }

    public string Description { get; set; }

    public IList<RowViewModel> RowViewModels { get; }

    public string CapabilitiesHeading { get; set; }
}
