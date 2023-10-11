using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.FrameworkModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers;

public static class FrameworksControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(FrameworksController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Dashboard_ReturnsViewWithModel(
        List<EntityFramework.Catalogue.Models.Framework> frameworks,
        [Frozen] Mock<IFrameworkService> frameworkService,
        FrameworksController controller)
    {
        var expectedModel = new FrameworksDashboardModel(frameworks);

        frameworkService
            .Setup(x => x.GetFrameworks())
            .ReturnsAsync(frameworks);

        var result = (await controller.Dashboard()).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void Add_ReturnsViewWithModel(
        FrameworksController controller)
    {
        var result = controller.Add().As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeOfType<AddEditFrameworkModel>();
    }

    [Theory]
    [CommonAutoData]
    public static async Task Add_InvalidModel_ReturnsView(
        AddEditFrameworkModel model,
        FrameworksController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.Add(model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Add_Valid_AddsFrameworkAndRedirects(
        AddEditFrameworkModel model,
        [Frozen] Mock<IFrameworkService> service,
        FrameworksController controller)
    {
        var result = (await controller.Add(model)).As<RedirectToActionResult>();

        service.Verify(x => x.AddFramework(model.Name, It.IsAny<IEnumerable<FundingType>>()), Times.Once());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Dashboard));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Edit_ReturnsViewWithModel(
        string frameworkId,
        EntityFramework.Catalogue.Models.Framework framework,
        [Frozen] Mock<IFrameworkService> service,
        FrameworksController controller)
    {
        service
            .Setup(x => x.GetFramework(frameworkId))
            .ReturnsAsync(framework);

        var result = (await controller.Edit(frameworkId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeOfType<AddEditFrameworkModel>();
    }

    [Theory]
    [CommonAutoData]
    public static async Task Edit_InvalidModel_ReturnsView(
        string frameworkId,
        AddEditFrameworkModel model,
        FrameworksController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.Edit(frameworkId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Edit_Valid_AddsFrameworkAndRedirects(
        string frameworkId,
        AddEditFrameworkModel model,
        [Frozen] Mock<IFrameworkService> service,
        FrameworksController controller)
    {
        var result = (await controller.Edit(frameworkId, model)).As<RedirectToActionResult>();

        service.Verify(x => x.UpdateFramework(frameworkId, model.Name, It.IsAny<IEnumerable<FundingType>>()), Times.Once());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Dashboard));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Expire_InvalidFramework_Redirects(
        string frameworkId,
        [Frozen] Mock<IFrameworkService> frameworkService,
        FrameworksController controller)
    {
        frameworkService
            .Setup(x => x.GetFramework(frameworkId))
            .ReturnsAsync((EntityFramework.Catalogue.Models.Framework)null);

        var result = (await controller.Expire(frameworkId)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Dashboard));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Expire_ValidFramework_ReturnsViewWithModel(
        EntityFramework.Catalogue.Models.Framework framework,
        [Frozen] Mock<IFrameworkService> frameworkService,
        FrameworksController controller)
    {
        frameworkService
            .Setup(x => x.GetFramework(framework.Id))
            .ReturnsAsync(framework);

        var result = (await controller.Expire(framework.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task Expire_MarksAsExpired_Redirects(
        string frameworkId,
        ExpireFrameworkModel model,
        [Frozen] Mock<IFrameworkService> service,
        FrameworksController controller)
    {
        var result = (await controller.Expire(frameworkId, model)).As<RedirectToActionResult>();

        service.Verify(x => x.MarkAsExpired(It.IsAny<string>()), Times.Once());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Dashboard));
    }
}
