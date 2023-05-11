using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
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

        public AdditionalFiltersModel(List<FrameworkFilterInfo> frameworks, string selectedClientApplicationTypeIds, string selectedHostingTypeIds)
        {
            FrameworkOptions = frameworks.Select(f => new SelectOption<string>
            {
                Value = f.Id,
                Text = $"{f.ShortName} ({f.CountOfActiveSolutions})",
                Selected = false,
            }).ToList();
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

			SetHostingType(selectedHostingTypeIds);
		}

        public string SelectedFrameworkId { get; set; }

        public string SelectedHostId { get; set; }

        public List<SelectOption<string>> FrameworkOptions { get; set; }

        public List<SelectOption<int>> ClientApplicationTypeOptions { get; set; }

        public List<SelectOption<int>> HostingTypeOptions { get; set; }

        public string SelectedClientApplicationTypes
        {
            get
            {
                return string.Join(
                    FilterConstants.Delimiter,
                    ClientApplicationTypeOptions?.Where(x => x.Selected)?.Select(x => x.Value) ?? Enumerable.Empty<int>());
            }
        }

        // TODO: Try combining with SelectedClientApplicationTypes when that branch is merged
        public string SelectedHostingTypes()
        {
            return string.Join(
                    ",",
                    HostingTypeOptions.Where(x => x.Selected).Select(x => x.Value));
        }

        public string CombineSelectedOptions(List<SelectOption<int>> options)
        {
            return string.Join(
                    FilterConstants.Delimiter,
                    options.Where(x => x.Selected).Select(x => x.Value));
        }

        private void SetHostingType(string selectedHostingTypeIds)
        {
            HostingTypeOptions = Enum.GetValues(typeof(HostingType))
            .Cast<HostingType>()
            .Select(x => new SelectOption<int>
            {
                Value = (int)x,
                Text = x.Name(),
                Selected = !string.IsNullOrEmpty(selectedHostingTypeIds) && selectedHostingTypeIds.Contains(((int)x).ToString()),
            })
           .OrderByDescending(x => x.Text)
           .ToList();
            /*HostingTypeOptions = new List<SelectOption<string>> { };
            foreach (var host in Enum.GetValues(typeof(HostingType)))
            {
                HostingTypeOptions.Add(new SelectOption<string>
                {
                    Value = Enum.GetName(typeof(HostingType), host),
                    Text = host.GetType().GetMember(host.ToString())
                        .First().GetCustomAttribute<DisplayAttribute>()
                        .GetName(),
                    Selected = !string.IsNullOrEmpty(selectedHostingTypeIds) && selectedHostingTypeIds.Contains(host.ToString()),
                });*/
        }
    }
}
