using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.Services.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Users;

public static class AccountRequestsServiceTests
{
    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetAccountRequests_Pending_OrdersByAscending(
        List<AccountRequest> accountRequests,
        [Frozen] BuyingCatalogueDbContext context,
        AccountRequestsService service)
    {
        const AccountRequestStatus status = AccountRequestStatus.Pending;

        accountRequests.ForEach(x =>
        {
            x.Status = status;
            x.IsConfirmed = true;
        });

        context.AccountRequests.RemoveRange(context.AccountRequests);
        await context.SaveChangesAsync();

        context.AccountRequests.AddRange(accountRequests);
        await context.SaveChangesAsync();

        var result = await service.GetAccountRequests(status, new PageOptions("0", 10));

        result.AccountRequests.Should().AllSatisfy(x => x.Status.Should().Be(status));
        result.AccountRequests.Should().BeInAscendingOrder(x => x.RequestedOn);
    }

    [Theory]
    [MockInMemoryDbInlineAutoData(AccountRequestStatus.Approved)]
    [MockInMemoryDbInlineAutoData(AccountRequestStatus.Rejected)]
    public static async Task GetAccountRequests_RejectedOrApproved_OrdersByDescending(
        AccountRequestStatus status,
        List<AccountRequest> accountRequests,
        [Frozen] BuyingCatalogueDbContext context,
        AccountRequestsService service)
    {
        accountRequests.ForEach(x =>
        {
            x.Status = status;
            x.IsConfirmed = true;
        });

        context.AccountRequests.RemoveRange(context.AccountRequests);
        await context.SaveChangesAsync();

        context.AccountRequests.AddRange(accountRequests);
        await context.SaveChangesAsync();

        var result = await service.GetAccountRequests(status, new PageOptions("0", 10));

        result.AccountRequests.Should().AllSatisfy(x => x.Status.Should().Be(status));
        result.AccountRequests.Should().BeInDescendingOrder(x => x.RequestedOn);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetAccountRequests_ReturnsMetadata(
        List<AccountRequest> accountRequests,
        [Frozen] BuyingCatalogueDbContext context,
        AccountRequestsService service)
    {
        const AccountRequestStatus status = AccountRequestStatus.Pending;

        accountRequests.ForEach(x =>
        {
            x.IsConfirmed = true;
        });

        context.AccountRequests.RemoveRange(context.AccountRequests);
        await context.SaveChangesAsync();

        context.AccountRequests.AddRange(accountRequests);
        await context.SaveChangesAsync();

        var result = await service.GetAccountRequests(status, new PageOptions("0", 10));

        result.TotalNumberOfRequests.Should().Be(accountRequests.Count);
        result.TotalNumberOfApprovedRequests.Should()
            .Be(accountRequests.Count(x => x.Status == AccountRequestStatus.Approved));
        result.TotalNumberOfRejectedRequests.Should()
            .Be(accountRequests.Count(x => x.Status == AccountRequestStatus.Rejected));
        result.TotalNumberOfPendingRequests.Should()
            .Be(accountRequests.Count(x => x.Status == AccountRequestStatus.Pending));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetAccountRequest_ReturnsAccountRequest(
        List<AccountRequest> accountRequests,
        [Frozen] BuyingCatalogueDbContext context,
        AccountRequestsService service)
    {
        accountRequests.ForEach(x =>
        {
            x.IsConfirmed = true;
        });

        var accountRequest = accountRequests.First();

        context.AccountRequests.RemoveRange(context.AccountRequests);
        await context.SaveChangesAsync();

        context.AccountRequests.AddRange(accountRequests);
        await context.SaveChangesAsync();

        var result = await service.GetAccountRequest(accountRequest.Id);

        result.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static Task ProcessAccountRequest_PendingStatus_ThrowsArgumentException(
        Guid requestId,
        int decidingUserId,
        string justification,
        AccountRequestsService service) => Assert.ThrowsAsync<ArgumentException>(() =>
        service.ProcessAccountRequest(requestId, decidingUserId, AccountRequestStatus.Pending, justification));

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task ProcessAccountRequest_Approved_CreatesUser(
        string justification,
        AspNetUser user,
        List<AccountRequest> accountRequests,
        [Frozen] ICreateUserService createUserService,
        [Frozen] BuyingCatalogueDbContext context,
        AccountRequestsService service)
    {
        accountRequests.ForEach(x =>
        {
            x.IsConfirmed = true;
            x.Status = AccountRequestStatus.Pending;
        });

        createUserService.Create(
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<bool>(),
                Arg.Any<bool>())
            .Returns(user);

        context.AccountRequests.RemoveRange(context.AccountRequests);
        await context.SaveChangesAsync();

        context.AccountRequests.AddRange(accountRequests);
        await context.SaveChangesAsync();

        var accountRequest = accountRequests.First();
        var decidingUser = await context.AspNetUsers.FirstOrDefaultAsync();
        var decidingUserId = decidingUser.Id;

        await service.ProcessAccountRequest(
            accountRequest.Id,
            decidingUserId,
            AccountRequestStatus.Approved,
            justification);

        await createUserService.Received()
            .Create(
                Arg.Any<int>(),
                accountRequest.FirstName,
                accountRequest.LastName,
                accountRequest.Email,
                OrganisationFunction.Buyer.Name,
                false,
                accountRequest.HasOptedInUserResearch);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task ProcessAccountRequest_Rejected_SendsEmail(
        string justification,
        List<AccountRequest> accountRequests,
        [Frozen] ICreateUserService createUserService,
        [Frozen] IGovNotifyEmailService mockEmailService,
        [Frozen] BuyingCatalogueDbContext context,
        AccountRequestsService service)
    {
        accountRequests.ForEach(x =>
        {
            x.IsConfirmed = true;
            x.Status = AccountRequestStatus.Pending;
        });

        context.AccountRequests.RemoveRange(context.AccountRequests);
        await context.SaveChangesAsync();

        context.AccountRequests.AddRange(accountRequests);
        await context.SaveChangesAsync();

        var accountRequest = accountRequests.First();
        var decidingUser = await context.AspNetUsers.FirstOrDefaultAsync();
        var decidingUserId = decidingUser.Id;

        await service.ProcessAccountRequest(
            accountRequest.Id,
            decidingUserId,
            AccountRequestStatus.Rejected,
            justification);

        await createUserService.DidNotReceive()
            .Create(
                Arg.Any<int>(),
                accountRequest.FirstName,
                accountRequest.LastName,
                accountRequest.Email,
                OrganisationFunction.Buyer.Name,
                false,
                accountRequest.HasOptedInUserResearch);

        await mockEmailService.Received()
            .SendEmailAsync(accountRequest.Email, Arg.Any<string>(), Arg.Any<Dictionary<string, dynamic>>());
    }

    [Theory]
    [MockAutoData]
    public static Task SubmitAccountRequest_NullAccountRequest_ThrowsArgumentNullException(
        RoutingResult confirmationRoute,
        AccountRequestsService service) =>
        Assert.ThrowsAsync<ArgumentNullException>(() => service.SubmitAccountRequest(null, confirmationRoute));

    [Theory]
    [MockAutoData]
    public static Task SubmitAccountRequest_NullConfirmationRoute_ThrowsArgumentNullException(
        AccountRequest accountRequest,
        AccountRequestsService service) =>
        Assert.ThrowsAsync<ArgumentNullException>(() => service.SubmitAccountRequest(accountRequest, null));

    [Theory]
    [MockAutoData]
    public static async Task SubmitAccountRequest_EmailDomainInvalid_Returns(
        AccountRequest accountRequest,
        RoutingResult confirmationRoute,
        [Frozen] IEmailDomainService emailDomainService,
        AccountRequestsService service)
    {
        emailDomainService.IsAllowed(Arg.Any<string>()).Returns(false);

        await service.SubmitAccountRequest(accountRequest, confirmationRoute);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SubmitAccountRequest_InvalidOdsCode_SendsInvalidOdsEmail(
        AccountRequest accountRequest,
        RoutingResult confirmationRoute,
        [Frozen] AccountTemplateSettings settings,
        [Frozen] IEmailDomainService emailDomainService,
        [Frozen] IGovNotifyEmailService govNotifyEmailService,
        AccountRequestsService service)
    {
        accountRequest.OdsCode = "INVALID";
        emailDomainService.IsAllowed(Arg.Any<string>()).Returns(true);

        await service.SubmitAccountRequest(accountRequest, confirmationRoute);

        await govNotifyEmailService.Received()
            .SendEmailAsync(
                Arg.Any<string>(),
                settings.InvalidOdsCodeTemplateId,
                Arg.Any<Dictionary<string, dynamic>>());
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SubmitAccountRequest_DuplicateRequest_SendsDuplicateRequestEmail(
        AccountRequest accountRequest,
        RoutingResult confirmationRoute,
        OdsOrganisation odsOrganisation,
        [Frozen] AccountTemplateSettings settings,
        [Frozen] IEmailDomainService emailDomainService,
        [Frozen] IGovNotifyEmailService govNotifyEmailService,
        [Frozen] BuyingCatalogueDbContext context,
        AccountRequestsService service)
    {
        accountRequest.OdsCode = odsOrganisation.Id;

        context.AccountRequests.Add(accountRequest);
        context.OdsOrganisations.Add(odsOrganisation);
        await context.SaveChangesAsync();

        emailDomainService.IsAllowed(Arg.Any<string>()).Returns(true);

        await service.SubmitAccountRequest(accountRequest, confirmationRoute);

        await govNotifyEmailService.Received()
            .SendEmailAsync(
                Arg.Any<string>(),
                settings.AccountDuplicateTemplateId,
                Arg.Any<Dictionary<string, dynamic>>());
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SubmitAccountRequest_DuplicateUser_SendsDuplicateRequestEmail(
        AspNetUser user,
        AccountRequest accountRequest,
        RoutingResult confirmationRoute,
        OdsOrganisation odsOrganisation,
        [Frozen] AccountTemplateSettings settings,
        [Frozen] IEmailDomainService emailDomainService,
        [Frozen] IGovNotifyEmailService govNotifyEmailService,
        [Frozen] BuyingCatalogueDbContext context,
        AccountRequestsService service)
    {
        accountRequest.Email = user.Email;
        accountRequest.OdsCode = odsOrganisation.Id;

        context.AspNetUsers.Add(user);
        context.OdsOrganisations.Add(odsOrganisation);
        await context.SaveChangesAsync();

        emailDomainService.IsAllowed(Arg.Any<string>()).Returns(true);

        await service.SubmitAccountRequest(accountRequest, confirmationRoute);

        await govNotifyEmailService.Received()
            .SendEmailAsync(
                Arg.Any<string>(),
                settings.AccountDuplicateTemplateId,
                Arg.Any<Dictionary<string, dynamic>>());
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SubmitAccountRequest_Valid_SendsRequestSubmittedEmail(
        AccountRequest accountRequest,
        RoutingResult confirmationRoute,
        OdsOrganisation odsOrganisation,
        [Frozen] AccountTemplateSettings settings,
        [Frozen] IEmailDomainService emailDomainService,
        [Frozen] IGovNotifyEmailService govNotifyEmailService,
        [Frozen] BuyingCatalogueDbContext context,
        AccountRequestsService service)
    {
        accountRequest.OdsCode = odsOrganisation.Id;

        context.OdsOrganisations.Add(odsOrganisation);
        await context.SaveChangesAsync();

        emailDomainService.IsAllowed(Arg.Any<string>()).Returns(true);

        await service.SubmitAccountRequest(accountRequest, confirmationRoute);

        await govNotifyEmailService.Received()
            .SendEmailAsync(
                Arg.Any<string>(),
                settings.AccountSubmittedTemplateId,
                Arg.Any<Dictionary<string, dynamic>>());
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task ConfirmAccountRequest_SetsIsConfirmed(
        AccountRequest accountRequest,
        [Frozen] BuyingCatalogueDbContext context,
        AccountRequestsService service)
    {
        accountRequest.IsConfirmed = false;
        context.AccountRequests.Add(accountRequest);
        await context.SaveChangesAsync();

        await service.ConfirmAccountRequest(accountRequest.Id, accountRequest.Email);

        accountRequest.IsConfirmed.Should().BeTrue();
    }
}
