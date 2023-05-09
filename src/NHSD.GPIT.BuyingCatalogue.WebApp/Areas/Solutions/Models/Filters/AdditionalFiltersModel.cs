using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using Microsoft.IdentityModel.Tokens;
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

        public List<ClientApplicationTypeSelectionModel> ClientApplicationTypeCheckBoxItems { get; set; }

        public void GetClientApplicationType(string clientApplicationTypeSelected)
        {
            ClientApplicationTypeCheckBoxItems = new List<ClientApplicationTypeSelectionModel>
           {
               new ClientApplicationTypeSelectionModel { ClientApplicationType = ClientApplicationType.BrowserBased, ClientApplicationEnumMemberName = ClientApplicationType.BrowserBased.EnumMemberName(), ClientApplicationdisplayName = ClientApplicationType.BrowserBased.Name() },
               new ClientApplicationTypeSelectionModel { ClientApplicationType = ClientApplicationType.Desktop, ClientApplicationEnumMemberName = ClientApplicationType.Desktop.EnumMemberName(), ClientApplicationdisplayName = ClientApplicationType.Desktop.Name() },
               new ClientApplicationTypeSelectionModel { ClientApplicationType = ClientApplicationType.MobileTablet, ClientApplicationEnumMemberName = ClientApplicationType.MobileTablet.EnumMemberName(), ClientApplicationdisplayName = ClientApplicationType.MobileTablet.Name() },
           };
            if (!clientApplicationTypeSelected.IsNullOrEmpty())
            {
                foreach (var item in ClientApplicationTypeCheckBoxItems.Where(x => clientApplicationTypeSelected.Contains(x.ClientApplicationType.EnumMemberName().ToString())))
                {
                    item.IsSelected = true;
                }
            }
        }
    }
}
