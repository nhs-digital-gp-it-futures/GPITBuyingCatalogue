using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.FrameworkModels;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers;

public static class FrameworksControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(FrameworksController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task Dashboard_ReturnsViewWithModel(
        List<EntityFramework.Catalogue.Models.Framework> frameworks,
        [Frozen] IFrameworkService frameworkService,
        FrameworksController controller)
    {
        var expectedModel = new FrameworksDashboardModel(frameworks);

        frameworkService
            .GetFrameworks()
            .Returns(frameworks);

        var result = (await controller.Dashboard()).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [MockAutoData]
    public static void Add_ReturnsViewWithModel(
        FrameworksController controller)
    {
        var result = controller.Add().As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeOfType<AddEditFrameworkModel>();
    }

    [Theory]
    [MockAutoData]
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
    [MockAutoData]
    public static async Task Add_Valid_AddsFrameworkAndRedirects(
        AddEditFrameworkModel model,
        [Frozen] IFrameworkService service,
        FrameworksController controller)
    {
        model.MaximumTerm = "36";

        var result = (await controller.Add(model)).As<RedirectToActionResult>();

        await service
            .Received()
            .AddFramework(model.Name, Arg.Any<IEnumerable<FundingType>>(), Arg.Any<int>());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Dashboard));
    }

    [Theory]
    [MockAutoData]
    public static async Task Edit_ReturnsViewWithModel(
        string frameworkId,
        EntityFramework.Catalogue.Models.Framework framework,
        [Frozen] IFrameworkService service,
        FrameworksController controller)
    {
        service
            .GetFramework(frameworkId)
            .Returns(framework);

        var result = (await controller.Edit(frameworkId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeOfType<AddEditFrameworkModel>();
    }

    [Theory]
    [MockAutoData]
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
    [MockAutoData]
    public static async Task Edit_Valid_AddsFrameworkAndRedirects(
        string frameworkId,
        AddEditFrameworkModel model,
        [Frozen] IFrameworkService service,
        FrameworksController controller)
    {
        model.MaximumTerm = "36";

        var result = (await controller.Edit(frameworkId, model)).As<RedirectToActionResult>();

        await service
            .Received()
            .UpdateFramework(frameworkId, model.Name, Arg.Any<IEnumerable<FundingType>>(), Arg.Any<int>());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Dashboard));
    }

    [Theory]
    [MockAutoData]
    public static async Task Expire_InvalidFramework_Redirects(
        string frameworkId,
        [Frozen] IFrameworkService frameworkService,
        FrameworksController controller)
    {
        frameworkService
            .GetFramework(frameworkId)
            .Returns((EntityFramework.Catalogue.Models.Framework)null);

        var result = (await controller.Expire(frameworkId)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Dashboard));
    }

    [Theory]
    [MockAutoData]
    public static async Task Expire_ValidFramework_ReturnsViewWithModel(
        EntityFramework.Catalogue.Models.Framework framework,
        [Frozen] IFrameworkService frameworkService,
        FrameworksController controller)
    {
        frameworkService
            .GetFramework(framework.Id)
            .Returns(framework);

        var result = (await controller.Expire(framework.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task Expire_MarksAsExpired_Redirects(
        string frameworkId,
        ExpireFrameworkModel model,
        [Frozen] IFrameworkService service,
        FrameworksController controller)
    {
        var result = (await controller.Expire(frameworkId, model)).As<RedirectToActionResult>();

        await service
            .Received()
            .MarkAsExpired(Arg.Any<string>());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Dashboard));
    }
}
