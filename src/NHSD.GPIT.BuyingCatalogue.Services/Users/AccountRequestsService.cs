using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AccountRequestModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;

namespace NHSD.GPIT.BuyingCatalogue.Services.Users;

public class AccountRequestsService(
    BuyingCatalogueDbContext dbContext,
    ICreateUserService createUserService,
    IOrganisationsService organisationsService,
    IGovNotifyEmailService govNotifyEmailService,
    IEmailDomainService emailDomainService,
    IHttpContextAccessor httpContextAccessor,
    LinkGenerator linkGenerator,
    DomainNameSettings domainNameSettings,
    AccountTemplateSettings settings) : IAccountRequestsService
{
    private readonly BuyingCatalogueDbContext dbContext =
        dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    private readonly ICreateUserService createUserService =
        createUserService ?? throw new ArgumentNullException(nameof(createUserService));

    private readonly IOrganisationsService organisationsService =
        organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));

    private readonly IGovNotifyEmailService govNotifyEmailService =
        govNotifyEmailService ?? throw new ArgumentNullException(nameof(govNotifyEmailService));

    private readonly IEmailDomainService emailDomainService =
        emailDomainService ?? throw new ArgumentNullException(nameof(emailDomainService));

    private readonly IHttpContextAccessor httpContextAccessor =
        httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    private readonly LinkGenerator linkGenerator =
        linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));

    private readonly DomainNameSettings domainNameSettings =
        domainNameSettings ?? throw new ArgumentNullException(nameof(domainNameSettings));

    private readonly AccountTemplateSettings settings =
        settings ?? throw new ArgumentNullException(nameof(settings));

    public async Task<AccountRequestOverviewModel> GetAccountRequests(AccountRequestStatus status, PageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var accountRequests = dbContext.AccountRequests.AsQueryable();

        accountRequests = status is AccountRequestStatus.Pending
            ? accountRequests.OrderBy(x => x.RequestedOn)
            : accountRequests.OrderByDescending(x => x.RequestedOn);

        options.TotalNumberOfItems = await accountRequests.CountAsync(x => x.Status == status);

        var model = new AccountRequestOverviewModel
        {
            TotalNumberOfRequests = await accountRequests.CountAsync(),
            TotalNumberOfApprovedRequests =
                await accountRequests.CountAsync(x => x.Status == AccountRequestStatus.Approved),
            TotalNumberOfPendingRequests =
                await accountRequests.CountAsync(x => x.Status == AccountRequestStatus.Pending),
            TotalNumberOfRejectedRequests =
                await accountRequests.CountAsync(x => x.Status == AccountRequestStatus.Rejected),
            AccountRequests = await accountRequests.Include(x => x.Organisation)
                .Where(x => x.Status == status)
                .Skip(Math.Max(0, (options.PageNumber - 1) * options.PageSize))
                .Take(options.PageSize)
                .ToListAsync(),
            Options = options,
        };

        return model;
    }

    public async Task<AccountRequest> GetAccountRequest(Guid id)
        => await dbContext.AccountRequests.Include(x => x.Organisation)
            .Include(x => x.DecidedByUser)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task ProcessAccountRequest(
        Guid requestId,
        int decidingUserId,
        AccountRequestStatus status,
        string justification)
    {
        if (status is AccountRequestStatus.Pending)
        {
            throw new ArgumentException(
                @"An account request can only be processed with Approved or Rejected status",
                nameof(status));
        }

        var accountRequest = await dbContext.AccountRequests.FirstOrDefaultAsync(x => x.Id == requestId);

        accountRequest.Status = status;
        accountRequest.DecisionJustification = justification;
        accountRequest.DecidedBy = decidingUserId;
        accountRequest.DecidedOn = DateTime.UtcNow;

        if (status is AccountRequestStatus.Approved)
        {
            var organisationId = await organisationsService.GetOrAddOrganisation(accountRequest.OdsCode);

            var user = await createUserService.Create(
                organisationId,
                accountRequest.FirstName,
                accountRequest.LastName,
                accountRequest.Email,
                OrganisationFunction.Buyer.Name,
                isDisabled: false,
                accountRequest.HasOptedInUserResearch);

            accountRequest.UserId = user.Id;
        }
        else
        {
            await govNotifyEmailService.SendEmailAsync(accountRequest.Email, settings.AccountRejectedTemplateId, null);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task SubmitAccountRequest(AccountRequest accountRequest, RoutingResult confirmationRoute)
    {
        ArgumentNullException.ThrowIfNull(accountRequest);
        ArgumentNullException.ThrowIfNull(confirmationRoute);

        var emailDomainAllowed = await emailDomainService.IsAllowed(accountRequest.Email);
        if (!emailDomainAllowed) return;

        var odsCodeValid = await dbContext.OdsOrganisations.AnyAsync(x => x.Id == accountRequest.OdsCode);
        if (!odsCodeValid)
        {
            await govNotifyEmailService.SendEmailAsync(accountRequest.Email, settings.InvalidOdsCodeTemplateId, null);
            return;
        }

        var isDuplicateRequest = await dbContext.AccountRequests.IgnoreQueryFilters()
            .AnyAsync(x => x.Email == accountRequest.Email);
        var userExists = await dbContext.AspNetUsers.AnyAsync(x => x.Email == accountRequest.Email);
        if (isDuplicateRequest || userExists)
        {
            await govNotifyEmailService.SendEmailAsync(accountRequest.Email, settings.AccountDuplicateTemplateId, null);
            return;
        }

        await dbContext.AccountRequests.AddAsync(accountRequest);
        await dbContext.SaveChangesAsync();

        var confirmationRouteUrl = linkGenerator.GetUriByAction(
            httpContextAccessor.HttpContext!,
            confirmationRoute.ActionName,
            confirmationRoute.ControllerName,
            new { area = confirmationRoute.AreaName, requestId = accountRequest.Id, email = accountRequest.Email },
            "https",
            new HostString(domainNameSettings.DomainName));

        await govNotifyEmailService.SendEmailAsync(
            accountRequest.Email,
            settings.AccountSubmittedTemplateId,
            new Dictionary<string, dynamic> { { "confirmation_link", confirmationRouteUrl } });
    }

    public async Task ConfirmAccountRequest(Guid requestId, string email)
    {
        var accountRequest =
            await dbContext.AccountRequests.IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == requestId && x.Email == email);
        if (accountRequest is null) return;

        accountRequest.IsConfirmed = true;
        await dbContext.SaveChangesAsync();
    }
}
