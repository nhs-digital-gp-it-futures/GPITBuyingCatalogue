using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
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
            SetIM1IntegrationsOptions(selectedIM1Integrations);
            SetGPConnectIntegrationsOptions(selectedGPConnectIntegrations);
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

        public string[] ApplicationTypeFilters => (ApplicationTypeOptions ?? Array.Empty<SelectOption<int>>().ToList())
            .Where(f => f.Selected)
            .Select(f => f.Text)
            .ToArray();

        public List<SelectOption<int>> HostingTypeOptions { get; set; }

        public string[] HostingTypeFilters => (HostingTypeOptions ?? Array.Empty<SelectOption<int>>().ToList())
            .Where(f => f.Selected)
            .Select(f => f.Text)
            .ToArray();

        public List<SelectOption<int>> InteroperabilityOptions { get; set; }

        public string[] InteroperabilityFilters => (InteroperabilityOptions ?? Array.Empty<SelectOption<int>>().ToList())
            .Where(f => f.Selected)
            .Select(f => f.Text)
            .ToArray();

        public List<SelectOption<int>> IM1IntegrationsOptions { get; set; }

        public string[] IM1IntegrationsFilters => (IM1IntegrationsOptions ?? Array.Empty<SelectOption<int>>().ToList())
            .Where(f => f.Selected)
            .Select(f => f.Text)
            .ToArray();

        public List<SelectOption<int>> GPConnectIntegrationsOptions { get; set; }

        public string[] GPConnectIntegrationsFilters => (GPConnectIntegrationsOptions ?? Array.Empty<SelectOption<int>>().ToList())
            .Where(f => f.Selected)
            .Select(f => f.Text)
            .ToArray();

        public string CombineSelectedOptions(List<SelectOption<int>> options)
        {
            return (options?.Where(x => x.Selected)?.Select(x => x.Value) ?? Enumerable.Empty<int>()).ToFilterString();
        }

        private void SetFrameworkOptions(List<FrameworkFilterInfo> frameworks, string selectedFrameworkId)
        {
            FrameworkOptions = frameworks
                .Where(f => !f.Expired)
                .Select(
                    f => new SelectOption<string>
                    {
                        Value = f.Id, Text = $"{f.ShortName}", Selected = false,
                    })
                .ToList();

            var framework = frameworks
                .FirstOrDefault(f => f.Id == selectedFrameworkId);

            FrameworkFilter = framework != null
                ? $"{framework.ShortName}"
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

        private void SetInteroperabilityOptions(string selectedInteroperabilityOptions)
        {
            InteroperabilityOptions = Enum.GetValues(typeof(InteropIntegrationType))
                .Cast<InteropIntegrationType>()
                .Select(
                    x => new SelectOption<int>
                    {
                        Value = (int)x,
                        Text = x.EnumMemberName(),
                        Selected = !string.IsNullOrEmpty(selectedInteroperabilityOptions)
                            && selectedInteroperabilityOptions.Contains(((int)x).ToString()),
                    }).ToList();
        }

        private void SetIM1IntegrationsOptions(string selectedIM1Integrations)
        {
            IM1IntegrationsOptions = Enum.GetValues(typeof(InteropIm1IntegrationType))
                .Cast<InteropIm1IntegrationType>()
                .Select(
                    x => new SelectOption<int>
                    {
                        Value = (int)x,
                        Text = x.Name(),
                        Selected = !string.IsNullOrEmpty(selectedIM1Integrations)
                            && selectedIM1Integrations.Contains(((int)x).ToString()),
                    }).ToList();
        }

        private void SetGPConnectIntegrationsOptions(string selectedGPConnectIntegrations)
        {
            GPConnectIntegrationsOptions = Enum.GetValues(typeof(InteropGpConnectIntegrationType))
                .Cast<InteropGpConnectIntegrationType>()
                .Select(
                    x => new SelectOption<int>
                    {
                        Value = (int)x,
                        Text = x.Name(),
                        Selected = !string.IsNullOrEmpty(selectedGPConnectIntegrations)
                            && selectedGPConnectIntegrations.Contains(((int)x).ToString()),
                    }).ToList();
        }
    }
}
