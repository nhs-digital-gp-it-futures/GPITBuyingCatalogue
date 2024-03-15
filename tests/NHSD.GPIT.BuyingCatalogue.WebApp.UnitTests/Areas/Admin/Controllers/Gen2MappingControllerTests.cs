using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilitiesMappingModels;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers;

public static class Gen2MappingControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(Gen2MappingController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static void Capabilities_ReturnsView(
        Guid id,
        Gen2MappingController controller)
    {
        var result = controller.Capabilities(id).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(new Gen2UploadModel());
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_Capabilities_InvalidModel_ReturnsView(
        Guid id,
        Gen2UploadModel model,
        Gen2MappingController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.Capabilities(id, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_Capabilities_NullImport_SetsModelError(
        Guid id,
        Gen2UploadModel model,
        [Frozen] IGen2UploadService service,
        Gen2MappingController controller)
    {
        service.GetCapabilitiesFromCsv(Arg.Any<Stream>()).Returns((Gen2CsvImportModel<Gen2CapabilitiesCsvModel>)null);

        var result = (await controller.Capabilities(id, model)).As<ViewResult>();

        controller.ModelState.Keys.Should().Contain(nameof(model.File));
        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_Capabilities_EmptyImport_SetsModelError(
        Guid id,
        Gen2UploadModel model,
        Gen2CsvImportModel<Gen2CapabilitiesCsvModel> capabilities,
        [Frozen] IGen2UploadService service,
        Gen2MappingController controller)
    {
        capabilities.Failed = capabilities.Imported = Enumerable.Empty<Gen2CapabilitiesCsvModel>().ToList();

        service.GetCapabilitiesFromCsv(Arg.Any<Stream>()).Returns(capabilities);

        var result = (await controller.Capabilities(id, model)).As<ViewResult>();

        controller.ModelState.Keys.Should().Contain(nameof(model.File));
        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_Capabilities_FailedEntries_ReturnsExpectedView(
        Guid id,
        Gen2UploadModel model,
        List<Gen2CapabilitiesCsvModel> failed,
        Gen2CsvImportModel<Gen2CapabilitiesCsvModel> capabilities,
        [Frozen] IGen2UploadService service,
        Gen2MappingController controller)
    {
        capabilities.Failed = failed;

        service.GetCapabilitiesFromCsv(Arg.Any<Stream>()).Returns(capabilities);

        var result = (await controller.Capabilities(id, model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.FailedCapabilities));
        result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { nameof(id), id }, });
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_Capabilities_NoFailedEntries_ReturnsExpectedView(
        Guid id,
        Gen2UploadModel model,
        List<Gen2CapabilitiesCsvModel> imported,
        Gen2CsvImportModel<Gen2CapabilitiesCsvModel> capabilities,
        [Frozen] IGen2UploadService service,
        Gen2MappingController controller)
    {
        capabilities.Imported = imported;
        capabilities.Failed = Enumerable.Empty<Gen2CapabilitiesCsvModel>().ToList();

        service.GetCapabilitiesFromCsv(Arg.Any<Stream>()).Returns(capabilities);

        var result = (await controller.Capabilities(id, model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Epics));
        result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { nameof(id), id }, });
    }

    [Theory]
    [MockAutoData]
    public static async Task FailedCapabilities_ReturnsViewWithModel(
        Guid id,
        Gen2CsvImportModel<Gen2CapabilitiesCsvModel> import,
        [Frozen] IGen2UploadService service,
        Gen2MappingController controller)
    {
        var expected = new FailedGen2UploadModel<Gen2CapabilitiesCsvModel>(import.Failed);

        service.GetCachedCapabilities(id).Returns(import);

        var result = (await controller.FailedCapabilities(id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_FailedCapabilities_ReturnsFile(
        Guid id,
        FailedGen2UploadModel<Gen2CapabilitiesCsvModel> model,
        Gen2CsvImportModel<Gen2CapabilitiesCsvModel> import,
        Stream stream,
        [Frozen] IGen2UploadService service,
        Gen2MappingController controller)
    {
        service.GetCachedCapabilities(id).Returns(import);

        service.WriteToCsv(import.Failed).Returns(stream);

        var result = (await controller.FailedCapabilities(id, model)).As<FileStreamResult>();

        result.Should().NotBeNull();
        result.FileStream.Should().BeSameAs(stream);
        result.ContentType.Should().Be("text/csv");
    }

    [Theory]
    [MockAutoData]
    public static void Epics_ReturnsView(
        Guid id,
        Gen2MappingController controller)
    {
        var result = controller.Epics(id).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(new Gen2UploadModel());
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_Epics_InvalidModel_ReturnsView(
        Guid id,
        Gen2UploadModel model,
        Gen2MappingController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.Epics(id, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_Epics_NullImport_SetsModelError(
        Guid id,
        Gen2UploadModel model,
        [Frozen] IGen2UploadService service,
        Gen2MappingController controller)
    {
        service.GetEpicsFromCsv(Arg.Any<Stream>()).Returns((Gen2CsvImportModel<Gen2EpicsCsvModel>)null);

        var result = (await controller.Epics(id, model)).As<ViewResult>();

        controller.ModelState.Keys.Should().Contain(nameof(model.File));
        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_Epics_EmptyImport_SetsModelError(
        Guid id,
        Gen2UploadModel model,
        Gen2CsvImportModel<Gen2EpicsCsvModel> capabilities,
        [Frozen] IGen2UploadService service,
        Gen2MappingController controller)
    {
        capabilities.Failed = capabilities.Imported = Enumerable.Empty<Gen2EpicsCsvModel>().ToList();

        service.GetEpicsFromCsv(Arg.Any<Stream>()).Returns(capabilities);

        var result = (await controller.Epics(id, model)).As<ViewResult>();

        controller.ModelState.Keys.Should().Contain(nameof(model.File));
        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_Epics_FailedEntries_ReturnsExpectedView(
        Guid id,
        Gen2UploadModel model,
        List<Gen2EpicsCsvModel> failed,
        Gen2CsvImportModel<Gen2EpicsCsvModel> capabilities,
        [Frozen] IGen2UploadService service,
        Gen2MappingController controller)
    {
        capabilities.Failed = failed;

        service.GetEpicsFromCsv(Arg.Any<Stream>()).Returns(capabilities);

        var result = (await controller.Epics(id, model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.FailedEpics));
        result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { nameof(id), id }, });
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_Epics_NoFailedEntries_ReturnsExpectedView(
        Guid id,
        Gen2UploadModel model,
        List<Gen2EpicsCsvModel> imported,
        Gen2CsvImportModel<Gen2EpicsCsvModel> capabilities,
        [Frozen] IGen2UploadService service,
        Gen2MappingController controller)
    {
        capabilities.Imported = imported;
        capabilities.Failed = Enumerable.Empty<Gen2EpicsCsvModel>().ToList();

        service.GetEpicsFromCsv(Arg.Any<Stream>()).Returns(capabilities);

        var result = (await controller.Epics(id, model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Mapping));
        result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { nameof(id), id }, });
    }

    [Theory]
    [MockAutoData]
    public static async Task FailedEpics_ReturnsViewWithModel(
        Guid id,
        Gen2CsvImportModel<Gen2EpicsCsvModel> import,
        [Frozen] IGen2UploadService service,
        Gen2MappingController controller)
    {
        var expected = new FailedGen2UploadModel<Gen2EpicsCsvModel>(import.Failed);

        service.GetCachedEpics(id).Returns(import);

        var result = (await controller.FailedEpics(id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_FailedEpics_ReturnsFile(
        Guid id,
        FailedGen2UploadModel<Gen2EpicsCsvModel> model,
        Gen2CsvImportModel<Gen2EpicsCsvModel> import,
        Stream stream,
        [Frozen] IGen2UploadService service,
        Gen2MappingController controller)
    {
        service.GetCachedEpics(id).Returns(import);

        service.WriteToCsv(import.Failed).Returns(stream);

        var result = (await controller.FailedEpics(id, model)).As<FileStreamResult>();

        result.Should().NotBeNull();
        result.FileStream.Should().BeSameAs(stream);
        result.ContentType.Should().Be("text/csv");
    }

    [Theory]
    [MockAutoData]
    public static async Task Mapping_NullCapabilities_Redirects(
        Guid id,
        [Frozen] IGen2UploadService uploadService,
        Gen2MappingController controller)
    {
        uploadService.GetCachedCapabilities(id).Returns((Gen2CsvImportModel<Gen2CapabilitiesCsvModel>)null);

        var result = (await controller.Mapping(id)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(HomeController.Index));
    }

    [Theory]
    [MockAutoData]
    public static async Task Mapping_NullEpics_Redirects(
        Guid id,
        Gen2CsvImportModel<Gen2CapabilitiesCsvModel> capabilities,
        [Frozen] IGen2UploadService uploadService,
        Gen2MappingController controller)
    {
        uploadService.GetCachedCapabilities(id).Returns(capabilities);
        uploadService.GetCachedEpics(id).Returns((Gen2CsvImportModel<Gen2EpicsCsvModel>)null);

        var result = (await controller.Mapping(id)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(HomeController.Index));
    }

    [Theory]
    [MockAutoData]
    public static async Task Mapping_WithCached_ReturnsView(
        Guid id,
        Gen2CsvImportModel<Gen2CapabilitiesCsvModel> capabilities,
        Gen2CsvImportModel<Gen2EpicsCsvModel> epics,
        [Frozen] IGen2UploadService uploadService,
        [Frozen] IGen2MappingService mappingService,
        Gen2MappingController controller)
    {
        uploadService.GetCachedCapabilities(id).Returns(capabilities);
        uploadService.GetCachedEpics(id).Returns(epics);

        mappingService.MapToSolutions(Arg.Any<Gen2MappingModel>()).Returns(true);

        var result = (await controller.Mapping(id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(new ConfirmationModel(true));
    }
}
