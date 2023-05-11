﻿using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters
{
    public class AdditionalFiltersModel
    {
        public AdditionalFiltersModel()
        {
        }

        public AdditionalFiltersModel(List<FrameworkFilterInfo> frameworks, string selectedCapabilityIds, string selectedEpicIds)
        {
            SelectedCapabilityIds = selectedCapabilityIds;
            SelectedEpicIds = selectedEpicIds;

            FrameworkOptions = frameworks.Select(f => new SelectOption<string>
            {
                Value = f.Id,
                Text = $"{f.ShortName} ({f.CountOfActiveSolutions})",
                Selected = false,
            }).ToList();
        }

        public string SelectedCapabilityIds { get; init; }

        public string SelectedEpicIds { get; init; }

        public string SelectedFrameworkId { get; set; }

        public List<SelectOption<string>> FrameworkOptions { get; set; }
    }
}
