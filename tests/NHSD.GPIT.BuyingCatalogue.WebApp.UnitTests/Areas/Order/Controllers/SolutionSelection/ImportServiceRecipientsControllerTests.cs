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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
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

    public static IEnumerable<object[]> InvalidServiceRecipientsTestData()
    {
        return
        [
            [ImportServiceRecipientsController.InvalidFormat, null],
            [ImportServiceRecipientsController.EmptyFile, new List<ServiceRecipientImportModel>()],
            [
                ImportServiceRecipientsController.InvalidFormat,
                new List<ServiceRecipientImportModel> { new() { Organisation = string.Empty, OdsCode = "ABC123" } },
            ],
            [
                ImportServiceRecipientsController.InvalidFormat,
                new List<ServiceRecipientImportModel> { new() { Organisation = "Fake Org", OdsCode = string.Empty } },
            ],
            [
                ImportServiceRecipientsController.OdsCodeExceedsLimit,
                new List<ServiceRecipientImportModel>
                {
                    new() { Organisation = "Fake Org", OdsCode = new string('A', 10) },
                },
            ],
            [
                ImportServiceRecipientsController.OrganisationExceedsLimit,
                new List<ServiceRecipientImportModel>
                {
                    new() { Organisation = new string('A', 300), OdsCode = "ABC123" },
                },
            ],
        ];
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
        List<ServiceRecipientImportModel> importedServiceRecipients = serviceRecipients
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
        List<ServiceRecipientImportModel> importedServiceRecipients = serviceRecipients
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
        List<ServiceRecipientImportModel> importedServiceRecipients = serviceRecipients
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
        List<ServiceRecipientImportModel> importedServiceRecipients = serviceRecipients
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
        [Frozen] IOdsService odsService,
        ImportServiceRecipientsController controller)
    {
        List<ServiceRecipientImportModel> importedServiceRecipients = serviceRecipients
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId })
            .ToList();

        importService.GetCached(Arg.Any<DistributedCacheKey>()).Returns(importedServiceRecipients);

        // This service call includes a filter to restrict by ods codes, but the controller logic should find mismatches regardless
        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                organisation.InternalIdentifier,
                Arg.Any<HashSet<string>>())
            .Returns(serviceRecipients);

        var result = (await controller.Validate(organisation.InternalIdentifier, order.CallOffId, false))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(ImportServiceRecipientsController.ValidationComplete));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", organisation.InternalIdentifier },
                    { "callOffId", order.CallOffId },
                    { "validationStatus", ValidationStatus.Success },
                });
    }

    public static IEnumerable<object[]> RecipientsToSublocationMapping()
    {
        return
        [
            [
                new List<ServiceRecipient>
                {
                    new()
                    {
                        Name = "Surgery 1",
                        OrgId = "AAAA",
                        PrimaryRoleId = OrganisationType.GP.ToString(),
                        Location = "NHS Big Location XXXX",
                        LocationOrgId = "XXXX",
                    },
                    new()
                    {
                        Name = "Surgery 2",
                        OrgId = "AAAB",
                        PrimaryRoleId = OrganisationType.GP.ToString(),
                        Location = "NHS Big Location XXXX",
                        LocationOrgId = "XXXX",
                    },
                    new()
                    {
                        Name = "Surgery 34",
                        OrgId = "AAAC",
                        PrimaryRoleId = OrganisationType.GP.ToString(),
                        Location = "NHS Unrelated Big Location XXXA",
                        LocationOrgId = "XXXA",
                    },
                },
                new List<SublocationModel>
                {
                    new()
                    {
                        OdsCode = "XXXX",
                        ServiceRecipients =
                            new List<ServiceRecipientModel>
                            {
                                new()
                                {
                                    OdsCode = "AAAA",
                                    Name = "Surgery 1",
                                    Location = "NHS Big Location XXXX",
                                    LocationOrgId = "XXXX",
                                },
                                new()
                                {
                                    OdsCode = "AAAB",
                                    Name = "Surgery 2",
                                    Location = "NHS Big Location XXXX",
                                    LocationOrgId = "XXXX",
                                },
                            },
                    },
                    new()
                    {
                        OdsCode = "XXXA",
                        ServiceRecipients = new List<ServiceRecipientModel>
                        {
                            new()
                            {
                                OdsCode = "AAAC",
                                Name = "Surgery 34",
                                Location = "NHS Unrelated Big Location XXXA",
                                LocationOrgId = "XXXA",
                            },
                        },
                    },
                },
            ],
        ];
    }

    [Theory]
    [MockMemberAutoData(nameof(RecipientsToSublocationMapping))]
    public static async Task ValidateComplete_ReturnsViewWithModel(
        List<ServiceRecipient> serviceRecipients,
        List<SublocationModel> sublocationsAsViewModel,
        Organisation organisation,
        EntityFramework.Ordering.Models.Order order,
        [Frozen] IServiceRecipientImportService importService,
        [Frozen] IOrderService orderService,
        [Frozen] IOdsService odsService,
        ImportServiceRecipientsController controller)
    {
        List<ServiceRecipientImportModel> importedServiceRecipients = serviceRecipients
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId })
            .ToList();

        importService.GetCached(Arg.Any<DistributedCacheKey>()).Returns(importedServiceRecipients);

        order.OrderingParty = organisation;

        var wrappedOrder = new OrderWrapper(order);

        orderService.GetOrderThin(order.CallOffId, organisation.InternalIdentifier).Returns(wrappedOrder);

        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                organisation.InternalIdentifier,
                Arg.Any<HashSet<string>>())
            .Returns(serviceRecipients);

        var expectedModel = new ValidationCompleteModel(
            order.Description,
            ValidationStatus.Success,
            sublocationsAsViewModel,
            string.Empty);

        var result = (await controller.ValidationComplete(
                organisation.InternalIdentifier,
                order.CallOffId,
                ValidationStatus.Success))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt.Excluding(m => m.Caption).Excluding(m => m.CancelLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task ValidateComplete_Post_CancelsIfInvalid(
        string internalOrgId,
        CallOffId callOffId,
        ImportServiceRecipientsController controller)
    {
        var model = new ValidationCompleteModel("MY competition", ValidationStatus.Failure, [], string.Empty);

        var result =
            (await controller.ValidationComplete(internalOrgId, callOffId, model))
            .As<RedirectToActionResult>();

        result.ActionName.Should().Be(nameof(ImportServiceRecipientsController.CancelImport));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId }, { nameof(callOffId), callOffId },
                });
    }

    public static IEnumerable<object[]> SublocationViewModelToSublocationEntityModelMapping()
    {
        return
        [
            [
                new Organisation
                {
                    Id = 21, InternalIdentifier = "BB-FFGG", ExternalIdentifier = "FFGG", Name = "A Local ICB",
                },
                new EntityFramework.Ordering.Models.Order
                {
                    Id = 34, OrderNumber = 300, Revision = 1, Description = "My order",
                },
                new List<SublocationModel>
                {
                    new()
                    {
                        OdsCode = "XXXX",
                        ServiceRecipients =
                            new List<ServiceRecipientModel>
                            {
                                new() { OdsCode = "AAAA", Name = "Surgery 1", LocationOrgId = "XXXX" },
                                new() { OdsCode = "AAAB", Name = "Surgery 2", LocationOrgId = "XXXX" },
                            },
                    },
                    new()
                    {
                        OdsCode = "XXXA",
                        ServiceRecipients =
                            new List<ServiceRecipientModel>
                            {
                                new() { OdsCode = "AAAC", Name = "Surgery 34", LocationOrgId = "XXXA" },
                            },
                    },
                },
                new List<OrderSublocation>
                {
                    new()
                    {
                        OrderId = 34,
                        SublocationOdsCode = "XXXX",
                        OwnerOdsCode = "FFGG",
                        SublocationRecipients = new List<OrderSublocationRecipient>
                        {
                            new() { OrderId = 34, RecipientOdsCode = "AAAA", ParentSublocationOdsCode = "XXXX" },
                            new() { OrderId = 34, RecipientOdsCode = "AAAB", ParentSublocationOdsCode = "XXXX" },
                        },
                    },
                    new()
                    {
                        OrderId = 34,
                        SublocationOdsCode = "XXXA",
                        OwnerOdsCode = "FFGG",
                        SublocationRecipients = new List<OrderSublocationRecipient>
                        {
                            new() { OrderId = 34, RecipientOdsCode = "AAAC", ParentSublocationOdsCode = "XXXA" },
                        },
                    },
                },
            ],
        ];
    }

    [Theory]
    [MockMemberAutoData(nameof(SublocationViewModelToSublocationEntityModelMapping))]
    public static async Task ValidateComplete_Post_PerformsExpectedFunctions(
        Organisation organisation,
        EntityFramework.Ordering.Models.Order order,
        List<SublocationModel> sublocationsAsViewModel,
        List<OrderSublocation> sublocationsAsEntityModel,
        [Frozen] IServiceRecipientImportService importService,
        [Frozen] IOrderService orderService,
        ImportServiceRecipientsController controller)
    {
        order.OrderingParty = organisation;

        var wrappedOrder = new OrderWrapper(order);

        orderService.GetOrderThin(order.CallOffId, organisation.InternalIdentifier).Returns(wrappedOrder);

        var modelForPost = new ValidationCompleteModel
        {
            Sublocations = sublocationsAsViewModel, ValidationStatus = ValidationStatus.Success,
        };

        var result =
            (await controller.ValidationComplete(organisation.InternalIdentifier, order.CallOffId, modelForPost))
            .As<RedirectToActionResult>();

        await orderService.Received()
            .SetSublocationsAndRecipients(
                Arg.Is<CallOffId>(i => i == order.CallOffId),
                Arg.Is<string>(s => s == organisation.InternalIdentifier),
                Arg.Is<List<OrderSublocation>>(list => AreListsEquivalentIgnoreOrder(list, sublocationsAsEntityModel)));

        await importService.Received().Clear(Arg.Any<DistributedCacheKey>());

        result.ActionName.Should().Be(nameof(ServiceRecipientsController.ConfirmSublocations));
        result.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", organisation.InternalIdentifier }, { "callOffId", order.CallOffId },
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

    public static IEnumerable<object[]> AmendmentValidationConfirmNewRecipients()
    {
        return
        [
            // Same recipients requested as already in order history causes failure (both order revisions have same sublocation data as this is currently copied between revisions)
            [
                new List<ServiceRecipient>
                {
                    new()
                    {
                        Name = "Surgery 1",
                        OrgId = "AAAA",
                        PrimaryRoleId = OrganisationType.GP.ToString(),
                        Location = "NHS Big Location XXXX",
                        LocationOrgId = "XXXX",
                    },
                    new()
                    {
                        Name = "Surgery 34",
                        OrgId = "AAAC",
                        PrimaryRoleId = OrganisationType.GP.ToString(),
                        Location = "NHS Unrelated Big Location XXXA",
                        LocationOrgId = "XXXA",
                    },
                },
                new EntityFramework.Ordering.Models.Order
                {
                    Id = 112,
                    OrderNumber = 300,
                    Revision = 1,
                    Description = "My order",
                    OrderSublocations =
                    [
                        new OrderSublocation
                        {
                            SublocationOdsCode = "XXXX",
                            SublocationRecipients =
                            [
                                new OrderSublocationRecipient("AAAA", "XXXX"),
                            ],
                        },
                        new OrderSublocation
                        {
                            SublocationOdsCode = "XXXA",
                            SublocationRecipients = [new OrderSublocationRecipient("AAAC", "XXXA")],
                        },
                    ],
                },
                new EntityFramework.Ordering.Models.Order
                {
                    Id = 34,
                    OrderNumber = 300,
                    Revision = 2,
                    Description = "My order",
                    OrderSublocations =
                    [
                        new OrderSublocation
                        {
                            SublocationOdsCode = "XXXX",
                            SublocationRecipients =
                            [
                                new OrderSublocationRecipient("AAAA", "XXXX"),
                            ],
                        },
                        new OrderSublocation
                        {
                            SublocationOdsCode = "XXXA",
                            SublocationRecipients = [new OrderSublocationRecipient("AAAC", "XXXA")],
                        },
                    ],
                },
                null,
                false,
                "ServiceRecipients/ImportServiceRecipients/ValidateAmendmentRecipientsFailed",
            ],

            // New recipients in addition to what is already in order causes hasMissing false
            [
                new List<ServiceRecipient>
                {
                    new()
                    {
                        Name = "Surgery 1",
                        OrgId = "AAAA",
                        PrimaryRoleId = OrganisationType.GP.ToString(),
                        Location = "NHS Big Location XXXX",
                        LocationOrgId = "XXXX",
                    },
                    new()
                    {
                        Name = "Surgery 2",
                        OrgId = "AAAB",
                        PrimaryRoleId = OrganisationType.GP.ToString(),
                        Location = "NHS Big Location XXXX",
                        LocationOrgId = "XXXX",
                    },
                    new()
                    {
                        Name = "Surgery 34",
                        OrgId = "AAAC",
                        PrimaryRoleId = OrganisationType.GP.ToString(),
                        Location = "NHS Unrelated Big Location XXXA",
                        LocationOrgId = "XXXA",
                    },
                },
                new EntityFramework.Ordering.Models.Order
                {
                    Id = 112,
                    OrderNumber = 300,
                    Revision = 1,
                    Description = "My order",
                    OrderSublocations =
                    [
                        new OrderSublocation
                        {
                            SublocationOdsCode = "XXXX",
                            SublocationRecipients =
                            [
                                new OrderSublocationRecipient("AAAA", "XXXX"),
                            ],
                        },
                        new OrderSublocation
                        {
                            SublocationOdsCode = "XXXA",
                            SublocationRecipients = [new OrderSublocationRecipient("AAAC", "XXXA")],
                        },
                    ],
                },
                new EntityFramework.Ordering.Models.Order
                {
                    Id = 34,
                    OrderNumber = 300,
                    Revision = 2,
                    Description = "My order",
                    OrderSublocations =
                    [
                        new OrderSublocation
                        {
                            SublocationOdsCode = "XXXX",
                            SublocationRecipients =
                            [
                                new OrderSublocationRecipient("AAAA", "XXXX"),
                            ],
                        },
                        new OrderSublocation
                        {
                            SublocationOdsCode = "XXXA",
                            SublocationRecipients = [new OrderSublocationRecipient("AAAC", "XXXA")],
                        },
                    ],
                },
                new List<ServiceRecipient> { new() { OrgId = "AAAB", Name = "Surgery 34", LocationOrgId = "XXXX" } },
                false,
                "ServiceRecipients/ImportServiceRecipients/ValidateAmendmentRecipients",
            ],

            // Only New recipients causes hasMissing true (used to show disclaimer message)
            [
                new List<ServiceRecipient>
                {
                    new()
                    {
                        Name = "Surgery 2",
                        OrgId = "AAAB",
                        PrimaryRoleId = OrganisationType.GP.ToString(),
                        Location = "NHS Big Location XXXX",
                        LocationOrgId = "XXXX",
                    },
                },
                new EntityFramework.Ordering.Models.Order
                {
                    Id = 112,
                    OrderNumber = 300,
                    Revision = 1,
                    Description = "My order",
                    OrderSublocations =
                    [
                        new OrderSublocation
                        {
                            SublocationOdsCode = "XXXX",
                            SublocationRecipients =
                            [
                                new OrderSublocationRecipient("AAAA", "XXXX"),
                            ],
                        },
                        new OrderSublocation
                        {
                            SublocationOdsCode = "XXXA",
                            SublocationRecipients = [new OrderSublocationRecipient("AAAC", "XXXA")],
                        },
                    ],
                },
                new EntityFramework.Ordering.Models.Order
                {
                    Id = 34,
                    OrderNumber = 300,
                    Revision = 2,
                    Description = "My order",
                    OrderSublocations =
                    [
                        new OrderSublocation
                        {
                            SublocationOdsCode = "XXXX",
                            SublocationRecipients =
                            [
                                new OrderSublocationRecipient("AAAA", "XXXX"),
                            ],
                        },
                        new OrderSublocation
                        {
                            SublocationOdsCode = "XXXA",
                            SublocationRecipients = [new OrderSublocationRecipient("AAAC", "XXXA")],
                        },
                    ],
                },
                new List<ServiceRecipient> { new() { OrgId = "AAAB", Name = "Surgery 34", LocationOrgId = "XXXX" } },
                true,
                "ServiceRecipients/ImportServiceRecipients/ValidateAmendmentRecipients",
            ],
        ];
    }

    [Theory]
    [MockMemberAutoData(nameof(AmendmentValidationConfirmNewRecipients))]
    public static async Task ValidateAmendment_ReturnsViewWithModel(
        List<ServiceRecipient> serviceRecipients,
        EntityFramework.Ordering.Models.Order previousOrder,
        EntityFramework.Ordering.Models.Order order,
        List<ServiceRecipient> expectedOdsServiceResultModels,
        bool expectedHasMissing,
        string expectedRoute,
        Organisation organisation,
        [Frozen] IServiceRecipientImportService importService,
        [Frozen] IOrderService orderService,
        [Frozen] IOdsService odsService,
        ImportServiceRecipientsController controller)
    {
        List<ServiceRecipientImportModel> importedServiceRecipients = serviceRecipients
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId })
            .ToList();

        importService.GetCached(Arg.Any<DistributedCacheKey>()).Returns(importedServiceRecipients);

        order.OrderingParty = organisation;

        var wrappedOrder = new OrderWrapper(order, [previousOrder]);

        orderService.GetOrderWithSublocationsAndSublocationRecipients(order.CallOffId, organisation.InternalIdentifier)
            .Returns(wrappedOrder);

        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                organisation.InternalIdentifier,
                Arg.Any<HashSet<string>>())
            .Returns(expectedOdsServiceResultModels);

        List<ServiceRecipientModel> expectedRecipientModels = expectedOdsServiceResultModels?
            .Select(x => new ServiceRecipientModel(x))
            .OrderBy(x => x.LocationOrgId)
            .ToList();

        var expectedModel = new ValidateAmendmentRecipientsModel
        {
            HasMissing = expectedHasMissing, NewRecipients = expectedRecipientModels,
        };

        var result = (await controller.ValidateAmendment(
                organisation.InternalIdentifier,
                order.CallOffId))
            .As<ViewResult>();

        result.ViewName.Should().BeEquivalentTo(expectedRoute);

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt.Excluding(m => m.Caption).Excluding(m => m.CancelLink).Excluding(m => m.ContinueLink));
    }

    public static IEnumerable<object[]> AmendmentRecipientsToSublocationMapping()
    {
        return
        [
            // Sets sublocation recipients with 1 previous and 2 new with new sublocation
            [
                new EntityFramework.Ordering.Models.Order
                {
                    Id = 112,
                    OrderNumber = 300,
                    Revision = 1,
                    Description = "My order",
                    OrderSublocations =
                    [
                        new OrderSublocation
                        {
                            SublocationOdsCode = "XXXX",
                            SublocationRecipients =
                            [
                                new OrderSublocationRecipient("AAAA", "XXXX"),
                            ],
                        },
                    ],
                },
                new EntityFramework.Ordering.Models.Order
                {
                    Id = 34,
                    OrderNumber = 300,
                    Revision = 2,
                    Description = "My order",
                    OrderSublocations =
                    [
                        new OrderSublocation
                        {
                            SublocationOdsCode = "XXXX",
                            SublocationRecipients =
                            [
                                new OrderSublocationRecipient("AAAA", "XXXX"),
                            ],
                        },
                    ],
                },
                new ValidateAmendmentRecipientsModel
                {
                    NewRecipients =
                    [
                        new ServiceRecipientModel { LocationOrgId = "XXXX", OdsCode = "AAAB" },
                        new ServiceRecipientModel { LocationOrgId = "XXXA", OdsCode = "AAAC" },
                    ],
                },
                new HashSet<string> { "XXXX", "XXXA" },
                new Dictionary<string, HashSet<string>> { { "XXXX", ["AAAA", "AAAB"] }, { "XXXA", ["AAAC"] } },
            ],
        ];
    }

    [Theory]
    [MockMemberAutoData(nameof(AmendmentRecipientsToSublocationMapping))]
    public static async Task ValidateAmendment_Post_PerformsServiceCallsAndRedirectsAsExpected(
        EntityFramework.Ordering.Models.Order previousOrder,
        EntityFramework.Ordering.Models.Order order,
        ValidateAmendmentRecipientsModel model,
        HashSet<string> expectedOrderServiceSetSublocationsCall,
        Dictionary<string, HashSet<string>> expectedOrderSublocationServiceSetSublocationRecipientsCall,
        Organisation organisation,
        [Frozen] IServiceRecipientImportService importService,
        [Frozen] IOrderService orderService,
        [Frozen] IOrderSublocationService orderSublocationService,
        ImportServiceRecipientsController controller)
    {
        order.OrderingParty = organisation;

        var wrappedOrder = new OrderWrapper(order, [previousOrder]);

        orderService.GetOrderWithSublocationsAndSublocationRecipients(order.CallOffId, organisation.InternalIdentifier)
            .Returns(wrappedOrder);

        var result = (await controller.ValidateAmendment(
                organisation.InternalIdentifier,
                order.CallOffId,
                model))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(ServiceRecipientsController.ConfirmSublocations));

        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", organisation.InternalIdentifier }, { "callOffId", order.CallOffId },
                });

        await orderService.Received()
            .SetSublocations(
                order.CallOffId,
                organisation.InternalIdentifier,
                Arg.Is<HashSet<string>>(hs => AreStringHashSetsEquivalent(
                    hs,
                    expectedOrderServiceSetSublocationsCall)));

        foreach (KeyValuePair<string, HashSet<string>> kvp in
                 expectedOrderSublocationServiceSetSublocationRecipientsCall)
        {
            await orderSublocationService.Received()
                .SetSublocationRecipients(
                    organisation.ExternalIdentifier,
                    order.Id,
                    kvp.Key,
                    Arg.Is<HashSet<string>>(hs => AreStringHashSetsEquivalent(
                        hs,
                        kvp.Value)));
        }

        await importService.Received().Clear(Arg.Any<DistributedCacheKey>());
    }

    private static bool AreListsEquivalentIgnoreOrder(
        IReadOnlyList<OrderSublocation> actual,
        IReadOnlyList<OrderSublocation> expected)
    {
        try
        {
            actual.Should().BeEquivalentTo(expected, options => options.WithoutStrictOrdering());
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool AreStringHashSetsEquivalent(
        HashSet<string> actual,
        HashSet<string> expected)
    {
        try
        {
            actual.Should().BeEquivalentTo(expected);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
