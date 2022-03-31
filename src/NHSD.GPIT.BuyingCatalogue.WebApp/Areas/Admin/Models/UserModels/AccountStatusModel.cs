using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels
{
    public class AccountStatusModel : NavBaseModel
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string SelectedAccountStatusId { get; set; }

        public AccountStatus SelectedAccountStatus => SelectedAccountStatusId == $"{AccountStatus.Active}"
            ? AccountStatus.Active
            : AccountStatus.Inactive;

        public IList<SelectListItem> AccountStatusOptions => new List<SelectListItem>
        {
            new(Enum.GetName(AccountStatus.Active), $"{AccountStatus.Active}"),
            new(Enum.GetName(AccountStatus.Inactive), $"{AccountStatus.Inactive}"),
        };
    }
}
