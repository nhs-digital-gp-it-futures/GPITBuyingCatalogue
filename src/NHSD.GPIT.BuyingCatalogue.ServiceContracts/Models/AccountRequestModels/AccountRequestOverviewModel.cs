using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AccountRequestModels;

public class AccountRequestOverviewModel
{
    public int TotalNumberOfRequests { get; set; }

    public int TotalNumberOfApprovedRequests { get; set; }

    public int TotalNumberOfRejectedRequests { get; set; }

    public int TotalNumberOfPendingRequests { get; set; }

    public ICollection<AccountRequest> AccountRequests { get; set; }

    public PageOptions Options { get; set; }
}
