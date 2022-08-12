using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.EmailDomainManagement;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers;

public static class EmailDomainManagementControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(EmailDomainManagementController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Index_ReturnsViewWithModel(
        List<EmailDomain> emailDomains,
        [Frozen] Mock<IEmailDomainService> service,
        EmailDomainManagementController controller)
    {
        var expectedModel = new ViewEmailDomainsModel(emailDomains);

        service.Setup(s => s.GetAllowedDomains())
            .ReturnsAsync(emailDomains);

        var result = (await controller.Index()).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Get_AddEmailDomain_ReturnsViewWithModel(
        EmailDomainManagementController controller)
    {
        var result = controller.AddEmailDomain().As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(new AddEmailDomainModel(), opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_AddEmailDomain_InvalidModelState(
        AddEmailDomainModel model,
        EmailDomainManagementController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.AddEmailDomain(model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(model);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_AddEmailDomain_Valid(
        AddEmailDomainModel model,
        [Frozen] Mock<IEmailDomainService> service,
        EmailDomainManagementController controller)
    {
        var result = (await controller.AddEmailDomain(model)).As<RedirectToActionResult>();

        service.Verify(s => s.AddAllowedDomain(model.EmailDomain));

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Get_DeleteEmailAddress_IdNotFound(
        int id,
        [Frozen] Mock<IEmailDomainService> service,
        EmailDomainManagementController controller)
    {
        service.Setup(s => s.GetAllowedDomain(id)).ReturnsAsync((EmailDomain)null);

        var result = (await controller.DeleteEmailDomain(id)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Get_DeleteEmailDomain_ReturnsViewWithModel(
        int id,
        EmailDomain emailDomain,
        [Frozen] Mock<IEmailDomainService> service,
        EmailDomainManagementController controller)
    {
        var expectedModel = new DeleteEmailDomainModel(emailDomain);

        service.Setup(s => s.GetAllowedDomain(id))
            .ReturnsAsync(emailDomain);

        var result = (await controller.DeleteEmailDomain(id)).As<ViewResult>();

        service.VerifyAll();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_DeleteEmailAddress_Redirects(
        int id,
        DeleteEmailDomainModel model,
        [Frozen] Mock<IEmailDomainService> service,
        EmailDomainManagementController controller)
    {
        var result = (await controller.DeleteEmailDomain(id, model)).As<RedirectToActionResult>();

        service.Verify(s => s.DeleteAllowedDomain(id));

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }
}
