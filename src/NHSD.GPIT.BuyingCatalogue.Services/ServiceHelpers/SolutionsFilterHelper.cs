using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers
{
    public static class SolutionsFilterHelper
    {
        public static ICollection<int> ParseCapabilityIds(string capabilityIds) =>
            capabilityIds?.Split(FilterConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
                .Where(x => int.TryParse(x, out _))
                .Select(int.Parse)
                .ToList() ?? new List<int>();

        public static Dictionary<int, string[]> ParseCapabilityAndEpicIds(string capabilityAndEpicsFilterString)
        {
            var capabilityAndEpics = capabilityAndEpicsFilterString?
                .Split(FilterConstants.GroupDelimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
                ?? Array.Empty<string>();

            return new Dictionary<int, string[]>(capabilityAndEpics
                .Select(x => x.Split(FilterConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries))
                .Where(x => int.TryParse(x[0], out _))
                .Select(x => new KeyValuePair<int, string[]>(int.Parse(x[0], CultureInfo.InvariantCulture), x.Skip(1).ToArray())));
        }

        public static ICollection<ApplicationType> ParseApplicationTypeIds(string applicationTypeIds) =>
            applicationTypeIds?.Split(FilterConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
                .Where(x => Enum.TryParse(typeof(ApplicationType), x, out var catValue) && Enum.IsDefined(typeof(ApplicationType), catValue))
                .Select(t => (ApplicationType)Enum.Parse(typeof(ApplicationType), t))
                .ToList() ?? new List<ApplicationType>();

        public static ICollection<HostingType> ParseHostingTypeIds(string hostingTypeIds) =>
            hostingTypeIds?.Split(FilterConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
                .Where(x => Enum.TryParse(typeof(HostingType), x, out var hostingValue) && Enum.IsDefined(typeof(HostingType), hostingValue))
                .Select(t => (HostingType)Enum.Parse(typeof(HostingType), t))
                .ToList() ?? new List<HostingType>();
    }
}
