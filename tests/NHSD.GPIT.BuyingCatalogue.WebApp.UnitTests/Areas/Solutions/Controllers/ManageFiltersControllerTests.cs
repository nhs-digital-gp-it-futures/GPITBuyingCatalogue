using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
using Xunit;
using ClientApplicationType = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.ClientApplicationType;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Controllers
{
    public static class ManageFiltersControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(ManageFiltersController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Policy == "Buyer");
            typeof(ManageFiltersController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Solutions");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ManageFiltersController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ReturnsExpectedResult(
            [Frozen] Mock<IOrganisationsService> organisationsService,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            [Frozen] Mock<IEpicsService> epicsService,
            [Frozen] Mock<IFrameworkService> frameworkService,
            [Frozen] Mock<IManageFiltersService> manageFiltersService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            [Frozen] Mock<IPdfService> mockPdfService,
            string primaryOrganisationInternalId,
            Organisation organisation,
            List<Filter> existingFilters)
        {
            organisationsService
                .Setup(x => x.GetOrganisationByInternalIdentifier(primaryOrganisationInternalId))
                .ReturnsAsync(organisation);

            manageFiltersService
                .Setup(x => x.GetFilters(organisation.Id))
                .ReturnsAsync(existingFilters);

            var controller = CreateController(
                organisationsService,
                capabilitiesService,
                epicsService,
                frameworkService,
                manageFiltersService,
                mockPdfService,
                primaryOrganisationInternalId);

            controller.Url = mockUrlHelper.Object;

            var result = await controller.Index();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ManageFiltersModel(existingFilters, organisation.Name);
            actualResult.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SaveFilter_ReturnsExpectedResult(
            AdditionalFiltersModel model,
            ManageFiltersController controller)
        {
            var result = controller.SaveFilter(model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(ManageFiltersController.ConfirmSaveFilter));
            actualResult.ControllerName.Should().Be(typeof(ManageFiltersController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "selectedCapabilityIds", model.SelectedCapabilityIds },
                { "selectedEpicIds", model.SelectedEpicIds },
                { "selectedFrameworkId", model.SelectedFrameworkId },
                { "selectedClientApplicationTypeIds", model.CombineSelectedOptions(model.ClientApplicationTypeOptions) },
                { "selectedHostingTypeIds", model.CombineSelectedOptions(model.HostingTypeOptions) },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DownloadResults_ValidFilter_ReturnsExpectedResult(
            FilterDetailsModel filter,
            [Frozen] Mock<IOrganisationsService> organisationsService,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            [Frozen] Mock<IEpicsService> epicsService,
            [Frozen] Mock<IFrameworkService> frameworkService,
            [Frozen] Mock<IManageFiltersService> manageFiltersService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            [Frozen] Mock<IPdfService> mockPdfService,
            byte[] fileContents,
            string primaryOrganisationInternalId,
            Organisation organisation)
        {
            organisationsService
                .Setup(x => x.GetOrganisationByInternalIdentifier(primaryOrganisationInternalId))
                .ReturnsAsync(organisation);

            manageFiltersService
                .Setup(x => x.GetFilterDetails(organisation.Id, filter.Id))
                .ReturnsAsync(filter);

            mockPdfService
                .Setup(s => s.Convert(It.IsAny<Uri>()))
                .ReturnsAsync(fileContents);

            mockPdfService
                .Setup(s => s.BaseUri())
                .Returns(new Uri("http://localhost"));

            var controller = CreateController(
                organisationsService,
                capabilitiesService,
                epicsService,
                frameworkService,
                manageFiltersService,
                mockPdfService,
                primaryOrganisationInternalId);

            controller.Url = mockUrlHelper.Object;

            var result = (await controller.DownloadResults(filter.Id)).As<FileContentResult>();

            result.Should().NotBeNull();
            result.ContentType.Should().Be("application/pdf");
            result.FileDownloadName.Should().Be($"{filter.Name} Catalogue Solutions.pdf");
            result.FileContents.Should().BeEquivalentTo(fileContents);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DownloadResults_NullFilter_ReturnsExpectedResult(
            FilterDetailsModel filter,
            [Frozen] Mock<IOrganisationsService> organisationsService,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            [Frozen] Mock<IEpicsService> epicsService,
            [Frozen] Mock<IFrameworkService> frameworkService,
            [Frozen] Mock<IManageFiltersService> manageFiltersService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            [Frozen] Mock<IPdfService> mockPdfService,
            string primaryOrganisationInternalId,
            Organisation organisation)
        {
            organisationsService
                .Setup(x => x.GetOrganisationByInternalIdentifier(primaryOrganisationInternalId))
                .ReturnsAsync(organisation);

            manageFiltersService
                .Setup(x => x.GetFilterDetails(organisation.Id, filter.Id))
                .ReturnsAsync((FilterDetailsModel)null);

            var controller = CreateController(
                organisationsService,
                capabilitiesService,
                epicsService,
                frameworkService,
                manageFiltersService,
                mockPdfService,
                primaryOrganisationInternalId);

            controller.Url = mockUrlHelper.Object;

            var result = await controller.DownloadResults(filter.Id);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_FilterDetails_NullFilter_ReturnsNotFound(
            Filter filter,
            [Frozen] Mock<IOrganisationsService> organisationsService,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            [Frozen] Mock<IEpicsService> epicsService,
            [Frozen] Mock<IFrameworkService> frameworkService,
            [Frozen] Mock<IManageFiltersService> manageFiltersService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            [Frozen] Mock<IPdfService> mockPdfService,
            string primaryOrganisationInternalId,
            Organisation organisation)
        {
            organisationsService
                .Setup(x => x.GetOrganisationByInternalIdentifier(primaryOrganisationInternalId))
                .ReturnsAsync(organisation);

            manageFiltersService
                .Setup(x => x.GetFilterDetails(organisation.Id, filter.Id))
                .ReturnsAsync((FilterDetailsModel)null);

            var controller = CreateController(
                organisationsService,
                capabilitiesService,
                epicsService,
                frameworkService,
                manageFiltersService,
                mockPdfService,
                primaryOrganisationInternalId);

            controller.Url = mockUrlHelper.Object;

            var result = await controller.FilterDetails(filter.Id);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_FilterDetails_ReturnsExpectedResult(
            string primaryOrganisationInternalId,
            int filterId,
            FilterDetailsModel filterDetailsModel,
            FilterIdsModel filterIdsModel,
            Organisation organisation,
            [Frozen] Mock<IOrganisationsService> organisationsService,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            [Frozen] Mock<IEpicsService> epicsService,
            [Frozen] Mock<IFrameworkService> frameworkService,
            [Frozen] Mock<IManageFiltersService> manageFiltersService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            [Frozen] Mock<IPdfService> mockPdfService)
        {
            organisationsService
                .Setup(x => x.GetOrganisationByInternalIdentifier(primaryOrganisationInternalId))
                .ReturnsAsync(organisation);

            manageFiltersService
                .Setup(x => x.GetFilterDetails(organisation.Id, filterId))
                .ReturnsAsync(filterDetailsModel);

            manageFiltersService
                .Setup(x => x.GetFilterIds(organisation.Id, filterId))
                .ReturnsAsync(filterIdsModel);

            var controller = CreateController(
                organisationsService,
                capabilitiesService,
                epicsService,
                frameworkService,
                manageFiltersService,
                mockPdfService,
                primaryOrganisationInternalId);

            controller.Url = mockUrlHelper.Object;

            var result = await controller.FilterDetails(filterId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ReviewFilterModel(filterDetailsModel, filterIdsModel) { Caption = organisation.Name };
            actualResult.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Get_CannotSaveFilter_ReturnsExpectedResult(
            ManageFiltersController controller)
        {
            var result = controller.CannotSaveFilter(string.Empty);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.Model.Should().BeOfType<NavBaseModel>();
        }

        [Theory]
        [CommonInlineAutoData(10)]
        [CommonInlineAutoData(11)]
        public static async Task Get_ConfirmSaveFilter_TooManyFilters_ReturnsExpectedResult(
            int numberOfExistingFilters,
            [Frozen] Mock<IOrganisationsService> organisationsService,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            [Frozen] Mock<IEpicsService> epicsService,
            [Frozen] Mock<IFrameworkService> frameworkService,
            [Frozen] Mock<IManageFiltersService> manageFiltersService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            [Frozen] Mock<IPdfService> mockPdfService,
            string primaryOrganisationInternalId,
            Organisation organisation)
        {
            var existingFilters = new List<Filter>(new Filter[numberOfExistingFilters]);

            organisationsService
                .Setup(x => x.GetOrganisationByInternalIdentifier(primaryOrganisationInternalId))
                .ReturnsAsync(organisation);

            manageFiltersService
                .Setup(x => x.GetFilters(organisation.Id))
                .ReturnsAsync(existingFilters);

            var controller = CreateController(
                organisationsService,
                capabilitiesService,
                epicsService,
                frameworkService,
                manageFiltersService,
                mockPdfService,
                primaryOrganisationInternalId);

            controller.Url = mockUrlHelper.Object;

            var result = await controller.ConfirmSaveFilter(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

            organisationsService.VerifyAll();
            manageFiltersService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(ManageFiltersController.CannotSaveFilter));
            actualResult.ControllerName.Should().Be(typeof(ManageFiltersController).ControllerName());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConfirmSaveFilter_ReturnsExpectedResult(
            List<Capability> capabilities,
            [Frozen] Mock<IOrganisationsService> organisationsService,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            [Frozen] Mock<IEpicsService> epicsService,
            [Frozen] Mock<IFrameworkService> frameworkService,
            [Frozen] Mock<IManageFiltersService> manageFiltersService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            [Frozen] Mock<IPdfService> mockPdfService,
            string primaryOrganisationInternalId,
            Organisation organisation,
            string selectedCapabilityIds,
            string selectedEpicIds,
            string selectedFrameworkId)
        {
            var existingFilters = new List<Filter>();

            organisationsService
                .Setup(x => x.GetOrganisationByInternalIdentifier(primaryOrganisationInternalId))
                .ReturnsAsync(organisation);

            manageFiltersService
                .Setup(x => x.GetFilters(organisation.Id))
                .ReturnsAsync(existingFilters);

            capabilitiesService
                .Setup(x => x.GetCapabilitiesByIds(It.IsAny<List<int>>()))
                .ReturnsAsync(capabilities);

            epicsService
                .Setup(x => x.GetEpicsByIds(It.IsAny<List<string>>()))
                .ReturnsAsync(new List<Epic>());

            frameworkService
                .Setup(x => x.GetFramework(selectedFrameworkId))
                .ReturnsAsync((EntityFramework.Catalogue.Models.Framework)null);

            var controller = CreateController(
                organisationsService,
                capabilitiesService,
                epicsService,
                frameworkService,
                manageFiltersService,
                mockPdfService,
                primaryOrganisationInternalId);

            controller.Url = mockUrlHelper.Object;

            var result = await controller.ConfirmSaveFilter(selectedCapabilityIds, selectedEpicIds, selectedFrameworkId, string.Empty, string.Empty);

            organisationsService.VerifyAll();
            manageFiltersService.VerifyAll();
            capabilitiesService.VerifyAll();
            epicsService.VerifyAll();
            frameworkService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var actualResultModel = actualResult.Model.Should().BeOfType<SaveFilterModel>().Subject;

            var expected = new SaveFilterModel(capabilities, new List<Epic>(), null, new List<ClientApplicationType>(), new List<HostingType>(), organisation.Id);
            actualResultModel.Should()
                .BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmSaveFilter_WithModelErrors_ReturnsExpectedResult(
            SaveFilterModel model,
            List<Capability> capabilities,
            List<Epic> epics,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            [Frozen] Mock<IEpicsService> epicsService,
            ManageFiltersController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            capabilitiesService
                .Setup(x => x.GetCapabilitiesByIds(model.CapabilityIds))
                .ReturnsAsync(capabilities);

            epicsService
                .Setup(x => x.GetEpicsByIds(model.EpicIds))
                .ReturnsAsync(epics);

            var result = await controller.ConfirmSaveFilter(model);

            capabilitiesService.VerifyAll();
            epicsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var resultModel = actualResult.Model.Should().BeOfType<SaveFilterModel>().Subject;
            resultModel.GroupedCapabilities.Count.Should().Be(capabilities.Count);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmSaveFilter_ReturnsExpectedResult(
            SaveFilterModel model,
            int filterId,
            [Frozen] Mock<IManageFiltersService> manageFiltersService,
            ManageFiltersController controller)
        {
            manageFiltersService
                .Setup(x => x.AddFilter(
                    model.Name,
                    model.Description,
                    model.OrganisationId,
                    model.CapabilityIds,
                    model.EpicIds,
                    model.FrameworkId,
                    model.ClientApplicationTypes,
                    model.HostingTypes)).ReturnsAsync(filterId);

            var result = await controller.ConfirmSaveFilter(model);

            manageFiltersService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(ManageFiltersController.Index));
            actualResult.ControllerName.Should().Be(typeof(ManageFiltersController).ControllerName());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteFilter_ReturnsViewResult(
            string primaryOrganisationInternalId,
            int filterId,
            FilterDetailsModel filterDetailsModel,
            Organisation organisation,
            [Frozen] Mock<IOrganisationsService> organisationsService,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            [Frozen] Mock<IEpicsService> epicsService,
            [Frozen] Mock<IFrameworkService> frameworkService,
            [Frozen] Mock<IManageFiltersService> manageFiltersService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            [Frozen] Mock<IPdfService> mockPdfService)
        {
            organisationsService
                .Setup(x => x.GetOrganisationByInternalIdentifier(primaryOrganisationInternalId))
                .ReturnsAsync(organisation);
            manageFiltersService
                .Setup(x => x.GetFilterDetails(organisation.Id, filterId))
                .ReturnsAsync(filterDetailsModel);
            var controller = CreateController(
                organisationsService,
                capabilitiesService,
                epicsService,
                frameworkService,
                manageFiltersService,
                mockPdfService,
                primaryOrganisationInternalId);
            controller.Url = mockUrlHelper.Object;

            var result = await controller.DeleteFilter(filterId);

            result.Should().BeOfType<ViewResult>();
            var viewResult = result.As<ViewResult>();
            viewResult.Model.Should().BeOfType<DeleteFilterModel>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteFilter_ReturnsRedirectToActionResult(
            string primaryOrganisationInternalId,
            FilterDetailsModel filterDetailsModel,
            DeleteFilterModel deleteFilterModel,
            Organisation organisation,
            [Frozen] Mock<IOrganisationsService> organisationsService,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            [Frozen] Mock<IEpicsService> epicsService,
            [Frozen] Mock<IFrameworkService> frameworkService,
            [Frozen] Mock<IManageFiltersService> manageFiltersService,
            [Frozen] Mock<IUrlHelper> mockUrlHelper,
            [Frozen] Mock<IPdfService> mockPdfService)
        {
            organisationsService
                .Setup(x => x.GetOrganisationByInternalIdentifier(primaryOrganisationInternalId))
                .ReturnsAsync(organisation);
            deleteFilterModel.FilterId = filterDetailsModel.Id;
            manageFiltersService
                .Setup(x => x.GetFilterDetails(organisation.Id, deleteFilterModel.FilterId))
                .ReturnsAsync(filterDetailsModel);
            var controller = CreateController(
                organisationsService,
                capabilitiesService,
                epicsService,
                frameworkService,
                manageFiltersService,
                mockPdfService,
                primaryOrganisationInternalId);
            controller.Url = mockUrlHelper.Object;

            var result = await controller.DeleteFilter(deleteFilterModel);
            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(ManageFiltersController.Index));
            manageFiltersService.Verify(x => x.DeleteFilter(deleteFilterModel.FilterId), Times.Once);
        }

        private static ManageFiltersController CreateController(
            Mock<IOrganisationsService> organisationsService,
            Mock<ICapabilitiesService> capabilitiesService,
            Mock<IEpicsService> epicsService,
            Mock<IFrameworkService> frameworkService,
            Mock<IManageFiltersService> manageFiltersService,
            Mock<IPdfService> pdfService,
            string primaryOrganisationInternalId)
        {
            return new ManageFiltersController(organisationsService.Object, capabilitiesService.Object, epicsService.Object, frameworkService.Object, manageFiltersService.Object, pdfService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(
                            new ClaimsIdentity(
                                new Claim[] { new(Framework.Constants.CatalogueClaims.PrimaryOrganisationInternalIdentifier, primaryOrganisationInternalId.ToString()) })),
                    },
                },
            };
        }
    }
}
