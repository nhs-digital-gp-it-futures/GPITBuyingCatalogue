using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;

public class SelectAdditionalServicesModel : SelectServicesModel
{
    public SelectAdditionalServicesModel()
        : base()
    {
    }

    public SelectAdditionalServicesModel(
        ICollection<CatalogueItem> excludedServices,
        ICollection<CatalogueItem> allServices)
        : base(excludedServices, allServices)
    {
        ExistingServices = excludedServices.Select(x => x.Name).ToList();
    }

    public List<string> ExistingServices { get; set; } = Enumerable.Empty<string>().ToList();
}
