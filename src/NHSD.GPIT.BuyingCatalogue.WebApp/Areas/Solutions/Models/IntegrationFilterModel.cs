using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

public class IntegrationFilterModel
{
    public IntegrationFilterModel()
    {
    }

    public IntegrationFilterModel(
        string name,
        SupportedIntegrations id,
        bool selected,
        IEnumerable<SelectOption<int>> integrationTypes)
    {
        Id = (int)id;
        Name = name;
        Selected = selected;
        IntegrationTypes = integrationTypes.ToList();
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public bool Selected { get; set; }

    public IList<SelectOption<int>> IntegrationTypes { get; set; }
}
