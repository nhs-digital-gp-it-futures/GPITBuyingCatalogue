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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.ImportServiceRecipients;
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
        string internalOrgId,
        CallOffId callOffId,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        ImportServiceRecipientsController controller)
    {
        var expectedModel = new ImportServiceRecipientModel { Caption = callOffId.ToString() };

        var result = (await controller.Index(internalOrgId, callOffId)).As<ViewResult>();

        importService.VerifyAll();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt
                    .Excluding(m => m.BackLink)
                    .Excluding(m => m.File)
                    .Excluding(m => m.DownloadTemplateLink));
    }

    [Theory]
    [CommonMemberAutoData(nameof(InvalidServiceRecipientsTestData))]
    public static async Task Index_InvalidRecipients_SetsModelError(
        string expectedErrorMessage,
        IList<ServiceRecipientImportModel> importedServiceRecipients,
        string internalOrgId,
        CallOffId callOffId,
        ImportServiceRecipientModel model,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        ImportServiceRecipientsController controller)
    {
        importService.Setup(s => s.ReadFromStream(It.IsAny<Stream>()))
            .ReturnsAsync(importedServiceRecipients);

        _ = await controller.Index(internalOrgId, callOffId, model);

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
        string internalOrgId,
        CallOffId callOffId,
        ImportServiceRecipientModel model,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        ImportServiceRecipientsController controller)
    {
        importService.Setup(s => s.ReadFromStream(It.IsAny<Stream>()))
            .ReturnsAsync(
                new List<ServiceRecipientImportModel> { new() { Organisation = "Fake Org", OdsCode = "ABC123" } });

        var result = (await controller.Index(
                internalOrgId,
                callOffId,
                model))
            .As<RedirectToActionResult>();

        importService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ValidateOds));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(callOffId), callOffId },
                });
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateOds_CachedRecipientsNull_Redirects(
        string internalOrgId,
        CallOffId callOffId,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        ImportServiceRecipientsController controller)
    {
        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .ReturnsAsync((IList<ServiceRecipientImportModel>)null);

        var result = (await controller.ValidateOds(internalOrgId, callOffId))
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
                });
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateOds_MismatchedOdsCodes_ReturnsViewWithModel(
        string internalOrgId,
        CallOffId callOffId,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        [Frozen] Mock<IOdsService> odsService,
        ImportServiceRecipientsController controller)
    {
        var importedServiceRecipients = serviceRecipients.Take(2)
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId, })
            .ToList();
        importedServiceRecipients.First().OdsCode = "MISMATCH";

        var expectedModel = new ValidateOdsModel(
            importedServiceRecipients.Take(1).ToList())
        { Caption = callOffId.ToString() };

        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .ReturnsAsync(importedServiceRecipients);

        odsService.Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
            .ReturnsAsync(serviceRecipients);

        var result = (await controller.ValidateOds(internalOrgId, callOffId))
            .As<ViewResult>();

        importService.VerifyAll();
        odsService.VerifyAll();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt
                    .Excluding(m => m.BackLink)
                    .Excluding(m => m.CancelLink)
                    .Excluding(m => m.ValidateNamesLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateOds_ValidOdsCodes_Redirects(
        string internalOrgId,
        CallOffId callOffId,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        [Frozen] Mock<IOdsService> odsService,
        ImportServiceRecipientsController controller)
    {
        var importedServiceRecipients = serviceRecipients.Take(2)
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId, })
            .ToList();

        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .ReturnsAsync(importedServiceRecipients);

        odsService.Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
            .ReturnsAsync(serviceRecipients);

        var result = (await controller.ValidateOds(internalOrgId, callOffId))
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
                });
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateNames_CachedRecipientsNull_Redirects(
        string internalOrgId,
        CallOffId callOffId,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        ImportServiceRecipientsController controller)
    {
        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .ReturnsAsync((IList<ServiceRecipientImportModel>)null);

        var result = (await controller.ValidateNames(internalOrgId, callOffId))
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
                });
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateNames_MismatchedNames_ReturnsViewWithModel(
        string internalOrgId,
        CallOffId callOffId,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        [Frozen] Mock<IOdsService> odsService,
        ImportServiceRecipientsController controller)
    {
        var importedServiceRecipients = serviceRecipients.Take(2)
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId, })
            .ToList();
        importedServiceRecipients.First().Organisation = "MISMATCH";

        var serviceRecipient = serviceRecipients.First();

        var mismatchedNames = new List<(string, string, string)>
        {
            ("MISMATCH", serviceRecipient.Name, serviceRecipient.OrgId),
        };

        var expectedModel = new ValidateNamesModel(mismatchedNames)
        {
            Caption = callOffId.ToString(),
        };

        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .ReturnsAsync(importedServiceRecipients);

        odsService.Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
            .ReturnsAsync(serviceRecipients);

        var result = (await controller.ValidateNames(internalOrgId, callOffId))
            .As<ViewResult>();

        importService.VerifyAll();
        odsService.VerifyAll();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt
                    .Excluding(m => m.BackLink)
                    .Excluding(m => m.CancelLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateNames_ValidNames_Redirects(
        string internalOrgId,
        CallOffId callOffId,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        [Frozen] Mock<IOdsService> odsService,
        ImportServiceRecipientsController controller)
    {
        var recipientIds = serviceRecipients.Take(2)
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId, })
            .ToList();

        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .ReturnsAsync(recipientIds);

        odsService.Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
            .ReturnsAsync(serviceRecipients);

        var result = (await controller.ValidateNames(internalOrgId, callOffId))
            .As<RedirectToActionResult>();

        importService.VerifyAll();
        odsService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(ServiceRecipientsController.ConfirmChanges));
        result.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(callOffId), callOffId },
                    { nameof(recipientIds), string.Join(',', recipientIds.Select(s => s.OdsCode)) },
                    { "hasImported", true },
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
        var recipientIds = serviceRecipients
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId, })
            .ToList();

        recipientIds.First().OdsCode = "MISMATCH";
        recipientIds.Skip(1).First().Organisation = "MISMATCH";

        var firstServiceRecipient = serviceRecipients.First();

        var mismatchedNames = new List<(string, string, string)>
        {
            ("MISMATCH", firstServiceRecipient.Name, firstServiceRecipient.OrgId),
        };

        var model = new ValidateNamesModel(mismatchedNames);

        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .ReturnsAsync(recipientIds);

        catalogueItemService.Setup(s => s.GetCatalogueItemName(catalogueItemId))
            .ReturnsAsync(catalogueItemName);

        odsService.Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
            .ReturnsAsync(serviceRecipients);

        var result = (await controller.ValidateNames(internalOrgId, callOffId, catalogueItemId, model))
            .As<RedirectToActionResult>();

        importService.VerifyAll();
        odsService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(ServiceRecipientsController.ConfirmChanges));
        result.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(callOffId), callOffId },
                    { nameof(catalogueItemId), catalogueItemId },
                    { nameof(recipientIds), string.Join(',', recipientIds.Skip(1).Select(x => x.OdsCode)) },
                    { "hasImported", true },
                });
    }

    [Theory]
    [CommonAutoData]
    public static void CancelImport_Redirects(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        ImportServiceRecipientsController controller)
    {
        var result = controller.CancelImport(internalOrgId, callOffId, catalogueItemId)
            .As<RedirectToActionResult>();

        importService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(ServiceRecipientsController.UploadOrSelectServiceRecipients));
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
