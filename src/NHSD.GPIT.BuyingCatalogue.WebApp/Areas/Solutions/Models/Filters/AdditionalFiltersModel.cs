using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters
{
    public class AdditionalFiltersModel
    {
        public AdditionalFiltersModel()
        {
        }

        public AdditionalFiltersModel(
            List<FrameworkFilterInfo> frameworks,
            string selectedFrameworkId,
            string selectedApplicationTypeIds,
            string selectedHostingTypeIds,
            string selectedIM1Integrations,
            string selectedGPConnectIntegrations,
            string selectedInteroperabilityOptions,
            string selected)
        {
            SetFrameworkOptions(frameworks, selectedFrameworkId);
            SetApplicationTypeOptions(selectedApplicationTypeIds);
            SetHostingTypeOptions(selectedHostingTypeIds);
            SetIM1IntegrationsOptions(selectedIM1Integrations, selectedInteroperabilityOptions);
            SetGPConnectIntegrationsOptions(selectedGPConnectIntegrations, selectedInteroperabilityOptions);
            SetInteroperabilityOptions(selectedInteroperabilityOptions);
            Selected = selected;
            SelectedFrameworkId = selectedFrameworkId;
        }

        public int? FilterId { get; set; }

        public string SelectedFrameworkId { get; set; }

        public string Selected { get; set; }

        public string SortBy { get; set; }

        public List<SelectOption<string>> FrameworkOptions { get; set; }

        public string FrameworkFilter { get; set; }

        public List<SelectOption<int>> ApplicationTypeOptions { get; set; }

        public string[] ApplicationTypeFilters => ApplicationTypeOptions
            ?.Where(f => f.Selected)
            ?.Select(f => f.Text)
            ?.ToArray() ?? Array.Empty<string>();

        public List<SelectOption<int>> HostingTypeOptions { get; set; }

        public string[] HostingTypeFilters => HostingTypeOptions
            ?.Where(f => f.Selected)
            ?.Select(f => f.Text)
            ?.ToArray() ?? Array.Empty<string>();

        public List<SelectOption<string>> InteroperabilityOptions { get; set; }

        public string[] IM1IntegrationsSelect => InteroperabilityOptions
            ?.Where(f => f.Selected)
            ?.Select(f => f.Text)
            ?.ToArray() ?? Array.Empty<string>();

        public List<SelectOption<string>> IM1IntegrationsOptions { get; set; }

        public string[] IM1IntegrationsFilters => IM1IntegrationsOptions
            ?.Where(f => f.Selected)
            ?.Select(f => f.Text)
            ?.ToArray() ?? Array.Empty<string>();

        public List<SelectOption<string>> GPConnectIntegrationsOptions { get; set; }

        public string[] GPConnectIntegrationsFilters => GPConnectIntegrationsOptions
            ?.Where(f => f.Selected)
            ?.Select(f => f.Text)
            ?.ToArray() ?? Array.Empty<string>();

        public string CombineSelectedOptions(List<SelectOption<int>> options)
        {
            return (options?.Where(x => x.Selected)?.Select(x => x.Value) ?? Enumerable.Empty<int>()).ToFilterString();
        }

        public string CombineSelectedOptions(List<SelectOption<string>> options)
        {
            return (options?.Where(x => x.Selected)?.Select(x => x.Value) ?? Enumerable.Empty<string>()).ToFilterString();
        }

        private void SetFrameworkOptions(List<FrameworkFilterInfo> frameworks, string selectedFrameworkId)
        {
            FrameworkOptions = frameworks
                .Where(f => !f.Expired)
                .Select(
                    f => new SelectOption<string>
                    {
                        Value = f.Id, Text = $"{f.ShortName} ({f.CountOfActiveSolutions})", Selected = false,
                    })
                .ToList();

            var framework = frameworks
                .FirstOrDefault(f => f.Id == selectedFrameworkId);

            FrameworkFilter = framework != null
                ? $"{framework.ShortName} ({framework.CountOfActiveSolutions})"
                : string.Empty;
        }

        private void SetApplicationTypeOptions(string selectedApplicationTypeIds)
        {
            ApplicationTypeOptions = Enum.GetValues(typeof(ApplicationType))
                .Cast<ApplicationType>()
                .Select(
                    x => new SelectOption<int>
                    {
                        Value = (int)x,
                        Text = x.Name(),
                        Selected = !string.IsNullOrEmpty(selectedApplicationTypeIds)
                            && selectedApplicationTypeIds.Contains(((int)x).ToString()),
                    })
                .OrderBy(x => x.Text)
                .ToList();
        }

        private void SetHostingTypeOptions(string selectedHostingTypeIds)
        {
            HostingTypeOptions = Enum.GetValues(typeof(HostingType))
                .Cast<HostingType>()
                .Select(
                    x => new SelectOption<int>
                    {
                        Value = (int)x,
                        Text = x.Name(),
                        Selected = !string.IsNullOrEmpty(selectedHostingTypeIds)
                            && selectedHostingTypeIds.Contains(((int)x).ToString()),
                    })
                .OrderBy(x => x.Text)
                .ToList();
        }

        private void SetIM1IntegrationsOptions(string selectedIM1Integrations, string selectedInteroperabilityOptions)
        {
            IM1IntegrationsOptions = Interoperability.Im1Integrations
            .Select(x => new SelectOption<string>
            {
                Value = x.Key,
                Text = x.Value,
                Selected = !string.IsNullOrEmpty(selectedIM1Integrations)
                            && selectedIM1Integrations.Contains(x.Key) && (!string.IsNullOrEmpty(selectedInteroperabilityOptions)
                            && selectedInteroperabilityOptions.Contains(Interoperability.IM1IntegrationType)),
            }).ToList();
        }

        private void SetGPConnectIntegrationsOptions(string selectedGPConnectIntegrations, string selectedInteroperabilityOptions)
        {
            GPConnectIntegrationsOptions = Interoperability.GpConnectIntegrations
            .Select(x => new SelectOption<string>
            {
                Value = x.Key,
                Text = x.Value,
                Selected = !string.IsNullOrEmpty(selectedGPConnectIntegrations)
                            && selectedGPConnectIntegrations.Contains(x.Key) && (!string.IsNullOrEmpty(selectedInteroperabilityOptions)
                            && selectedInteroperabilityOptions.Contains(Interoperability.GpConnectIntegrationType)),
            }).ToList();
        }

        private void SetInteroperabilityOptions(string selectedInteroperabilityOptions)
        {
            InteroperabilityOptions = new List<SelectOption<string>>
            {
                new SelectOption<string>
                {
                    Value = Interoperability.IM1IntegrationType,
                    Text = Interoperability.IM1IntegrationType,
                    Selected = !string.IsNullOrEmpty(selectedInteroperabilityOptions)
                                && selectedInteroperabilityOptions.Contains(Interoperability.IM1IntegrationType),
                },
                new SelectOption<string>
                {
                    Value = Interoperability.GpConnectIntegrationType,
                    Text = Interoperability.GpConnectIntegrationType,
                    Selected = !string.IsNullOrEmpty(selectedInteroperabilityOptions)
                                && selectedInteroperabilityOptions.Contains(Interoperability.GpConnectIntegrationType),
                },
            };
        }
    }
}
