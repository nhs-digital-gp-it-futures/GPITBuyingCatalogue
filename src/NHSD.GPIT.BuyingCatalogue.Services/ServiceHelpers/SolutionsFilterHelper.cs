using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;

namespace NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers
{
    public static class SolutionsFilterHelper
    {
        public static ICollection<int> ParseCapabilityIds(string capabilityIds) =>
            capabilityIds?.Split(FilterConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
                .Where(x => int.TryParse(x, out _))
                .Select(int.Parse)
                .ToList() ?? new List<int>();

        public static ICollection<string> ParseEpicIds(string epicIds) =>
            epicIds?.Split(FilterConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
        .ToList() ?? new List<string>();
    }
}
