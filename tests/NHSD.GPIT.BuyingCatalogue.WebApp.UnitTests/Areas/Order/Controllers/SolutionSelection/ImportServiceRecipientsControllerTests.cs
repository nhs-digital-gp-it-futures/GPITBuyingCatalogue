using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.ImportServiceRecipients;
using Xunit;
using ServiceRecipient = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceRecipient;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection;

public static class ImportServiceRecipientsControllerTests
{
    private const string MismatchOdsCode = "MISMATCH";
    private const string MismatchOrganisationName = "MISMATCH organisation name";

    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(ImportServiceRecipientsController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_ReturnsViewWithModel(
        string internalOrgId,
        CallOffId callOffId,
        ImportServiceRecipientsController controller)
    {
        var expectedModel = new ImportServiceRecipientModel { Caption = callOffId.ToString() };

        var result = (await controller.Index(internalOrgId, callOffId)).As<ViewResult>();

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
    [MockMemberAutoData(nameof(InvalidServiceRecipientsTestData))]
    public static async Task Index_InvalidRecipients_SetsModelError(
        string expectedErrorMessage,
        IList<ServiceRecipientImportModel> importedServiceRecipients,
        string internalOrgId,
        CallOffId callOffId,
        ImportServiceRecipientModel model,
        [Frozen] IServiceRecipientImportService importService,
        ImportServiceRecipientsController controller)
    {
        importService.ReadFromStream(Arg.Any<Stream>()).Returns(importedServiceRecipients);

        _ = await controller.Index(internalOrgId, callOffId, model);

        controller.ModelState.Should().ContainKey(nameof(model.File));
        controller.ModelState.Should()
            .Contain(
                m => m.Value.Errors.Any(
                    x => string.Equals(x.ErrorMessage, expectedErrorMessage)));
        controller.ModelState.Clear();
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_ValidRecipients_Redirects(
        string internalOrgId,
        CallOffId callOffId,
        ImportServiceRecipientModel model,
        [Frozen] IServiceRecipientImportService importService,
        ImportServiceRecipientsController controller)
    {
        importService.ReadFromStream(Arg.Any<Stream>())
            .Returns(
                new List<ServiceRecipientImportModel> { new() { Organisation = "Fake Org", OdsCode = "ABC123" } });

        var result = (await controller.Index(
                internalOrgId,
                callOffId,
                model))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Validate));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId }, { nameof(callOffId), callOffId },
                });
    }

    [Theory]
    [MockAutoData]
    public static async Task ValidateOds_CachedRecipientsNull_Redirects(
        string internalOrgId,
        CallOffId callOffId,
        [Frozen] IServiceRecipientImportService importService,
        ImportServiceRecipientsController controller)
    {
        importService.GetCached(Arg.Any<DistributedCacheKey>()).Returns((IList<ServiceRecipientImportModel>)null);

        var result = (await controller.Validate(internalOrgId, callOffId, null))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId }, { nameof(callOffId), callOffId },
                });
    }

    [Theory]
    [MockAutoData]
    public static async Task Validate_MismatchedOdsCodes_ReturnsMismatchedOdsView(
        Organisation organisation,
        EntityFramework.Ordering.Models.Order order,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] IServiceRecipientImportService importService,
        [Frozen] IOrderService ordersService,
        [Frozen] IOdsService odsService,
        ImportServiceRecipientsController controller)
    {
        List<ServiceRecipient> workingRecipients = serviceRecipients.Take(3).ToList();

        List<ServiceRecipientImportModel> importedServiceRecipients = workingRecipients
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId })
            .ToList();
        importedServiceRecipients.First().OdsCode = MismatchOdsCode;
        List<ServiceRecipientImportModel> expectedInvalidRecipients =
            importedServiceRecipients.Where(x => x.OdsCode == MismatchOdsCode).ToList();
        var expectedModel = new ValidateOdsModel(expectedInvalidRecipients) { Caption = order.Description };

        importService.GetCached(Arg.Any<DistributedCacheKey>()).Returns(importedServiceRecipients);

        ordersService.GetOrderThin(order.CallOffId, organisation.InternalIdentifier)
            .Returns(new OrderWrapper(order));

        // This service call includes a filter to restrict by ods codes, but the controller logic should find mismatches regardless
        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                organisation.InternalIdentifier,
                Arg.Any<HashSet<string>>())
            .Returns(serviceRecipients);

        var result = (await controller.Validate(organisation.InternalIdentifier, order.CallOffId, null))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt
                    .Excluding(m => m.BackLink)
                    .Excluding(m => m.CancelLink)
                    .Excluding(m => m.ContinueLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Validate_MismatchedOdsCodes_SkipsIfDisclaimerAccepted(
        Organisation organisation,
        EntityFramework.Ordering.Models.Order order,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] IServiceRecipientImportService importService,
        [Frozen] IOrderService ordersService,
        [Frozen] IOdsService odsService,
        ImportServiceRecipientsController controller)
    {
        List<ServiceRecipient> workingRecipients = serviceRecipients.Take(3).ToList();

        List<ServiceRecipientImportModel> importedServiceRecipients = workingRecipients
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId })
            .ToList();
        importedServiceRecipients.First().OdsCode = MismatchOdsCode;

        importService.GetCached(Arg.Any<DistributedCacheKey>()).Returns(importedServiceRecipients);

        ordersService.GetOrderThin(order.CallOffId, organisation.InternalIdentifier)
            .Returns(new OrderWrapper(order));

        // This service call includes a filter to restrict by ods codes, but the controller logic should find mismatches regardless
        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                organisation.InternalIdentifier,
                Arg.Any<HashSet<string>>())
            .Returns(serviceRecipients);

        var result = (await controller.Validate(organisation.InternalIdentifier, order.CallOffId, true))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(ImportServiceRecipientsController.ValidationComplete));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", organisation.InternalIdentifier },
                    { "callOffId", order.CallOffId },
                    { "validationStatus", ValidationStatus.PartialSuccess },
                });
    }

    [Theory]
    [MockAutoData]
    public static async Task Validate_AllMismatchedOdsCodes_FailedStatus(
        Organisation organisation,
        EntityFramework.Ordering.Models.Order order,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] IServiceRecipientImportService importService,
        [Frozen] IOrderService ordersService,
        [Frozen] IOdsService odsService,
        ImportServiceRecipientsController controller)
    {
        List<ServiceRecipient> workingRecipients = serviceRecipients.Take(3).ToList();

        List<ServiceRecipientImportModel> importedServiceRecipients = workingRecipients
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId })
            .ToList();
        importedServiceRecipients.First().OdsCode = MismatchOdsCode;

        importService.GetCached(Arg.Any<DistributedCacheKey>()).Returns(importedServiceRecipients);

        ordersService.GetOrderThin(order.CallOffId, organisation.InternalIdentifier)
            .Returns(new OrderWrapper(order));

        // This service call includes a filter to restrict by ods codes, but the controller logic should find mismatches regardless
        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                organisation.InternalIdentifier,
                Arg.Any<HashSet<string>>())
            .Returns([]);

        var result = (await controller.Validate(organisation.InternalIdentifier, order.CallOffId, true))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(ImportServiceRecipientsController.ValidationComplete));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", organisation.InternalIdentifier },
                    { "callOffId", order.CallOffId },
                    { "validationStatus", ValidationStatus.Failure },
                });
    }

    [Theory]
    [MockAutoData]
    public static async Task Validate_MismatchedNames_ReturnsViewWithModel(
        Organisation organisation,
        EntityFramework.Ordering.Models.Order order,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] IServiceRecipientImportService importService,
        [Frozen] IOrderService ordersService,
        [Frozen] IOdsService odsService,
        ImportServiceRecipientsController controller)
    {
        List<ServiceRecipientImportModel> importedServiceRecipients = serviceRecipients.Take(3)
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId })
            .ToList();

        importedServiceRecipients.First().Organisation = MismatchOrganisationName;

        ServiceRecipient serviceRecipient = serviceRecipients.First();

        var mismatchedNames = new List<(string, string, string)>
        {
            (MismatchOrganisationName, serviceRecipient.Name, serviceRecipient.OrgId),
        };

        var expectedModel = new ValidateNamesModel(
            mismatchedNames) { Caption = order.Description };

        importService.GetCached(Arg.Any<DistributedCacheKey>()).Returns(importedServiceRecipients);

        ordersService.GetOrderThin(order.CallOffId, organisation.InternalIdentifier)
            .Returns(new OrderWrapper(order));

        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                organisation.InternalIdentifier,
                Arg.Any<HashSet<string>>())
            .Returns(serviceRecipients);

        var result = (await controller.Validate(organisation.InternalIdentifier, order.CallOffId, false))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt
                    .Excluding(m => m.BackLink)
                    .Excluding(m => m.CancelLink)
                    .Excluding(m => m.ContinueLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Validate_AllValid_Redirects(
        Organisation organisation,
        EntityFramework.Ordering.Models.Order order,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] IServiceRecipientImportService importService,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IOdsService odsService,
        ImportServiceRecipientsController controller)
    {
        List<ServiceRecipient> workingRecipients = serviceRecipients.Take(3).ToList();

        List<ServiceRecipientImportModel> importedServiceRecipients = workingRecipients
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId })
            .ToList();

        importService.GetCached(Arg.Any<DistributedCacheKey>()).Returns(importedServiceRecipients);

        competitionsService.GetCompetitionName(organisation.InternalIdentifier, order.Id)
            .Returns(order.Description);

        // This service call includes a filter to restrict by ods codes, but the controller logic should find mismatches regardless
        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                organisation.InternalIdentifier,
                Arg.Any<HashSet<string>>())
            .Returns(serviceRecipients);

        var result = (await controller.Validate(organisation.InternalIdentifier, order.CallOffId, false))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionImportServiceRecipientsController.ValidationComplete));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", organisation.InternalIdentifier },
                    { "callOffId", order.CallOffId },
                    { "validationStatus", ValidationStatus.Success },
                });
    }

    [Theory]
    [MockAutoData]
    public static void CancelImport_Redirects(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        ImportServiceRecipientsController controller)
    {
        var result = controller.CancelImport(internalOrgId, callOffId, catalogueItemId)
            .As<RedirectToActionResult>();

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
