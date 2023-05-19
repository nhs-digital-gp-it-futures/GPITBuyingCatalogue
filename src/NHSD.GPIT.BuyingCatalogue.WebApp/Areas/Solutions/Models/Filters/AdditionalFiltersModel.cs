using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
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

        public AdditionalFiltersModel(List<FrameworkFilterInfo> frameworks, string selectedClientApplicationTypeIds, string selectedHostingTypeIds, string selectedCapabilityIds, string selectedEpicIds)
        {
            SetFrameworkOptions(frameworks);
            SetClientApplicationTypeOptions(selectedClientApplicationTypeIds);
            SetHostingTypeOptions(selectedHostingTypeIds);
        }

        public string SelectedFrameworkId { get; set; }

        public string SelectedCapabilityIds { get; set; }

        public string SelectedEpicIds { get; set; }

        public string SelectedHostId { get; set; }

        public List<SelectOption<string>> FrameworkOptions { get; set; }

        public List<SelectOption<int>> ClientApplicationTypeOptions { get; set; }

        public List<SelectOption<int>> HostingTypeOptions { get; set; }

        public string CombineSelectedOptions(List<SelectOption<int>> options)
        {
            return string.Join(
                    FilterConstants.Delimiter,
                    options?.Where(x => x.Selected)?.Select(x => x.Value) ?? Enumerable.Empty<int>());
        }

        private void SetFrameworkOptions(List<FrameworkFilterInfo> frameworks)
        {
            FrameworkOptions = frameworks.Select(f => new SelectOption<string>
            {
                Value = f.Id,
                Text = $"{f.ShortName} ({f.CountOfActiveSolutions})",
                Selected = false,
            }).ToList();
        }

        private void SetClientApplicationTypeOptions(string selectedClientApplicationTypeIds)
        {
            ClientApplicationTypeOptions = Enum.GetValues(typeof(ClientApplicationType))
                .Cast<ClientApplicationType>()
                .Select(
                    x => new SelectOption<int>
                    {
                        Value = (int)x,
                        Text = x.Name(),
                        Selected = !string.IsNullOrEmpty(selectedClientApplicationTypeIds)
                            && selectedClientApplicationTypeIds.Contains(((int)x).ToString()),
                    })
                .OrderBy(x => x.Text)
                .ToList();
        }

        private void SetHostingTypeOptions(string selectedHostingTypeIds)
        {
            HostingTypeOptions = Enum.GetValues(typeof(HostingType))
                .Cast<HostingType>()
                .Select(x => new SelectOption<int>
                {
                    Value = (int)x,
                    Text = x.Name(),
                    Selected = !string.IsNullOrEmpty(selectedHostingTypeIds)
                        && selectedHostingTypeIds.Contains(((int)x).ToString()),
                })
               .OrderByDescending(x => x.Text)
               .ToList();
        }
    }
}
