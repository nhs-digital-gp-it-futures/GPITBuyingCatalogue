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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CatalogueItems;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.ImportServiceRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.ServiceRecipients;
using Xunit;
using ServiceRecipient = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceRecipient;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection;

public static class ImportServiceRecipientsControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(ImportServiceRecipientsController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Index_ReturnsViewWithModel(
        string catalogueItemName,
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        ServiceRecipientImportMode importMode,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        [Frozen] Mock<ICatalogueItemService> catalogueItemService,
        ImportServiceRecipientsController controller)
    {
        catalogueItemService.Setup(s => s.GetCatalogueItemName(catalogueItemId))
            .ReturnsAsync(catalogueItemName);

        var expectedModel = new ImportServiceRecipientModel(
            internalOrgId,
            callOffId,
            catalogueItemId,
            catalogueItemName);

        var result = (await controller.Index(internalOrgId, callOffId, catalogueItemId, importMode)).As<ViewResult>();

        importService.VerifyAll();
        catalogueItemService.VerifyAll();

        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt
                    .Excluding(m => m.BackLink)
                    .Excluding(m => m.File));
    }

    [Theory]
    [CommonMemberAutoData(nameof(InvalidServiceRecipientsTestData))]
    public static async Task Index_InvalidRecipients_SetsModelError(
        string expectedErrorMessage,
        IList<ServiceRecipientImportModel> importedServiceRecipients,
        ImportServiceRecipientModel model,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        ImportServiceRecipientsController controller)
    {
        importService.Setup(s => s.ReadFromStream(It.IsAny<Stream>()))
            .ReturnsAsync(importedServiceRecipients);

        _ = await controller.Index(model.InternalOrgId, model.CallOffId, model.CatalogueItemId, model);

        importService.VerifyAll();

        controller.ModelState.Should().ContainKey(nameof(model.File));
        controller.ModelState.Should()
            .Contain(
                m => m.Value.Errors.Any(
                    x => string.Equals(x.ErrorMessage, expectedErrorMessage)));
        controller.ModelState.Clear();
    }

    [Theory]
    [CommonAutoData]
    public static async Task Index_ValidRecipients_Redirects(
        ImportServiceRecipientModel model,
        ServiceRecipientImportMode importMode,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        ImportServiceRecipientsController controller)
    {
        importService.Setup(s => s.ReadFromStream(It.IsAny<Stream>()))
            .ReturnsAsync(
                new List<ServiceRecipientImportModel> { new() { Organisation = "Fake Org", OdsCode = "ABC123" } });

        var result = (await controller.Index(
                model.InternalOrgId,
                model.CallOffId,
                model.CatalogueItemId,
                model,
                importMode))
            .As<RedirectToActionResult>();

        importService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ValidateOds));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(model.InternalOrgId), model.InternalOrgId },
                    { nameof(model.CallOffId), model.CallOffId },
                    { nameof(model.CatalogueItemId), model.CatalogueItemId },
                    { nameof(importMode), importMode },
                });
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateOds_CachedRecipientsNull_Redirects(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        ServiceRecipientImportMode importMode,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        ImportServiceRecipientsController controller)
    {
        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .Returns((IList<ServiceRecipientImportModel>)null);

        var result = (await controller.ValidateOds(internalOrgId, callOffId, catalogueItemId, importMode))
            .As<RedirectToActionResult>();

        importService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(callOffId), callOffId },
                    { nameof(catalogueItemId), catalogueItemId },
                    { nameof(importMode), importMode },
                });
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateOds_MismatchedOdsCodes_ReturnsViewWithModel(
        string catalogueItemName,
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        ServiceRecipientImportMode importMode,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        [Frozen] Mock<ICatalogueItemService> catalogueItemService,
        [Frozen] Mock<IOdsService> odsService,
        ImportServiceRecipientsController controller)
    {
        var importedServiceRecipients = serviceRecipients.Take(2)
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId, })
            .ToList();
        importedServiceRecipients.First().OdsCode = "MISMATCH";

        var expectedModel = new ValidateOdsModel(
            internalOrgId,
            callOffId,
            catalogueItemId,
            catalogueItemName,
            importMode,
            importedServiceRecipients.Take(1).ToList());

        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .Returns(importedServiceRecipients);

        catalogueItemService.Setup(s => s.GetCatalogueItemName(catalogueItemId))
            .ReturnsAsync(catalogueItemName);

        odsService.Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
            .ReturnsAsync(serviceRecipients);

        var result = (await controller.ValidateOds(internalOrgId, callOffId, catalogueItemId, importMode))
            .As<ViewResult>();

        importService.VerifyAll();
        catalogueItemService.VerifyAll();
        odsService.VerifyAll();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateOds_ValidOdsCodes_Redirects(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        ServiceRecipientImportMode importMode,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        [Frozen] Mock<IOdsService> odsService,
        ImportServiceRecipientsController controller)
    {
        var importedServiceRecipients = serviceRecipients.Take(2)
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId, })
            .ToList();

        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .Returns(importedServiceRecipients);

        odsService.Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
            .ReturnsAsync(serviceRecipients);

        var result = (await controller.ValidateOds(internalOrgId, callOffId, catalogueItemId, importMode))
            .As<RedirectToActionResult>();

        importService.VerifyAll();
        odsService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ValidateNames));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(callOffId), callOffId },
                    { nameof(catalogueItemId), catalogueItemId },
                    { nameof(importMode), importMode },
                });
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateNames_CachedRecipientsNull_Redirects(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        ServiceRecipientImportMode importMode,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        ImportServiceRecipientsController controller)
    {
        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .Returns((IList<ServiceRecipientImportModel>)null);

        var result = (await controller.ValidateNames(internalOrgId, callOffId, catalogueItemId, importMode))
            .As<RedirectToActionResult>();

        importService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(callOffId), callOffId },
                    { nameof(catalogueItemId), catalogueItemId },
                    { nameof(importMode), importMode },
                });
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateNames_MismatchedNames_ReturnsViewWithModel(
        string catalogueItemName,
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        ServiceRecipientImportMode importMode,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        [Frozen] Mock<ICatalogueItemService> catalogueItemService,
        [Frozen] Mock<IOdsService> odsService,
        ImportServiceRecipientsController controller)
    {
        var importedServiceRecipients = serviceRecipients.Take(2)
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId, })
            .ToList();
        importedServiceRecipients.First().Organisation = "MISMATCH";

        var expectedModel = new ValidateNamesModel(
            internalOrgId,
            callOffId,
            catalogueItemId,
            catalogueItemName,
            importMode,
            importedServiceRecipients,
            serviceRecipients.Take(1).ToList());

        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .Returns(importedServiceRecipients);

        catalogueItemService.Setup(s => s.GetCatalogueItemName(catalogueItemId))
            .ReturnsAsync(catalogueItemName);

        odsService.Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
            .ReturnsAsync(serviceRecipients);

        var result = (await controller.ValidateNames(internalOrgId, callOffId, catalogueItemId, importMode))
            .As<ViewResult>();

        importService.VerifyAll();
        catalogueItemService.VerifyAll();
        odsService.VerifyAll();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateNames_ValidNames_Redirects(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        [Frozen] Mock<IOdsService> odsService,
        ImportServiceRecipientsController controller)
    {
        const ServiceRecipientImportMode importMode = ServiceRecipientImportMode.Edit;

        var importedRecipients = serviceRecipients.Take(2)
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId, })
            .ToList();

        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .Returns(importedRecipients);

        odsService.Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
            .ReturnsAsync(serviceRecipients);

        var result = (await controller.ValidateNames(internalOrgId, callOffId, catalogueItemId, importMode))
            .As<RedirectToActionResult>();

        importService.VerifyAll();
        odsService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(ServiceRecipientsController.EditServiceRecipients));
        result.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(callOffId), callOffId },
                    { nameof(catalogueItemId), catalogueItemId },
                    { nameof(importedRecipients), importedRecipients.Select(s => s.OdsCode).ToArray() },
                });
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateNames_Post_Redirects(
        string catalogueItemName,
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        [Frozen] Mock<ICatalogueItemService> catalogueItemService,
        [Frozen] Mock<IOdsService> odsService,
        ImportServiceRecipientsController controller)
    {
        const ServiceRecipientImportMode importMode = ServiceRecipientImportMode.Edit;

        var importedRecipients = serviceRecipients
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId, })
            .ToList();

        importedRecipients.First().Organisation = "MISMATCH";
        importedRecipients.Skip(1).First().OdsCode = "MISMATCH";

        var mismatchedRecipients = serviceRecipients.Where(
                r => importedRecipients.Any(
                    x => string.Equals(r.OrgId, x.OdsCode, StringComparison.OrdinalIgnoreCase) && !string.Equals(
                        r.Name,
                        x.Organisation,
                        StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var model = new ValidateNamesModel(
            internalOrgId,
            callOffId,
            catalogueItemId,
            catalogueItemName,
            importMode,
            importedRecipients,
            mismatchedRecipients);

        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .Returns(importedRecipients);

        catalogueItemService.Setup(s => s.GetCatalogueItemName(catalogueItemId))
            .ReturnsAsync(catalogueItemName);

        odsService.Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
            .ReturnsAsync(serviceRecipients);

        var result = (await controller.ValidateNames(internalOrgId, callOffId, catalogueItemId, model, importMode))
            .As<RedirectToActionResult>();

        importService.VerifyAll();
        odsService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(ServiceRecipientsController.EditServiceRecipients));
        result.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(callOffId), callOffId },
                    { nameof(catalogueItemId), catalogueItemId },
                    { nameof(importedRecipients), importedRecipients.Skip(2).Select(x => x.OdsCode).ToArray() },
                });
    }

    [Theory]
    [CommonInlineAutoData(ServiceRecipientImportMode.Edit, nameof(ServiceRecipientsController.EditServiceRecipients))]
    [CommonInlineAutoData(ServiceRecipientImportMode.Add, nameof(ServiceRecipientsController.AddServiceRecipients))]
    public static void CancelImport_Redirects(
        ServiceRecipientImportMode importMode,
        string expectedRedirectAction,
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        ImportServiceRecipientsController controller)
    {
        var result = controller.CancelImport(internalOrgId, callOffId, catalogueItemId, importMode)
            .As<RedirectToActionResult>();

        importService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(expectedRedirectAction);
        result.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(callOffId), callOffId },
                    { nameof(catalogueItemId), catalogueItemId },
                });
    }

    public static IEnumerable<object[]> InvalidServiceRecipientsTestData()
        => new[]
        {
            new object[] { ImportServiceRecipientsController.InvalidFormat, null, },
            new object[] { ImportServiceRecipientsController.EmptyFile, new List<ServiceRecipientImportModel>() },
            new object[]
            {
                ImportServiceRecipientsController.InvalidFormat,
                new List<ServiceRecipientImportModel>
                {
                    new() { Organisation = string.Empty, OdsCode = "ABC123", },
                },
            },
            new object[]
            {
                ImportServiceRecipientsController.InvalidFormat,
                new List<ServiceRecipientImportModel>
                {
                    new() { Organisation = "Fake Org", OdsCode = string.Empty },
                },
            },
            new object[]
            {
                ImportServiceRecipientsController.OdsCodeExceedsLimit,
                new List<ServiceRecipientImportModel>
                {
                    new() { Organisation = "Fake Org", OdsCode = new('A', 10) },
                },
            },
            new object[]
            {
                ImportServiceRecipientsController.OrganisationExceedsLimit,
                new List<ServiceRecipientImportModel>
                {
                    new() { Organisation = new('A', 300), OdsCode = "ABC123" },
                },
            },
        };
}
