using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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

        public static Dictionary<SupportedIntegrations, int[]>
            ParseIntegrationAndTypeIds(string selectedIntegrations)
        {
            var integrations = selectedIntegrations?
                    .Split(FilterConstants.GroupDelimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
                ?? Array.Empty<string>();

            return new Dictionary<SupportedIntegrations, int[]>(integrations
                .Select(x => x.Split(FilterConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries))
                .Where(x => Enum.TryParse(typeof(SupportedIntegrations), x[0], out var parsed) && Enum.IsDefined(typeof(SupportedIntegrations), parsed))
                .Select(x => new KeyValuePair<SupportedIntegrations, int[]>(Enum.Parse<SupportedIntegrations>(x[0]), x.Skip(1).Where(y => int.TryParse(y, out _)).Select(int.Parse).ToArray())));
        }

        public static ICollection<T> ParseEnumFilter<T>(string enumDelimitedValues)
            where T : struct, Enum
            =>
                enumDelimitedValues?.Split(
                        FilterConstants.Delimiter,
                        StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
                    .Where(x => Enum.TryParse(typeof(T), x, out var value) && Enum.IsDefined(typeof(T), value))
                    .Select(t => (T)Enum.Parse(typeof(T), t))
                    .ToList() ?? new List<T>();

        public static ICollection<T> ParseSelectedFilterIds<T>(string selectedFilterIds)
        where T : struct, Enum
        {
            if (string.IsNullOrEmpty(selectedFilterIds))
                return new List<T>();

            var selectedFilterEnums = selectedFilterIds.Split(FilterConstants.Delimiter)
                .Where(t => Enum.TryParse<T>(t, out var enumVal) && Enum.IsDefined(enumVal))
                .Select(Enum.Parse<T>)
                .ToList();

            if (selectedFilterEnums == null || !selectedFilterEnums.Any())
                throw new ArgumentException("Invalid filter format", nameof(selectedFilterIds));

            return selectedFilterEnums;
        }
    }
}
