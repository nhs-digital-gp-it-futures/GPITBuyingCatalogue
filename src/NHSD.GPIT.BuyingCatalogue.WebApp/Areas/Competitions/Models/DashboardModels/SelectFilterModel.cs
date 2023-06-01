using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;

public class SelectFilterModel : NavBaseModel
{
    public SelectFilterModel()
    {
    }

    public SelectFilterModel(
        string organisationName,
        IEnumerable<Filter> filters)
    {
        OrganisationName = organisationName;

        WithFilters(filters);
    }

    public string OrganisationName { get; set; }

    public List<SelectOption<string>> Filters { get; set; }

    public int? SelectedFilterId { get; set; }

    public void WithFilters(IEnumerable<Filter> filters)
    {
        Filters = filters.Select(x => new SelectOption<string>(x.Name, x.Id.ToString())).ToList();
    }
}
