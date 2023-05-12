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

        public AdditionalFiltersModel(List<FrameworkFilterInfo> frameworks, string selectedClientApplicationTypeIds, string selectedCapabilityIds, string selectedEpicIds)
        {
            SelectedCapabilityIds = selectedCapabilityIds;
            SelectedEpicIds = selectedEpicIds;

            FrameworkOptions = frameworks.Select(f => new SelectOption<string>
            {
                Value = f.Id,
                Text = $"{f.ShortName} ({f.CountOfActiveSolutions})",
                Selected = false,
            }).ToList();
            ClientApplicationTypeOptions = Enum.GetValues(typeof(ClientApplicationType))
            .Cast<ClientApplicationType>()
            .Select(x => new SelectOption<int>
            {
                Value = (int)x,
                Text = x.Name(),
                Selected = !string.IsNullOrEmpty(selectedClientApplicationTypeIds) && selectedClientApplicationTypeIds.Contains(((int)x).ToString()),
            })
           .OrderBy(x => x.Text)
           .ToList();
        }

        public string SelectedCapabilityIds { get; init; }

        public string SelectedEpicIds { get; init; }

        public string SelectedFrameworkId { get; set; }

        public List<SelectOption<string>> FrameworkOptions { get; set; }

        public List<SelectOption<int>> ClientApplicationTypeOptions { get; set; }

        public string SelectedClientApplicationTypeIds
        {
            get
            {
                return string.Join(
                    FilterConstants.Delimiter,
                    ClientApplicationTypeOptions.Where(x => x.Selected).Select(x => x.Value));
            }
        }
    }
}
