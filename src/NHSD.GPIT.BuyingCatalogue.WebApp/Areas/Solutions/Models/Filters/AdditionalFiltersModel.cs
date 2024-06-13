using System;
using System.Collections.Generic;
using System.Linq;
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
            RequestedFilters filters)
        {
            SetFrameworkOptions(frameworks);
            SetApplicationTypeOptions(filters.SelectedApplicationTypeIds);
            SetHostingTypeOptions(filters.SelectedHostingTypeIds);
            SetIM1IntegrationsOptions(filters.SelectedIM1Integrations);
            SetGPConnectIntegrationsOptions(filters.SelectedGPConnectIntegrations);
            SetNhsAppIntegrationsOptions(filters.SelectedNhsAppIntegrations);
            SetInteroperabilityOptions(filters.SelectedInteroperabilityOptions);
            Selected = filters.Selected;
            SelectedFrameworkId = filters.SelectedFrameworkId;
            SortBy = filters.SortBy;

            var capabilities = filters.GetCapabilityAndEpicIds();
            CapabilitiesCount = capabilities.Keys.Count;
            EpicsCount = capabilities.Values.Sum(v => v.Count());
        }

        public string SelectedFrameworkId { get; set; }

        public string Selected { get; set; }

        public string SortBy { get; set; }

        public List<SelectOption<string>> FrameworkOptions { get; set; }

        public List<SelectOption<int>> ApplicationTypeOptions { get; set; }

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

        public List<SelectOption<int>> NhsAppIntegrationsOptions { get; set; }

        public string[] NhsAppIntegrationsFilters => (NhsAppIntegrationsOptions ?? Array.Empty<SelectOption<int>>().ToList())
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
                    CombineSelectedOptions(IM1IntegrationsOptions),
                    CombineSelectedOptions(GPConnectIntegrationsOptions),
                   CombineSelectedOptions(NhsAppIntegrationsOptions),
                    CombineSelectedOptions(InteroperabilityOptions),
                    SortBy);
        }

        private void SetFrameworkOptions(List<FrameworkFilterInfo> frameworks)
        {
            FrameworkOptions = frameworks
                .Select(
                    f => new SelectOption<string>
                    {
                        Value = f.Id,
                        Text = $"{f.ShortName}{(f.Expired ? " (expired)" : string.Empty)}",
                        Selected = false,
                    })
                .ToList();
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

        private void SetNhsAppIntegrationsOptions(string selectedNhsAppIntegrations)
        {
            NhsAppIntegrationsOptions = Enum.GetValues(typeof(InteropNhsAppIntegrationType))
                .Cast<InteropNhsAppIntegrationType>()
                .Select(
                    x => new SelectOption<int>
                    {
                        Value = (int)x,
                        Text = x.Name(),
                        Selected = !string.IsNullOrEmpty(selectedNhsAppIntegrations)
                            && selectedNhsAppIntegrations.Contains(((int)x).ToString()),
                    }).ToList();
        }
    }
}
