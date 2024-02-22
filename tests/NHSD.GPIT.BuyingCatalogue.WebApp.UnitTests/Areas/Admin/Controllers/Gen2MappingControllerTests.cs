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
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilitiesMappingModels;
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
    [CommonAutoData]
    public static void Capabilities_ReturnsView(
        Gen2MappingController controller)
    {
        var result = controller.Capabilities().As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(new Gen2UploadModel());
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_Capabilities_InvalidModel_ReturnsView(
        Gen2UploadModel model,
        Gen2MappingController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.Capabilities(model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_Capabilities_NullImport_SetsModelError(
        Gen2UploadModel model,
        [Frozen] Mock<IGen2UploadService> service,
        Gen2MappingController controller)
    {
        service.Setup(x => x.GetCapabilitiesFromCsv(It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync((Gen2CsvImportModel<Gen2CapabilitiesCsvModel>)null);

        var result = (await controller.Capabilities(model)).As<ViewResult>();

        controller.ModelState.Keys.Should().Contain(nameof(model.File));
        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_Capabilities_EmptyImport_SetsModelError(
        Gen2UploadModel model,
        Gen2CsvImportModel<Gen2CapabilitiesCsvModel> capabilities,
        [Frozen] Mock<IGen2UploadService> service,
        Gen2MappingController controller)
    {
        capabilities.Failed = capabilities.Imported = Enumerable.Empty<Gen2CapabilitiesCsvModel>().ToList();

        service.Setup(x => x.GetCapabilitiesFromCsv(It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync(capabilities);

        var result = (await controller.Capabilities(model)).As<ViewResult>();

        controller.ModelState.Keys.Should().Contain(nameof(model.File));
        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_Capabilities_FailedEntries_ReturnsExpectedView(
        Gen2UploadModel model,
        List<Gen2CapabilitiesCsvModel> failed,
        Gen2CsvImportModel<Gen2CapabilitiesCsvModel> capabilities,
        [Frozen] Mock<IGen2UploadService> service,
        Gen2MappingController controller)
    {
        var id = Guid.NewGuid();

        capabilities.Failed = failed;

        service.Setup(x => x.GetCapabilitiesFromCsv(It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync(capabilities);

        service.Setup(x => x.AddToCache(capabilities))
            .ReturnsAsync(id);

        var result = (await controller.Capabilities(model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.FailedCapabilities));
        result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { nameof(id), id }, });
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_Capabilities_NoFailedEntries_ReturnsExpectedView(
        Gen2UploadModel model,
        List<Gen2CapabilitiesCsvModel> imported,
        Gen2CsvImportModel<Gen2CapabilitiesCsvModel> capabilities,
        [Frozen] Mock<IGen2UploadService> service,
        Gen2MappingController controller)
    {
        var id = Guid.NewGuid();

        capabilities.Imported = imported;
        capabilities.Failed = Enumerable.Empty<Gen2CapabilitiesCsvModel>().ToList();

        service.Setup(x => x.GetCapabilitiesFromCsv(It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync(capabilities);

        service.Setup(x => x.AddToCache(capabilities))
            .ReturnsAsync(id);

        var result = (await controller.Capabilities(model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Epics));
        result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { nameof(id), id }, });
    }

    [Theory]
    [CommonAutoData]
    public static async Task FailedCapabilities_ReturnsViewWithModel(
        Guid id,
        Gen2CsvImportModel<Gen2CapabilitiesCsvModel> import,
        [Frozen] Mock<IGen2UploadService> service,
        Gen2MappingController controller)
    {
        var expected = new FailedGen2UploadModel<Gen2CapabilitiesCsvModel>(import.FileName, import.Failed);

        service.Setup(x => x.GetCachedCapabilities(id))
            .ReturnsAsync(import);

        var result = (await controller.FailedCapabilities(id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_FailedCapabilities_ReturnsFile(
        Guid id,
        FailedGen2UploadModel<Gen2CapabilitiesCsvModel> model,
        Gen2CsvImportModel<Gen2CapabilitiesCsvModel> import,
        Stream stream,
        [Frozen] Mock<IGen2UploadService> service,
        Gen2MappingController controller)
    {
        service.Setup(x => x.GetCachedCapabilities(id))
            .ReturnsAsync(import);

        service.Setup(x => x.WriteToCsv(import.Failed))
            .ReturnsAsync(stream);

        var result = (await controller.FailedCapabilities(id, model)).As<FileStreamResult>();

        result.Should().NotBeNull();
        result.FileStream.Should().BeSameAs(stream);
        result.ContentType.Should().Be("text/csv");
    }

    [Theory]
    [CommonAutoData]
    public static void Epics_ReturnsView(
        Guid id,
        Gen2MappingController controller)
    {
        var result = controller.Epics(id).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(new Gen2UploadModel());
    }

    [Theory]
    [CommonAutoData]
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
    [CommonAutoData]
    public static async Task Post_Epics_NullImport_SetsModelError(
        Guid id,
        Gen2UploadModel model,
        [Frozen] Mock<IGen2UploadService> service,
        Gen2MappingController controller)
    {
        service.Setup(x => x.GetEpicsFromCsv(It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync((Gen2CsvImportModel<Gen2EpicsCsvModel>)null);

        var result = (await controller.Epics(id, model)).As<ViewResult>();

        controller.ModelState.Keys.Should().Contain(nameof(model.File));
        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_Epics_EmptyImport_SetsModelError(
        Guid id,
        Gen2UploadModel model,
        Gen2CsvImportModel<Gen2EpicsCsvModel> capabilities,
        [Frozen] Mock<IGen2UploadService> service,
        Gen2MappingController controller)
    {
        capabilities.Failed = capabilities.Imported = Enumerable.Empty<Gen2EpicsCsvModel>().ToList();

        service.Setup(x => x.GetEpicsFromCsv(It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync(capabilities);

        var result = (await controller.Epics(id, model)).As<ViewResult>();

        controller.ModelState.Keys.Should().Contain(nameof(model.File));
        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_Epics_FailedEntries_ReturnsExpectedView(
        Guid id,
        Gen2UploadModel model,
        List<Gen2EpicsCsvModel> failed,
        Gen2CsvImportModel<Gen2EpicsCsvModel> capabilities,
        [Frozen] Mock<IGen2UploadService> service,
        Gen2MappingController controller)
    {
        capabilities.Failed = failed;

        service.Setup(x => x.GetEpicsFromCsv(It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync(capabilities);

        service.Setup(x => x.AddToCache(capabilities))
            .ReturnsAsync(id);

        var result = (await controller.Epics(id, model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.FailedEpics));
        result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { nameof(id), id }, });
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_Epics_NoFailedEntries_ReturnsExpectedView(
        Guid id,
        Gen2UploadModel model,
        List<Gen2EpicsCsvModel> imported,
        Gen2CsvImportModel<Gen2EpicsCsvModel> capabilities,
        [Frozen] Mock<IGen2UploadService> service,
        Gen2MappingController controller)
    {
        capabilities.Imported = imported;
        capabilities.Failed = Enumerable.Empty<Gen2EpicsCsvModel>().ToList();

        service.Setup(x => x.GetEpicsFromCsv(It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync(capabilities);

        service.Setup(x => x.AddToCache(capabilities))
            .ReturnsAsync(id);

        var result = (await controller.Epics(id, model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Epics));
        result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { nameof(id), id }, });
    }

    [Theory]
    [CommonAutoData]
    public static async Task FailedEpics_ReturnsViewWithModel(
        Guid id,
        Gen2CsvImportModel<Gen2EpicsCsvModel> import,
        [Frozen] Mock<IGen2UploadService> service,
        Gen2MappingController controller)
    {
        var expected = new FailedGen2UploadModel<Gen2EpicsCsvModel>(import.FileName, import.Failed);

        service.Setup(x => x.GetCachedEpics(id))
            .ReturnsAsync(import);

        var result = (await controller.FailedEpics(id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_FailedEpics_ReturnsFile(
        Guid id,
        FailedGen2UploadModel<Gen2EpicsCsvModel> model,
        Gen2CsvImportModel<Gen2EpicsCsvModel> import,
        Stream stream,
        [Frozen] Mock<IGen2UploadService> service,
        Gen2MappingController controller)
    {
        service.Setup(x => x.GetCachedEpics(id))
            .ReturnsAsync(import);

        service.Setup(x => x.WriteToCsv(import.Failed))
            .ReturnsAsync(stream);

        var result = (await controller.FailedEpics(id, model)).As<FileStreamResult>();

        result.Should().NotBeNull();
        result.FileStream.Should().BeSameAs(stream);
        result.ContentType.Should().Be("text/csv");
    }
}
