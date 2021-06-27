using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    public static class AboutOrganisationControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AboutOrganisationController).Should()
                .BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
        }

        [Fact]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AboutOrganisationController( null,
                    Mock.Of<ISolutionsService>()));
        }

        [Fact]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AboutOrganisationController(
                    Mock.Of<IMapper>(), null));
        }

        [Fact]
        public static void Get_AboutSupplier_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(AboutOrganisationController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutOrganisationController.AboutSupplier)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(AboutOrganisationController.AboutSupplier).ToLowerCaseHyphenated());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AboutSupplier_ValidIdInput_RetrievesSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutOrganisationController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.AboutSupplier(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AboutSupplier_ValidIdInput_MapsSolutionToModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new AboutOrganisationController(
                mockMapper.Object, mockService.Object);

            await controller.AboutSupplier(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, AboutSupplierModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AboutSupplier_ValidIdInput_ReturnsViewWithExpectedModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockAboutSupplierModel = new Mock<AboutSupplierModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, AboutSupplierModel>(mockCatalogueItem))
                .Returns(mockAboutSupplierModel);
            var controller = new AboutOrganisationController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.AboutSupplier(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockAboutSupplierModel);
        }

        [Fact]
        public static void Post_AboutSupplier_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(AboutOrganisationController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutOrganisationController.AboutSupplier)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(AboutOrganisationController.AboutSupplier).ToLowerCaseHyphenated());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AboutSupplier_ModelNotValid_DoesNotCallService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutOrganisationController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.AboutSupplier(id, new Mock<AboutSupplierModel>().Object);

            mockService.Verify(
                x => x.SaveSupplierDescriptionAndLink(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AboutSupplier_ModelNotValid_ReturnsViewWithModel(CatalogueItemId id)
        {
            var mockAboutSupplierModel = new Mock<AboutSupplierModel>().Object;
            var controller = new AboutOrganisationController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.AboutSupplier(id, mockAboutSupplierModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockAboutSupplierModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AboutSupplier_ModelValid_CallsSaveOnService(CatalogueItemId id, AboutSupplierModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutOrganisationController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.AboutSupplier(id, model);

            mockService.Verify(x => x.SaveSupplierDescriptionAndLink(model.SupplierId,
                model.Description, model.Link));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AboutSupplier_ModelValid_RedirectsToSolution(
            [Frozen] CatalogueItemId id,
            AboutSupplierModel model)
        {
            var controller = new AboutOrganisationController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            var actual = (await controller.AboutSupplier(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be("Solution");
            actual.RouteValues["solutionId"].Should().Be(model.SolutionId);
        }

        [Fact]
        public static void Get_ContactDetails_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(AboutOrganisationController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutOrganisationController.ContactDetails)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(AboutOrganisationController.ContactDetails).ToLowerCaseHyphenated());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ContactDetails_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutOrganisationController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.ContactDetails(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ContactDetails_NullSolutionFromService_ReturnsBadRequestResponse(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new AboutOrganisationController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.ContactDetails(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ContactDetails_ValidId_MapsSolutionToViewModel_ReturnsExpectedView(CatalogueItemId id)
        {
            var mockContactDetailsModel = new Mock<ContactDetailsModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<CatalogueItem, ContactDetailsModel>(mockCatalogueItem))
                .Returns(mockContactDetailsModel);

            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);

            var controller = new AboutOrganisationController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.ContactDetails(id)).As<ViewResult>();

            mockMapper.Verify(x => x.Map<CatalogueItem, ContactDetailsModel>(mockCatalogueItem));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockContactDetailsModel);
        }

        [Fact]
        public static void Post_ContactDetails_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(AboutOrganisationController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutOrganisationController.ContactDetails)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(AboutOrganisationController.ContactDetails).ToLowerCaseHyphenated());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ContactDetails_InvalidModel_DoesNotCallService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutOrganisationController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.ContactDetails(id, new Mock<ContactDetailsModel>().Object);

            mockService.Verify(x => x.SaveSupplierContacts(It.IsAny<SupplierContactsModel>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ContactDetails_InvalidModel_ReturnsViewWithModel(CatalogueItemId id)
        {
            var model = new Mock<ContactDetailsModel>().Object;
            var controller = new AboutOrganisationController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.ContactDetails(id, model)).As<ViewResult>();

            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ContactDetails_ModelValid_ConvertsModelToSupplierContactsModel(CatalogueItemId id)
        {
            var contactDetailsModel = new Mock<ContactDetailsModel>().Object;
            var mapper = new Mock<IMapper>();
            var controller = new AboutOrganisationController(
                mapper.Object, Mock.Of<ISolutionsService>());

            await controller.ContactDetails(id, contactDetailsModel);

            mapper.Verify(x => x.Map<ContactDetailsModel, SupplierContactsModel>(contactDetailsModel));
        }

        // TODO: fix
        [Theory(Skip = "Broken")]
        [CommonAutoData]
        public static async Task Post_ContactDetails_ModelValid_CallsSaveOnService(CatalogueItemId id)
        {
            var contactDetailsModel = new Mock<ContactDetailsModel>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mapper = new Mock<IMapper>();
            var supplierContactsModel = new Mock<SupplierContactsModel>().Object;
            mapper.Setup(x => x.Map<ContactDetailsModel, SupplierContactsModel>(contactDetailsModel))
                .Returns(supplierContactsModel);
            var controller = new AboutOrganisationController(
                mapper.Object, mockService.Object);

            await controller.ContactDetails(id, contactDetailsModel);

            mockService.Verify(x => x.SaveSupplierContacts(supplierContactsModel));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ContactDetails_ModelValid_RedirectToSolutionAction([Frozen] CatalogueItemId id)
        {
            var contactDetailsModel = new ContactDetailsModel { SolutionId = id };
            var controller = new AboutOrganisationController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            var actual = (await controller.ContactDetails(id, contactDetailsModel))
                .As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be("Solution");
            actual.RouteValues["solutionId"].Should().Be(contactDetailsModel.SolutionId);
        }
    }
}
