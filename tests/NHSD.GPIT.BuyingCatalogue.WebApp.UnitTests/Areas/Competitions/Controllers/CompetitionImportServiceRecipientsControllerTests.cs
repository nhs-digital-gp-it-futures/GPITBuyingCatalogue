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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.ImportServiceRecipients;
using Xunit;
using ServiceRecipient = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceRecipient;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Controllers;

public static class CompetitionImportServiceRecipientsControllerTests
{
    private const string MismatchOdsCode = "MISMATCH";
    private const string MismatchOrganisationName = "MISMATCH organisation name";

    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionImportServiceRecipientsController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionImportServiceRecipientsController controller)
    {
        competitionsService.GetCompetitionName(Arg.Any<string>(), competition.Id).Returns(competition.Name);

        var expectedModel = new ImportServiceRecipientModel { Caption = competition.Name };

        var result = (await controller.Index(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

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
        int competitionId,
        ImportServiceRecipientModel model,
        [Frozen] IServiceRecipientImportService importService,
        CompetitionImportServiceRecipientsController controller)
    {
        importService.ReadFromStream(Arg.Any<Stream>()).Returns(importedServiceRecipients);

        _ = await controller.Index(internalOrgId, competitionId, model);

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
        int competitionId,
        ImportServiceRecipientModel model,
        [Frozen] IServiceRecipientImportService importService,
        CompetitionImportServiceRecipientsController controller)
    {
        importService.ReadFromStream(Arg.Any<Stream>())
            .Returns(
                new List<ServiceRecipientImportModel> { new() { Organisation = "Fake Org", OdsCode = "ABC123" } });

        var result = (await controller.Index(
                internalOrgId,
                competitionId,
                model))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Validate));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId }, { nameof(competitionId), competitionId },
                });
    }

    [Theory]
    [MockAutoData]
    public static async Task Validate_CachedRecipientsNull_Redirects(
        string internalOrgId,
        int competitionId,
        [Frozen] IServiceRecipientImportService importService,
        CompetitionImportServiceRecipientsController controller)
    {
        importService.GetCached(Arg.Any<DistributedCacheKey>()).Returns((IList<ServiceRecipientImportModel>)null);

        var result = (await controller.Validate(internalOrgId, competitionId, false))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId }, { nameof(competitionId), competitionId },
                });
    }

    [Theory]
    [MockAutoData]
    public static async Task Validate_MismatchedOdsCodes_ReturnsMismatchedOdsView(
        Organisation organisation,
        Competition competition,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] IServiceRecipientImportService importService,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IOdsService odsService,
        CompetitionImportServiceRecipientsController controller)
    {
        List<ServiceRecipient> workingRecipients = serviceRecipients.Take(3).ToList();

        List<ServiceRecipientImportModel> importedServiceRecipients = workingRecipients
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId })
            .ToList();
        importedServiceRecipients.First().OdsCode = MismatchOdsCode;
        List<ServiceRecipientImportModel> expectedInvalidRecipients =
            importedServiceRecipients.Where(x => x.OdsCode == MismatchOdsCode).ToList();
        var expectedModel = new ValidateOdsModel(expectedInvalidRecipients) { Caption = competition.Name };

        importService.GetCached(Arg.Any<DistributedCacheKey>()).Returns(importedServiceRecipients);

        competitionsService.GetCompetitionName(organisation.InternalIdentifier, competition.Id)
            .Returns(competition.Name);

        // This service call includes a filter to restrict by ods codes, but the controller logic should find mismatches regardless
        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                organisation.InternalIdentifier,
                Arg.Any<HashSet<string>>())
            .Returns(serviceRecipients);

        var result = (await controller.Validate(organisation.InternalIdentifier, competition.Id, false))
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
        Competition competition,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] IServiceRecipientImportService importService,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IOdsService odsService,
        CompetitionImportServiceRecipientsController controller)
    {
        List<ServiceRecipient> workingRecipients = serviceRecipients.Take(3).ToList();

        List<ServiceRecipientImportModel> importedServiceRecipients = workingRecipients
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId })
            .ToList();
        importedServiceRecipients.First().OdsCode = MismatchOdsCode;

        importService.GetCached(Arg.Any<DistributedCacheKey>()).Returns(importedServiceRecipients);

        competitionsService.GetCompetitionName(Arg.Any<string>(), competition.Id).Returns(competition.Name);

        // This service call includes a filter to restrict by ods codes, but the controller logic should find mismatches regardless
        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                organisation.InternalIdentifier,
                Arg.Any<HashSet<string>>())
            .Returns(serviceRecipients);

        var result = (await controller.Validate(organisation.InternalIdentifier, competition.Id, true))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionImportServiceRecipientsController.ValidationComplete));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", organisation.InternalIdentifier },
                    { "competitionId", competition.Id },
                    { "validationStatus", ValidationStatus.PartialSuccess },
                });
    }

    [Theory]
    [MockAutoData]
    public static async Task Validate_AllMismatchedOdsCodes_FailedStatus(
        Organisation organisation,
        Competition competition,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] IServiceRecipientImportService importService,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IOdsService odsService,
        CompetitionImportServiceRecipientsController controller)
    {
        List<ServiceRecipient> workingRecipients = serviceRecipients.Take(3).ToList();

        List<ServiceRecipientImportModel> importedServiceRecipients = workingRecipients
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId })
            .ToList();
        importedServiceRecipients.First().OdsCode = MismatchOdsCode;

        importService.GetCached(Arg.Any<DistributedCacheKey>()).Returns(importedServiceRecipients);

        competitionsService.GetCompetitionName(Arg.Any<string>(), competition.Id).Returns(competition.Name);

        // This service call includes a filter to restrict by ods codes, but the controller logic should find mismatches regardless
        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                organisation.InternalIdentifier,
                Arg.Any<HashSet<string>>())
            .Returns([]);

        var result = (await controller.Validate(organisation.InternalIdentifier, competition.Id, true))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionImportServiceRecipientsController.ValidationComplete));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", organisation.InternalIdentifier },
                    { "competitionId", competition.Id },
                    { "validationStatus", ValidationStatus.Failure },
                });
    }

    [Theory]
    [MockAutoData]
    public static async Task Validate_MismatchedNames_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] IServiceRecipientImportService importService,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IOdsService odsService,
        CompetitionImportServiceRecipientsController controller)
    {
        List<ServiceRecipientImportModel> importedServiceRecipients = serviceRecipients.Take(3)
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId })
            .ToList();

        importedServiceRecipients.First().Organisation = MismatchOrganisationName;

        var serviceRecipient = serviceRecipients.First();

        var mismatchedNames = new List<(string, string, string)>
        {
            (MismatchOrganisationName, serviceRecipient.Name, serviceRecipient.OrgId),
        };

        var expectedModel = new ValidateNamesModel(
            mismatchedNames)
        { Caption = competition.Name };

        importService.GetCached(Arg.Any<DistributedCacheKey>()).Returns(importedServiceRecipients);

        competitionsService.GetCompetitionName(organisation.InternalIdentifier, competition.Id)
            .Returns(competition.Name);

        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                organisation.InternalIdentifier,
                Arg.Any<HashSet<string>>())
            .Returns(serviceRecipients);

        var result = (await controller.Validate(organisation.InternalIdentifier, competition.Id, false))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt
                    .Excluding(m => m.BackLink)
                    .Excluding(m => m.CancelLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Validate_AllValid_Redirects(
        Organisation organisation,
        Competition competition,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] IServiceRecipientImportService importService,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IOdsService odsService,
        CompetitionImportServiceRecipientsController controller)
    {
        List<ServiceRecipient> workingRecipients = serviceRecipients.Take(3).ToList();

        List<ServiceRecipientImportModel> importedServiceRecipients = workingRecipients
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId })
            .ToList();

        importService.GetCached(Arg.Any<DistributedCacheKey>()).Returns(importedServiceRecipients);

        competitionsService.GetCompetitionName(organisation.InternalIdentifier, competition.Id)
            .Returns(competition.Name);

        // This service call includes a filter to restrict by ods codes, but the controller logic should find mismatches regardless
        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                organisation.InternalIdentifier,
                Arg.Any<HashSet<string>>())
            .Returns(serviceRecipients);

        var result = (await controller.Validate(organisation.InternalIdentifier, competition.Id, false))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionImportServiceRecipientsController.ValidationComplete));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", organisation.InternalIdentifier },
                    { "competitionId", competition.Id },
                    { "validationStatus", ValidationStatus.Success },
                });
    }

    [Theory]
    [MockMemberAutoData(nameof(RecipientsToSublocationMapping))]
    public static async Task ValidateComplete_ReturnsViewWithModel(
        List<ServiceRecipient> serviceRecipients,
        List<SublocationModel> sublocationsAsViewModel,
        Organisation organisation,
        Competition competition,
        [Frozen] IServiceRecipientImportService importService,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IOdsService odsService,
        CompetitionImportServiceRecipientsController controller)
    {
        List<ServiceRecipient> workingRecipients = serviceRecipients.Take(3).ToList();

        List<ServiceRecipientImportModel> importedServiceRecipients = workingRecipients
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId })
            .ToList();

        importService.GetCached(Arg.Any<DistributedCacheKey>()).Returns(importedServiceRecipients);

        competition.Organisation = organisation;

        competitionsService.GetCompetition(organisation.InternalIdentifier, competition.Id).Returns(competition);

        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                organisation.InternalIdentifier,
                Arg.Any<HashSet<string>>())
            .Returns(serviceRecipients);

        var expectedModel = new ValidationCompleteModel(
            competition.Name,
            ValidationStatus.Success,
            sublocationsAsViewModel);

        var result = (await controller.ValidationComplete(
                organisation.InternalIdentifier,
                competition.Id,
                ValidationStatus.Success))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt.Excluding(m => m.Caption));
    }

    [Theory]
    [MockAutoData]
    public static async Task ValidateComplete_Post_CancelsIfInvalid(
        string internalOrgId,
        int competitionId,
        CompetitionImportServiceRecipientsController controller)
    {
        var model = new ValidationCompleteModel("MY competition", ValidationStatus.Failure, []);

        var result =
            (await controller.ValidationComplete(internalOrgId, competitionId, model))
            .As<RedirectToActionResult>();

        result.ActionName.Should().Be(nameof(CompetitionImportServiceRecipientsController.CancelImport));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId }, { nameof(competitionId), competitionId },
                });
    }

    [Theory]
    [MockMemberAutoData(nameof(SublocationViewModelToSublocationEntityModelMapping))]
    public static async Task ValidateComplete_Post_PerformsExpectedFunctions(
        Organisation organisation,
        Competition competition,
        List<SublocationModel> sublocationsAsViewModel,
        List<CompetitionSublocation> sublocationsAsEntityModel,
        [Frozen] IServiceRecipientImportService importService,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionImportServiceRecipientsController controller)
    {
        competition.Organisation = organisation;

        competitionsService.GetCompetition(organisation.InternalIdentifier, competition.Id).Returns(competition);

        var modelForPost = new ValidationCompleteModel
        {
            Sublocations = sublocationsAsViewModel, ValidationStatus = ValidationStatus.Success,
        };

        var result =
            (await controller.ValidationComplete(organisation.InternalIdentifier, competition.Id, modelForPost))
            .As<RedirectToActionResult>();

        await competitionsService.Received()
            .SetCompetitionSublocationsAndRecipients(
                Arg.Is<string>(s => s == organisation.InternalIdentifier),
                Arg.Is<int>(i => i == competition.Id),
                Arg.Is<List<CompetitionSublocation>>(
                    list => AreListsEquivalentIgnoreOrder(list, sublocationsAsEntityModel)));

        await importService.Received().Clear(Arg.Any<DistributedCacheKey>());

        result.ActionName.Should().Be(nameof(CompetitionRecipientsController.ConfirmSublocations));
        result.ControllerName.Should().Be(typeof(CompetitionRecipientsController).ControllerName());
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", organisation.InternalIdentifier }, { "competitionId", competition.Id },
                });
    }

    [Theory]
    [MockAutoData]
    public static void CancelImport_Redirects(
        string internalOrgId,
        int competitionId,
        CompetitionImportServiceRecipientsController controller)
    {
        var result = controller.CancelImport(internalOrgId, competitionId)
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionRecipientsController.UploadOrSelectServiceRecipients));
        result.ControllerName.Should().Be(typeof(CompetitionRecipientsController).ControllerName());
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(competitionId), competitionId },
                });
    }

    public static IEnumerable<object[]> RecipientsToSublocationMapping()
    {
        return new[]
        {
            new object[]
            {
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
            },
        };
    }

    public static IEnumerable<object[]> SublocationViewModelToSublocationEntityModelMapping()
    {
        return new[]
        {
            new object[]
            {
                new Organisation
                {
                    Id = 21, InternalIdentifier = "BB-FFGG", ExternalIdentifier = "FFGG", Name = "A Local ICB",
                },
                new Competition
                {
                    Id = 34, Name = "My Competition", Description = "Competition for competitiony things",
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
                new List<CompetitionSublocation>
                {
                    new()
                    {
                        CompetitionId = 34,
                        SublocationOdsCode = "XXXX",
                        OwnerOdsCode = "FFGG",
                        SublocationRecipients = new List<CompetitionSublocationRecipient>
                        {
                            new()
                            {
                                CompetitionId = 34,
                                RecipientOdsCode = "AAAA",
                                ParentSublocationOdsCode = "XXXX",
                            },
                            new()
                            {
                                CompetitionId = 34,
                                RecipientOdsCode = "AAAB",
                                ParentSublocationOdsCode = "XXXX",
                            },
                        },
                    },
                    new()
                    {
                        CompetitionId = 34,
                        SublocationOdsCode = "XXXA",
                        OwnerOdsCode = "FFGG",
                        SublocationRecipients = new List<CompetitionSublocationRecipient>
                        {
                            new()
                            {
                                CompetitionId = 34,
                                RecipientOdsCode = "AAAC",
                                ParentSublocationOdsCode = "XXXA",
                            },
                        },
                    },
                },
            },
        };
    }

    public static IEnumerable<object[]> InvalidServiceRecipientsTestData()
        => new[]
        {
            new object[] { CompetitionImportServiceRecipientsController.InvalidFormat, null, },
            new object[] { CompetitionImportServiceRecipientsController.EmptyFile, new List<ServiceRecipientImportModel>() },
            new object[]
            {
                CompetitionImportServiceRecipientsController.InvalidFormat,
                new List<ServiceRecipientImportModel>
                {
                    new() { Organisation = string.Empty, OdsCode = "ABC123", },
                },
            },
            new object[]
            {
                CompetitionImportServiceRecipientsController.InvalidFormat,
                new List<ServiceRecipientImportModel>
                {
                    new() { Organisation = "Fake Org", OdsCode = string.Empty },
                },
            },
            new object[]
            {
                CompetitionImportServiceRecipientsController.OdsCodeExceedsLimit,
                new List<ServiceRecipientImportModel>
                {
                    new() { Organisation = "Fake Org", OdsCode = new('A', 10) },
                },
            },
            new object[]
            {
                CompetitionImportServiceRecipientsController.OrganisationExceedsLimit,
                new List<ServiceRecipientImportModel>
                {
                    new() { Organisation = new('A', 300), OdsCode = "ABC123" },
                },
            },
        };

    private static bool AreListsEquivalentIgnoreOrder(
        IReadOnlyList<CompetitionSublocation> actual,
        IReadOnlyList<CompetitionSublocation> expected)
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
}
