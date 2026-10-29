using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Controllers
{
    public static class CommunityPharmacyControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);

            assertion.Verify(typeof(CommunityPharmacyController).GetConstructors());
        }

        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(CommunityPharmacyController)
                .Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "CommunityPharmacy");
            typeof(CommunityPharmacyController)
                .Should().BeDecoratedWith<RouteAttribute>(x => x.Template == "community-pharmacy");
        }

        [Theory]
        [InlineData(nameof(CommunityPharmacyController.Summary), "{solutionId}")]
        [InlineData(nameof(CommunityPharmacyController.ListPrice), "{solutionId}/list-price")]
        [InlineData(nameof(CommunityPharmacyController.Capabilities), "{solutionId}/capabilities")]
        [InlineData(nameof(CommunityPharmacyController.Standards), "{solutionId}/standards")]
        [InlineData(nameof(CommunityPharmacyController.Integrations), "{solutionId}/integrations")]
        [InlineData(nameof(CommunityPharmacyController.ApplicationTypes), "{solutionId}/application-types")]
        [InlineData(nameof(CommunityPharmacyController.HostingType), "{solutionId}/hosting-type")]
        [InlineData(nameof(CommunityPharmacyController.DevelopmentPlans), "{solutionId}/development-plans")]
        [InlineData(nameof(CommunityPharmacyController.SupplierDetails), "{solutionId}/supplier-details")]
        public static void SolutionPage_IsCorrectlyDecorated(string action, string template)
        {
            typeof(CommunityPharmacyController).GetMethod(action)
                .Should().BeDecoratedWith<HttpGetAttribute>(x => x.Template == template);
        }

        [Theory]
        [MockInlineAutoData(PublicationStatus.Published)]
        [MockInlineAutoData(PublicationStatus.InRemediation)]
        public static async Task ApplicationTypes_ValidSolution_ReturnsCommunityPharmacyModel(
            PublicationStatus publicationStatus,
            Solution solution,
            CatalogueItemContentStatus contentStatus,
            [Frozen] ISolutionsService solutionsService,
            CommunityPharmacyController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = publicationStatus;
            solution.ApplicationTypeDetail = new ApplicationTypeDetail
            {
                ApplicationTypes = new() { ApplicationType.BrowserBased.EnumMemberName() },
                BrowsersSupported = new() { new() { BrowserName = "Chrome", MinimumBrowserVersion = "100" } },
            };
            solutionsService.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);
            solutionsService.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);

            var result = (await controller.ApplicationTypes(catalogueItem.Id)).Should().BeOfType<ViewResult>().Subject;
            var model = result.Model.Should().BeOfType<CommunityPharmacyApplicationTypesModel>().Subject;

            result.ViewName.Should().BeNullOrEmpty();
            model.ApplicationTypesDetails.ApplicationTypes.Items.Should().ContainKey("Browser-based application");
            model.ApplicationTypesDetails.BrowserBasedApplication.Items["Supported browser types"].List
                .Should().Equal("Chrome (Version 100)");
            model.ApplicationTypesDetails.NativeDesktopApplication.Should().BeNull();
            model.ApplicationTypesDetails.NativeMobileApplication.Should().BeNull();
            model.SelectedSection.Should().Be("Application type");
            model.Sections.Single(s => s.Selected).Action.Should().Be(nameof(CommunityPharmacyController.ApplicationTypes));
            model.BreadcrumbItems.Last().Controller.Should().Be("CommunityPharmacy");
            model.PaginationFooter.Previous.Name.Should().Be("Integrations");
            model.PaginationFooter.Next.Name.Should().Be("Hosting type");
            model.PaginationFooter.FullWidth.Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static async Task ApplicationTypes_MissingSolution_ReturnsBadRequest(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            CommunityPharmacyController controller)
        {
            solutionsService.GetSolutionWithBasicInformation(solutionId).Returns(default(CatalogueItem));

            var result = (await controller.ApplicationTypes(solutionId)).Should().BeOfType<BadRequestObjectResult>().Subject;

            result.Value.Should().Be($"No Catalogue Item found for Id: {solutionId}");
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockAutoData]
        public static async Task ApplicationTypes_SuspendedSolution_RedirectsToSummary(
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService solutionsService,
            CommunityPharmacyController controller)
        {
            catalogueItem.PublishedStatus = PublicationStatus.Suspended;
            solutionsService.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);

            var result = (await controller.ApplicationTypes(catalogueItem.Id)).Should().BeOfType<RedirectToActionResult>().Subject;

            result.ActionName.Should().Be(nameof(CommunityPharmacyController.Summary));
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockInlineAutoData(PublicationStatus.Published)]
        [MockInlineAutoData(PublicationStatus.InRemediation)]
        [MockInlineAutoData(PublicationStatus.Suspended)]
        public static async Task Summary_ValidSolution_ReturnsExpectedModelAndView(
            PublicationStatus publicationStatus,
            Solution solution,
            CatalogueItemContentStatus contentStatus,
            [Frozen] ISolutionsService solutionsService,
            CommunityPharmacyController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = publicationStatus;
            solutionsService.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);
            solutionsService.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);

            var result = await controller.Summary(catalogueItem.Id);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewName.Should().BeNullOrEmpty();
            viewResult.Model.Should().BeOfType<SummaryModel>();
            await solutionsService.Received(1).GetSolutionWithBasicInformation(catalogueItem.Id);
            await solutionsService.Received(1).GetContentStatusForCatalogueItem(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Summary_MissingSolution_ReturnsBadRequest(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] ISolutionStandardsService solutionStandardsService,
            CommunityPharmacyController controller)
        {
            solutionsService.GetSolutionWithBasicInformation(solutionId).Returns(default(CatalogueItem));

            var result = await controller.Summary(solutionId);

            result.Should().BeOfType<BadRequestObjectResult>()
                .Subject.Value.Should().Be($"No Catalogue Item found for Id: {solutionId}");
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
            await solutionsService.DidNotReceive().GetWorkOffPlans(Arg.Any<CatalogueItemId>());
            await solutionStandardsService.DidNotReceive().GetSolutionStandards(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockInlineAutoData(PublicationStatus.Published)]
        [MockInlineAutoData(PublicationStatus.InRemediation)]
        [MockInlineAutoData(PublicationStatus.Suspended)]
        public static async Task ListPrice_ValidSolution_ReturnsExpectedModelAndView(
            PublicationStatus publicationStatus,
            Solution solution,
            CatalogueItemContentStatus contentStatus,
            [Frozen] ISolutionsService solutionsService,
            CommunityPharmacyController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = publicationStatus;
            solutionsService.GetSolutionWithCataloguePrice(catalogueItem.Id).Returns(catalogueItem);
            solutionsService.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);

            var result = await controller.ListPrice(catalogueItem.Id);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewName.Should().BeNullOrEmpty();
            viewResult.Model.Should().BeOfType<CommunityPharmacyListPriceModel>()
                .Which.Should().BeEquivalentTo(new CommunityPharmacyListPriceModel(catalogueItem, contentStatus));
            await solutionsService.Received(1).GetSolutionWithCataloguePrice(catalogueItem.Id);
            await solutionsService.Received(1).GetContentStatusForCatalogueItem(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task ListPrice_MissingSolution_ReturnsBadRequest(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] ISolutionStandardsService solutionStandardsService,
            CommunityPharmacyController controller)
        {
            solutionsService.GetSolutionWithCataloguePrice(solutionId).Returns(default(CatalogueItem));

            var result = await controller.ListPrice(solutionId);

            result.Should().BeOfType<BadRequestObjectResult>()
                .Subject.Value.Should().Be($"No Catalogue Item found for Id: {solutionId}");
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
            await solutionsService.DidNotReceive().GetWorkOffPlans(Arg.Any<CatalogueItemId>());
            await solutionStandardsService.DidNotReceive().GetSolutionStandards(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockInlineAutoData(PublicationStatus.Published)]
        [MockInlineAutoData(PublicationStatus.InRemediation)]
        public static async Task Capabilities_ValidSolution_ReturnsExpectedModelAndView(
            PublicationStatus publicationStatus,
            Solution solution,
            CatalogueItemContentStatus contentStatus,
            [Frozen] ISolutionsService solutionsService,
            CommunityPharmacyController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = publicationStatus;
            solutionsService.GetSolutionWithCataloguePrice(catalogueItem.Id).Returns(catalogueItem);
            solutionsService.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);

            var result = await controller.Capabilities(catalogueItem.Id);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewName.Should().BeNullOrEmpty();
            viewResult.Model.Should().BeOfType<CommunityPharmacyCapabilitiesModel>()
                .Which.Should().BeEquivalentTo(new CommunityPharmacyCapabilitiesModel(catalogueItem, contentStatus));
            await solutionsService.Received(1).GetSolutionWithCataloguePrice(catalogueItem.Id);
            await solutionsService.Received(1).GetContentStatusForCatalogueItem(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Capabilities_MissingSolution_ReturnsBadRequest(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] ISolutionStandardsService solutionStandardsService,
            CommunityPharmacyController controller)
        {
            solutionsService.GetSolutionWithCataloguePrice(solutionId).Returns(default(CatalogueItem));

            var result = await controller.Capabilities(solutionId);

            result.Should().BeOfType<BadRequestObjectResult>()
                .Subject.Value.Should().Be($"No Catalogue Item found for Id: {solutionId}");
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
            await solutionsService.DidNotReceive().GetWorkOffPlans(Arg.Any<CatalogueItemId>());
            await solutionStandardsService.DidNotReceive().GetSolutionStandards(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Capabilities_SuspendedSolution_RedirectsToSummary(
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] ISolutionStandardsService solutionStandardsService,
            CommunityPharmacyController controller)
        {
            catalogueItem.PublishedStatus = PublicationStatus.Suspended;
            solutionsService.GetSolutionWithCataloguePrice(catalogueItem.Id).Returns(catalogueItem);

            var result = await controller.Capabilities(catalogueItem.Id);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be(nameof(CommunityPharmacyController.Summary));
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
            await solutionsService.DidNotReceive().GetWorkOffPlans(Arg.Any<CatalogueItemId>());
            await solutionStandardsService.DidNotReceive().GetSolutionStandards(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockInlineAutoData(PublicationStatus.Published)]
        [MockInlineAutoData(PublicationStatus.InRemediation)]
        public static async Task Standards_ValidSolution_ReturnsExpectedModelAndView(
            PublicationStatus publicationStatus,
            Solution solution,
            CatalogueItemContentStatus contentStatus,
            List<StandardComplianceModel> standards,
            List<WorkOffPlan> workOffPlans,
            [Frozen] ISolutionStandardsService solutionStandardsService,
            [Frozen] ISolutionsService solutionsService,
            CommunityPharmacyController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = publicationStatus;
            solutionsService.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);
            solutionsService.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);
            solutionStandardsService.GetSolutionStandards(catalogueItem.Id).Returns(standards);
            solutionsService.GetWorkOffPlans(catalogueItem.Id).Returns(workOffPlans);

            var result = await controller.Standards(catalogueItem.Id);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewName.Should().BeNullOrEmpty();
            viewResult.Model.Should().BeOfType<CommunityPharmacyStandardsModel>()
                .Which.Should().BeEquivalentTo(new CommunityPharmacyStandardsModel(catalogueItem, standards, workOffPlans.Select(x => x.StandardId), contentStatus));
            await solutionsService.Received(1).GetSolutionWithBasicInformation(catalogueItem.Id);
            await solutionsService.Received(1).GetContentStatusForCatalogueItem(catalogueItem.Id);
            await solutionStandardsService.Received(1).GetSolutionStandards(catalogueItem.Id);
            await solutionsService.Received(1).GetWorkOffPlans(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Standards_MissingSolution_ReturnsBadRequest(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] ISolutionStandardsService solutionStandardsService,
            CommunityPharmacyController controller)
        {
            solutionsService.GetSolutionWithBasicInformation(solutionId).Returns(default(CatalogueItem));

            var result = await controller.Standards(solutionId);

            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be($"No Catalogue Item found for Id: {solutionId}");
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
            await solutionsService.DidNotReceive().GetWorkOffPlans(Arg.Any<CatalogueItemId>());
            await solutionStandardsService.DidNotReceive().GetSolutionStandards(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Standards_SuspendedSolution_RedirectsToSummary(
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] ISolutionStandardsService solutionStandardsService,
            CommunityPharmacyController controller)
        {
            catalogueItem.PublishedStatus = PublicationStatus.Suspended;
            solutionsService.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);

            var result = await controller.Standards(catalogueItem.Id);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be(nameof(CommunityPharmacyController.Summary));
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
            await solutionsService.DidNotReceive().GetWorkOffPlans(Arg.Any<CatalogueItemId>());
            await solutionStandardsService.DidNotReceive().GetSolutionStandards(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockInlineAutoData(PublicationStatus.Published)]
        [MockInlineAutoData(PublicationStatus.InRemediation)]
        public static async Task Integrations_ValidSolution_ReturnsExpectedModelAndView(
            PublicationStatus publicationStatus,
            Solution solution,
            CatalogueItemContentStatus contentStatus,
            [Frozen] ISolutionsService solutionsService,
            CommunityPharmacyController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = publicationStatus;
            solutionsService.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);
            solutionsService.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);

            var result = await controller.Integrations(catalogueItem.Id);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewName.Should().BeNullOrEmpty();
            viewResult.Model.Should().BeOfType<CommunityPharmacyIntegrationModel>()
                .Which.Should().BeEquivalentTo(new CommunityPharmacyIntegrationModel(catalogueItem, contentStatus));
            await solutionsService.Received(1).GetSolutionWithBasicInformation(catalogueItem.Id);
            await solutionsService.Received(1).GetContentStatusForCatalogueItem(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Integrations_MissingSolution_ReturnsBadRequest(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] ISolutionStandardsService solutionStandardsService,
            CommunityPharmacyController controller)
        {
            solutionsService.GetSolutionWithBasicInformation(solutionId).Returns(default(CatalogueItem));

            var result = await controller.Integrations(solutionId);

            result.Should().BeOfType<BadRequestObjectResult>()
                .Subject.Value.Should().Be($"No Catalogue Item found for Id: {solutionId}");
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
            await solutionsService.DidNotReceive().GetWorkOffPlans(Arg.Any<CatalogueItemId>());
            await solutionStandardsService.DidNotReceive().GetSolutionStandards(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Integrations_SuspendedSolution_RedirectsToSummary(
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] ISolutionStandardsService solutionStandardsService,
            CommunityPharmacyController controller)
        {
            catalogueItem.PublishedStatus = PublicationStatus.Suspended;
            solutionsService.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);

            var result = await controller.Integrations(catalogueItem.Id);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be(nameof(CommunityPharmacyController.Summary));
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
            await solutionsService.DidNotReceive().GetWorkOffPlans(Arg.Any<CatalogueItemId>());
            await solutionStandardsService.DidNotReceive().GetSolutionStandards(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockInlineAutoData(PublicationStatus.Published)]
        [MockInlineAutoData(PublicationStatus.InRemediation)]
        public static async Task HostingType_ValidSolution_ReturnsExpectedModelAndView(
            PublicationStatus publicationStatus,
            Solution solution,
            CatalogueItemContentStatus contentStatus,
            [Frozen] ISolutionsService solutionsService,
            CommunityPharmacyController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = publicationStatus;
            solutionsService.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);
            solutionsService.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);

            var result = await controller.HostingType(catalogueItem.Id);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewName.Should().BeNullOrEmpty();
            viewResult.Model.Should().BeOfType<CommunityPharmacyHostingTypeModel>()
                .Which.Should().BeEquivalentTo(new CommunityPharmacyHostingTypeModel(catalogueItem, contentStatus));
            await solutionsService.Received(1).GetSolutionWithBasicInformation(catalogueItem.Id);
            await solutionsService.Received(1).GetContentStatusForCatalogueItem(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task HostingType_MissingSolution_ReturnsBadRequest(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] ISolutionStandardsService solutionStandardsService,
            CommunityPharmacyController controller)
        {
            solutionsService.GetSolutionWithBasicInformation(solutionId).Returns(default(CatalogueItem));

            var result = await controller.HostingType(solutionId);

            result.Should().BeOfType<BadRequestObjectResult>()
                .Subject.Value.Should().Be($"No Catalogue Item found for Id: {solutionId}");
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
            await solutionsService.DidNotReceive().GetWorkOffPlans(Arg.Any<CatalogueItemId>());
            await solutionStandardsService.DidNotReceive().GetSolutionStandards(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockAutoData]
        public static async Task HostingType_SuspendedSolution_RedirectsToSummary(
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] ISolutionStandardsService solutionStandardsService,
            CommunityPharmacyController controller)
        {
            catalogueItem.PublishedStatus = PublicationStatus.Suspended;
            solutionsService.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);

            var result = await controller.HostingType(catalogueItem.Id);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be(nameof(CommunityPharmacyController.Summary));
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
            await solutionsService.DidNotReceive().GetWorkOffPlans(Arg.Any<CatalogueItemId>());
            await solutionStandardsService.DidNotReceive().GetSolutionStandards(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockInlineAutoData(PublicationStatus.Published)]
        [MockInlineAutoData(PublicationStatus.InRemediation)]
        public static async Task DevelopmentPlans_ValidSolution_ReturnsExpectedModelAndView(
            PublicationStatus publicationStatus,
            Solution solution,
            CatalogueItemContentStatus contentStatus,
            List<WorkOffPlan> workOffPlans,
            [Frozen] ISolutionsService solutionsService,
            CommunityPharmacyController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = publicationStatus;
            solutionsService.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);
            solutionsService.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);
            solutionsService.GetWorkOffPlans(catalogueItem.Id).Returns(workOffPlans);

            var result = await controller.DevelopmentPlans(catalogueItem.Id);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewName.Should().BeNullOrEmpty();
            viewResult.Model.Should().BeOfType<CommunityPharmacyDevelopmentPlansModel>()
                .Which.Should().BeEquivalentTo(new CommunityPharmacyDevelopmentPlansModel(catalogueItem, workOffPlans, contentStatus));
            await solutionsService.Received(1).GetSolutionWithBasicInformation(catalogueItem.Id);
            await solutionsService.Received(1).GetContentStatusForCatalogueItem(catalogueItem.Id);
            await solutionsService.Received(1).GetWorkOffPlans(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task DevelopmentPlans_MissingSolution_ReturnsBadRequest(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] ISolutionStandardsService solutionStandardsService,
            CommunityPharmacyController controller)
        {
            solutionsService.GetSolutionWithBasicInformation(solutionId).Returns(default(CatalogueItem));

            var result = await controller.DevelopmentPlans(solutionId);

            result.Should().BeOfType<BadRequestObjectResult>()
                .Subject.Value.Should().Be($"No Catalogue Item found for Id: {solutionId}");
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
            await solutionsService.DidNotReceive().GetWorkOffPlans(Arg.Any<CatalogueItemId>());
            await solutionStandardsService.DidNotReceive().GetSolutionStandards(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockAutoData]
        public static async Task DevelopmentPlans_SuspendedSolution_RedirectsToSummary(
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] ISolutionStandardsService solutionStandardsService,
            CommunityPharmacyController controller)
        {
            catalogueItem.PublishedStatus = PublicationStatus.Suspended;
            solutionsService.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);

            var result = await controller.DevelopmentPlans(catalogueItem.Id);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be(nameof(CommunityPharmacyController.Summary));
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
            await solutionsService.DidNotReceive().GetWorkOffPlans(Arg.Any<CatalogueItemId>());
            await solutionStandardsService.DidNotReceive().GetSolutionStandards(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockInlineAutoData(PublicationStatus.Published)]
        [MockInlineAutoData(PublicationStatus.InRemediation)]
        public static async Task SupplierDetails_ValidSolution_ReturnsExpectedModelAndView(
            PublicationStatus publicationStatus,
            Solution solution,
            CatalogueItemContentStatus contentStatus,
            [Frozen] ISolutionsService solutionsService,
            CommunityPharmacyController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = publicationStatus;
            solutionsService.GetSolutionWithSupplierDetails(catalogueItem.Id).Returns(catalogueItem);
            solutionsService.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);

            var result = await controller.SupplierDetails(catalogueItem.Id);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewName.Should().BeNullOrEmpty();
            viewResult.Model.Should().BeOfType<CommunityPharmacySupplierDetails>()
                .Subject.Should().BeEquivalentTo(new CommunityPharmacySupplierDetails(catalogueItem, contentStatus));
            await solutionsService.Received(1).GetSolutionWithSupplierDetails(catalogueItem.Id);
            await solutionsService.Received(1).GetContentStatusForCatalogueItem(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task SupplierDetails_MissingSolution_ReturnsBadRequest(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] ISolutionStandardsService solutionStandardsService,
            CommunityPharmacyController controller)
        {
            solutionsService.GetSolutionWithSupplierDetails(solutionId).Returns(default(CatalogueItem));

            var result = await controller.SupplierDetails(solutionId);

            result.Should().BeOfType<BadRequestObjectResult>()
                .Subject.Value.Should().Be($"No Catalogue Item found for Id: {solutionId}");
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
            await solutionsService.DidNotReceive().GetWorkOffPlans(Arg.Any<CatalogueItemId>());
            await solutionStandardsService.DidNotReceive().GetSolutionStandards(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockAutoData]
        public static async Task SupplierDetails_SuspendedSolution_RedirectsToSummary(
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] ISolutionStandardsService solutionStandardsService,
            CommunityPharmacyController controller)
        {
            catalogueItem.PublishedStatus = PublicationStatus.Suspended;
            solutionsService.GetSolutionWithSupplierDetails(catalogueItem.Id).Returns(catalogueItem);

            var result = await controller.SupplierDetails(catalogueItem.Id);

            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be(nameof(CommunityPharmacyController.Summary));
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
            await solutionsService.DidNotReceive().GetWorkOffPlans(Arg.Any<CatalogueItemId>());
            await solutionStandardsService.DidNotReceive().GetSolutionStandards(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockAutoData]
        public static async Task GetSolutions_ReturnsExpectedModelAndView(
            string search,
            IList<CatalogueItem> catalogueItems,
            PageOptions returnedOptions,
            [Frozen] ISolutionsFilterService solutionsFilterService,
            CommunityPharmacyController controller)
        {
            const string page = "2";

            solutionsFilterService.GetAllSolutionsFiltered(
                    Arg.Is<SolutionsFilters>(filters => filters.Search == search && filters.IsCommunityPharmacy == true),
                    Arg.Any<PageOptions>())
                .Returns((catalogueItems, returnedOptions, null));

            var result = await controller.Solutions(page, null, search);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<CommunityPharmacySolutionsModel>().Subject;

            model.ResultsModel.CatalogueItems.Should().BeEquivalentTo(catalogueItems);
            model.ResultsModel.PageOptions.Should().BeSameAs(returnedOptions);

            await solutionsFilterService.Received(1).GetAllSolutionsFiltered(
                Arg.Is<SolutionsFilters>(filters => filters.Search == search && filters.IsCommunityPharmacy == true),
                Arg.Any<PageOptions>());
        }

        [Theory]
        [MockAutoData]
        public static async Task GetFilterSearchSuggestions_ReturnsJsonResult(
            string search,
            Uri uri,
            List<SearchFilterModel> searchResults,
            [Frozen] ISolutionsFilterService solutionsFilterService,
            CommunityPharmacyController controller)
        {
            solutionsFilterService.GetSolutionsBySearchTerm(search, Arg.Any<int>(), true).Returns(searchResults);

            var context = new DefaultHttpContext();
            context.Request.Headers.Referer = uri.ToString();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = context,
            };

            var currentPageUrl = new UriBuilder(uri.ToString());
            var expectedResults = searchResults.Select(r =>
                new HtmlEncodedSuggestionSearchResult(
                    r.Title,
                    r.Category,
                    currentPageUrl.AppendQueryParameterToUrl(nameof(search), r.Title).Uri.PathAndQuery));

            var result = await controller.FilterSearchSuggestions(search);

            await solutionsFilterService.Received(1).GetSolutionsBySearchTerm(search, Arg.Any<int>(), true);
            var actionResult = result.Should().BeOfType<JsonResult>().Subject;
            actionResult.Value.Should().BeEquivalentTo(expectedResults);
        }
    }
}
