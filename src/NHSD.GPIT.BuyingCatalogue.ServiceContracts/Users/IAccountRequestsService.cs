using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AccountRequestModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;

public interface IAccountRequestsService
{
    Task<AccountRequestOverviewModel> GetAccountRequests(AccountRequestStatus status, PageOptions options);

    Task<AccountRequest> GetAccountRequest(Guid id);

    Task ProcessAccountRequest(Guid requestId, int decidingUserId, AccountRequestStatus status, string justification);

    Task SubmitAccountRequest(AccountRequest accountRequest, RoutingResult confirmationRoute);

    Task ConfirmAccountRequest(Guid requestId, string email);
}
