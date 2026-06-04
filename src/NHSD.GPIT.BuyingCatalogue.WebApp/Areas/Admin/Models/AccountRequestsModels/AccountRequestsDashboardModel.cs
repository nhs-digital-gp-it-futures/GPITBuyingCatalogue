using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AccountRequestModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AccountRequestsModels;

public class AccountRequestsDashboardModel : NavBaseModel
{
    public AccountRequestsDashboardModel()
    {
    }

    public AccountRequestsDashboardModel(
        AccountRequestStatus selectedStatus,
        AccountRequestOverviewModel model)
    {
        SelectedStatus = selectedStatus;
        AccountRequestsModel = model;
    }

    public AccountRequestOverviewModel AccountRequestsModel { get; set; }

    public AccountRequestStatus? SelectedStatus { get; set; }

    public bool HasItems => AccountRequestsModel?.AccountRequests?.Count > 0;

    public List<SelectOption<AccountRequestStatus>> AccountRequestStatuses { get; } = Enum
        .GetValues<AccountRequestStatus>()
        .Select(x => new SelectOption<AccountRequestStatus>(x.ToString(), x))
        .ToList();

    public string StatusColumnHeading => SelectedStatus switch
    {
        AccountRequestStatus.Pending => "Days active",
        _ => $"{SelectedStatus.ToString()} date",
    };
}
