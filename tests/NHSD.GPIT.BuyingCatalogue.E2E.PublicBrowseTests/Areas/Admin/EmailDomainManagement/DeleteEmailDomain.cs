using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.EmailDomainManagement;

[Collection(nameof(AdminCollection))]
public class DeleteEmailDomain : AuthorityTestBase
{
    private const int Id = 1;
    private static readonly Dictionary<string, string> Parameters = new() { { nameof(Id), Id.ToString() }, };

    public DeleteEmailDomain(LocalWebApplicationFactory factory)
    : base(
        factory,
        typeof(EmailDomainManagementController),
        nameof(EmailDomainManagementController.DeleteEmailDomain),
        Parameters)
    {
    }

    [Fact]
    public void AllSectionsDisplayed()
    {
        CommonActions.PageTitle().Should().Be("Delete this email domain?".FormatForComparison());
        CommonActions.LedeText().Should().Be("The email domain @nhs.net will be deleted from the allow list.".FormatForComparison());

        CommonActions.GoBackLinkDisplayed().Should().BeTrue();
        CommonActions.SaveButtonDisplayed().Should().BeTrue();
        CommonActions.CancelLinkDisplayed().Should().BeTrue();
    }

    [Fact]
    public void ClickGoBackLink_NavigatesCorrectly()
    {
        CommonActions.ClickGoBackLink();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(EmailDomainManagementController),
                nameof(EmailDomainManagementController.Index))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void ClickCancel_NavigatesCorrectly()
    {
        CommonActions.ClickCancel();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(EmailDomainManagementController),
                nameof(EmailDomainManagementController.Index))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void ClickSubmit_NavigatesCorrectly()
    {
        var domain = new EmailDomain("@gmail.com");

        using var context = GetEndToEndDbContext();
        context.EmailDomains.Add(domain);
        context.SaveChanges();

        context.EmailDomains.Count().Should().BeGreaterThan(1);

        var parameters = new Dictionary<string, string> { { nameof(domain.Id), domain.Id.ToString() }, };
        NavigateToUrl(
            typeof(EmailDomainManagementController),
            nameof(EmailDomainManagementController.DeleteEmailDomain),
            parameters);

        CommonActions.ClickSave();

        CommonActions.PageLoadedCorrectGetIndex(
                typeof(EmailDomainManagementController),
                nameof(EmailDomainManagementController.Index))
            .Should()
            .BeTrue();

        context.EmailDomains.Count().Should().Be(1);
    }
}
