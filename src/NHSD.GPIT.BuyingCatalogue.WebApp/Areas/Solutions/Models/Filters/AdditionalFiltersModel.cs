using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters
{
    public class AdditionalFiltersModel
    {
        [ExcludeFromCodeCoverage]
        public AdditionalFiltersModel()
        {
        }

        public AdditionalFiltersModel(
            List<EntityFramework.Catalogue.Models.Framework> frameworks,
            RequestedFilters filters,
            IEnumerable<Integration> integrations)
        {
            SetFrameworkOptions(frameworks);
            ApplicationTypeOptions = SetEnumOptions<ApplicationType>(filters.SelectedApplicationTypeIds);
            HostingTypeOptions = SetEnumOptions<HostingType>(filters.SelectedHostingTypeIds);

            Selected = filters.Selected;
            SelectedFrameworkId = filters.SelectedFrameworkId;
            SortBy = filters.SortBy;

            var selectedIntegrations = filters.GetIntegrationsAndTypes();
            IntegrationOptions = integrations.Select(
                    x => new IntegrationFilterModel(
                        x.Name,
                        x.Id,
                        selectedIntegrations.ContainsKey(x.Id),
                        x.IntegrationTypes.Select(
                            y => new SelectOption<int>(
                                y.Name,
                                y.Id,
                                selectedIntegrations.TryGetValue(x.Id, out var values) && values.Contains(y.Id)))))
                .ToList();

            var capabilities = filters.GetCapabilityAndEpicIds();
            CapabilitiesCount = capabilities.Keys.Count;
            EpicsCount = capabilities.Values.Sum(v => v.Length);
        }

        public string SelectedFrameworkId { get; set; }

        public string Selected { get; set; }

        public string SortBy { get; set; }

        public List<SelectOption<string>> FrameworkOptions { get; set; }

        public List<SelectOption<int>> ApplicationTypeOptions { get; set; }

        public List<IntegrationFilterModel> IntegrationOptions { get; set; } = [];

        public int CapabilitiesCount { get; set; }

        public int EpicsCount { get; set; }

        public string[] ApplicationTypeFilters => (ApplicationTypeOptions ?? Array.Empty<SelectOption<int>>().ToList())
            .Where(f => f.Selected)
            .Select(f => f.Text)
            .ToArray();

        public List<SelectOption<int>> HostingTypeOptions { get; set; }

        public string[] HostingTypeFilters => (HostingTypeOptions ?? Array.Empty<SelectOption<int>>().ToList())
            .Where(f => f.Selected)
            .Select(f => f.Text)
            .ToArray();

        public string FoundationCapabilitiesFilterString => new FoundationCapabilitiesModel().ToFilterString();

        public string CombineSelectedOptions(List<SelectOption<int>> options)
        {
            return (options?.Where(x => x.Selected)?.Select(x => x.Value) ?? Enumerable.Empty<int>()).ToFilterString();
        }

        public RequestedFilters ToRequestedFilters()
        {
            return new RequestedFilters(
                Selected,
                null,
                SelectedFrameworkId,
                CombineSelectedOptions(ApplicationTypeOptions),
                CombineSelectedOptions(HostingTypeOptions),
                GetIntegrationIds(),
                SortBy);
        }

        public string GetIntegrationIds() => IntegrationOptions.Where(x => x.Selected)
            .ToDictionary(x => x.Id, x => x.IntegrationTypes.Where(y => y.Selected).Select(y => y.Value).ToArray())
            .ToFilterString();

        private static List<SelectOption<int>> SetEnumOptions<T>(string selection)
            where T : struct, Enum, IConvertible
            => Enum.GetValues<T>()
                .Select(
                    x => new SelectOption<int>
                    {
                        Value = x.ToInt32(CultureInfo.InvariantCulture.NumberFormat),
                        Text = x.Name(),
                        Selected = !string.IsNullOrEmpty(selection)
                            && selection.Contains(x.ToInt32(CultureInfo.InvariantCulture.NumberFormat).ToString()),
                    })
                .OrderBy(x => x.Text)
                .ToList();

        private void SetFrameworkOptions(List<EntityFramework.Catalogue.Models.Framework> frameworks)
        {
            FrameworkOptions = frameworks
                .Select(
                    f => new SelectOption<string>
                    {
                        Value = f.Id,
                        Text = $"{f.ShortName}{(f.IsExpired ? " (expired)" : string.Empty)}",
                        Selected = false,
                    })
                .ToList();
        }
    }
}
