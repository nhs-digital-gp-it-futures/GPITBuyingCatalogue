using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

public class SolutionSortModel
{
    public PageOptions.SortOptions SelectedSortOption { get; init; }

    public IEnumerable<SelectOption<string>> SortOptions => Enum.GetValues<PageOptions.SortOptions>()
        .Where(e => !e.Equals(PageOptions.SortOptions.None))
        .Select(e => new SelectOption<string>(e.Name(), e.ToString(), e == SelectedSortOption));
}
