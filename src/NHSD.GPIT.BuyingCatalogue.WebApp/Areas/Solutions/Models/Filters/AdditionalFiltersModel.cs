using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
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

        public AdditionalFiltersModel(List<FrameworkFilterInfo> frameworks, string clientApplicationTypeSelected)
        {
            FrameworkOptions = frameworks.Select(f => new SelectOption<string>
            {
                Value = f.Id,
                Text = $"{f.ShortName} ({f.CountOfActiveSolutions})",
                Selected = false,
            }).ToList();
            GetClientApplicationType(clientApplicationTypeSelected);
        }

        public string SelectedFrameworkId { get; set; }

        public List<SelectOption<string>> FrameworkOptions { get; set; }

        public List<SelectOption<int>> ClientApplicaitontypeOptions { get; set; }

        public string SelectedClientApplicationTypes
        {
            get
            {
                return string.Join(
                    ",",
                    ClientApplicaitontypeOptions.Where(x => x.Selected).Select(x => x.Value));
            }
        }

        public void GetClientApplicationType(string clientApplicationTypeSelected)
        {
            ClientApplicaitontypeOptions = Enum.GetValues(typeof(ClientApplicationType))
            .Cast<ClientApplicationType>()
            .Select(x => new SelectOption<int>
            {
                 Value = (int)x,
                 Text = x.Name(),
                 Selected = !string.IsNullOrEmpty(clientApplicationTypeSelected) && clientApplicationTypeSelected.Contains(((int)x).ToString()),
            })
           .ToList();
        }
    }
}
