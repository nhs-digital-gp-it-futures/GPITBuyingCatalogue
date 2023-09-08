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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.ImportServiceRecipients;
using Xunit;
using ServiceRecipient = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceRecipient;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Controllers;

public static class CompetitionImportServiceRecipientsControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionImportServiceRecipientsController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Index_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionImportServiceRecipientsController controller)
    {
        competitionsService.Setup(s => s.GetCompetitionName(It.IsAny<string>(), competition.Id))
            .ReturnsAsync(competition.Name);

        var expectedModel = new ImportServiceRecipientModel { Caption = competition.Name };

        var result = (await controller.Index(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        importService.VerifyAll();
        competitionsService.VerifyAll();

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
        int competitionId,
        ImportServiceRecipientModel model,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        CompetitionImportServiceRecipientsController controller)
    {
        importService.Setup(s => s.ReadFromStream(It.IsAny<Stream>()))
            .ReturnsAsync(importedServiceRecipients);

        _ = await controller.Index(internalOrgId, competitionId, model);

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
        int competitionId,
        ImportServiceRecipientModel model,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        CompetitionImportServiceRecipientsController controller)
    {
        importService.Setup(s => s.ReadFromStream(It.IsAny<Stream>()))
            .ReturnsAsync(
                new List<ServiceRecipientImportModel> { new() { Organisation = "Fake Org", OdsCode = "ABC123" } });

        var result = (await controller.Index(
                internalOrgId,
                competitionId,
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
                    { nameof(competitionId), competitionId },
                });
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateOds_CachedRecipientsNull_Redirects(
        string internalOrgId,
        int competitionId,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        CompetitionImportServiceRecipientsController controller)
    {
        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .ReturnsAsync((IList<ServiceRecipientImportModel>)null);

        var result = (await controller.ValidateOds(internalOrgId, competitionId))
            .As<RedirectToActionResult>();

        importService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(competitionId), competitionId },
                });
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateOds_MismatchedOdsCodes_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        [Frozen] Mock<IOdsService> odsService,
        CompetitionImportServiceRecipientsController controller)
    {
        var importedServiceRecipients = serviceRecipients.Take(2)
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId, })
            .ToList();
        importedServiceRecipients.First().OdsCode = "MISMATCH";

        var expectedModel = new ValidateOdsModel(
            importedServiceRecipients.Take(1).ToList()) { Caption = competition.Name };

        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .ReturnsAsync(importedServiceRecipients);

        competitionsService.Setup(s => s.GetCompetitionName(It.IsAny<string>(), competition.Id))
            .ReturnsAsync(competition.Name);

        odsService.Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(organisation.InternalIdentifier))
            .ReturnsAsync(serviceRecipients);

        var result = (await controller.ValidateOds(organisation.InternalIdentifier, competition.Id))
            .As<ViewResult>();

        importService.VerifyAll();
        competitionsService.VerifyAll();
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
        int competitionId,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        [Frozen] Mock<IOdsService> odsService,
        CompetitionImportServiceRecipientsController controller)
    {
        var importedServiceRecipients = serviceRecipients.Take(2)
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId, })
            .ToList();

        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .ReturnsAsync(importedServiceRecipients);

        odsService.Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
            .ReturnsAsync(serviceRecipients);

        var result = (await controller.ValidateOds(internalOrgId, competitionId))
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
                    { nameof(competitionId), competitionId },
                });
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateNames_CachedRecipientsNull_Redirects(
        string internalOrgId,
        int competitionId,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        CompetitionImportServiceRecipientsController controller)
    {
        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .ReturnsAsync((IList<ServiceRecipientImportModel>)null);

        var result = (await controller.ValidateNames(internalOrgId, competitionId))
            .As<RedirectToActionResult>();

        importService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(competitionId), competitionId },
                });
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateNames_MismatchedNames_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        [Frozen] Mock<IOdsService> odsService,
        CompetitionImportServiceRecipientsController controller)
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

        var expectedModel = new ValidateNamesModel(
            mismatchedNames) { Caption = competition.Name };

        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .ReturnsAsync(importedServiceRecipients);

        competitionsService.Setup(s => s.GetCompetitionName(It.IsAny<string>(), competition.Id))
            .ReturnsAsync(competition.Name);

        odsService.Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(organisation.InternalIdentifier))
            .ReturnsAsync(serviceRecipients);

        var result = (await controller.ValidateNames(organisation.InternalIdentifier, competition.Id))
            .As<ViewResult>();

        importService.VerifyAll();
        competitionsService.VerifyAll();
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
        int competitionId,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        [Frozen] Mock<IOdsService> odsService,
        CompetitionImportServiceRecipientsController controller)
    {
        var importedRecipients = serviceRecipients.Take(2)
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId, })
            .ToList();

        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .ReturnsAsync(importedRecipients);

        odsService.Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
            .ReturnsAsync(serviceRecipients);

        var result = (await controller.ValidateNames(internalOrgId, competitionId))
            .As<RedirectToActionResult>();

        importService.VerifyAll();
        odsService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionRecipientsController.Index));
        result.ControllerName.Should().Be(typeof(CompetitionRecipientsController).ControllerName());
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(competitionId), competitionId },
                    { nameof(importedRecipients), string.Join(',', importedRecipients.Select(s => s.OdsCode)) },
                });
    }

    [Theory]
    [CommonAutoData]
    public static async Task ValidateNames_Post_Redirects(
        Competition competition,
        string internalOrgId,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        [Frozen] Mock<IOdsService> odsService,
        CompetitionImportServiceRecipientsController controller)
    {
        var importedRecipients = serviceRecipients
            .Select(r => new ServiceRecipientImportModel { Organisation = r.Name, OdsCode = r.OrgId, })
            .ToList();

        importedRecipients.First().OdsCode = "MISMATCH";
        importedRecipients.Skip(1).First().Organisation = "MISMATCH";

        var firstServiceRecipient = serviceRecipients.First();

        var mismatchedNames = new List<(string, string, string)>
        {
            ("MISMATCH", firstServiceRecipient.Name, firstServiceRecipient.OrgId),
        };

        var model = new ValidateNamesModel(
            mismatchedNames);

        importService.Setup(s => s.GetCached(It.IsAny<ServiceRecipientCacheKey>()))
            .ReturnsAsync(importedRecipients);

        competitionsService.Setup(s => s.GetCompetitionName(It.IsAny<string>(), competition.Id))
            .ReturnsAsync(competition.Name);

        odsService.Setup(s => s.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
            .ReturnsAsync(serviceRecipients);

        var result = (await controller.ValidateNames(internalOrgId, competition.Id, model))
            .As<RedirectToActionResult>();

        importService.VerifyAll();
        odsService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionRecipientsController.Index));
        result.ControllerName.Should().Be(typeof(CompetitionRecipientsController).ControllerName());
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { "competitionId", competition.Id },
                    { nameof(importedRecipients), string.Join(',', importedRecipients.Skip(1).Select(x => x.OdsCode)) },
                });
    }

    [Theory]
    [CommonAutoData]
    public static void CancelImport_Redirects(
        string internalOrgId,
        int competitionId,
        [Frozen] Mock<IServiceRecipientImportService> importService,
        CompetitionImportServiceRecipientsController controller)
    {
        var result = controller.CancelImport(internalOrgId, competitionId)
            .As<RedirectToActionResult>();

        importService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionRecipientsController.Index));
        result.ControllerName.Should().Be(typeof(CompetitionRecipientsController).ControllerName());
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(competitionId), competitionId },
                });
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
}
