using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

[ExcludeFromCodeCoverage(Justification = "Class of auto properties")]
public class SolutionSortModel
{
    public PageOptions.SortOptions SelectedSortOption { get; init; }

    public IEnumerable<SelectOption<string>> SortOptions => Enum.GetValues<PageOptions.SortOptions>()
        .Where(e => !e.Equals(PageOptions.SortOptions.None))
        .Select(e => new SelectOption<string>(e.Name(), e.ToString(), e == SelectedSortOption));
}
