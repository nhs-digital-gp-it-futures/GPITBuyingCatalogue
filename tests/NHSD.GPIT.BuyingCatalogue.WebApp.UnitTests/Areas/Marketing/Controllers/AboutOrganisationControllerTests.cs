using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class AboutOrganisationControllerTests
    {
        private static string[] InvalidStrings = {null, string.Empty, "    "};
        
        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AboutOrganisationController).Should()
                .BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AboutOrganisationController(null, Mock.Of<IMapper>(), Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AboutOrganisationController(Mock.Of<ILogWrapper<AboutOrganisationController>>(), null,
                    Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AboutOrganisationController(Mock.Of<ILogWrapper<AboutOrganisationController>>(),
                    Mock.Of<IMapper>(), null));
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_AboutSupplier_InvalidId_ThrowsException(string id)
        {
            var controller = new AboutOrganisationController(Mock.Of<ILogWrapper<AboutOrganisationController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.AboutSupplier(id));
        }

        [Test]
        public static void Post_AboutSupplier_NullModel_ThrowsException()
        {
            var controller = new AboutOrganisationController(Mock.Of<ILogWrapper<AboutOrganisationController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.AboutSupplier((AboutSupplierModel) null));
        }
        
        [Test]
        public static void Get_ContactDetails_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(AboutOrganisationController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutOrganisationController.ContactDetails)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be("contact-details");
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_ContactDetails_InvalidId_ThrowsException(string id)
        {
            var controller = new AboutOrganisationController(Mock.Of<ILogWrapper<AboutOrganisationController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.ContactDetails(id));
        }

        [Test]
        public static void Post_ContactDetails_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(AboutOrganisationController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutOrganisationController.ContactDetails)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be("contact-details");
        }

        [Test]
        public static void Post_ContactDetails_NullModel_ThrowsException()
        {
            var controller = new AboutOrganisationController(Mock.Of<ILogWrapper<AboutOrganisationController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.ContactDetails((ContactDetailsModel) null));
        }

        [Test]
        public static async Task Post_ContactDetails_InvalidModel_DoesNotCallService()
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutOrganisationController(Mock.Of<ILogWrapper<AboutOrganisationController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.ContactDetails(new Mock<ContactDetailsModel>().Object);

            mockService.Verify(x => x.SaveSupplierContacts(It.IsAny<SupplierContactsModel>()), Times.Never);
        }

        [Test]
        public static async Task Post_ContactDetails_InvalidModel_ReturnsViewWithModel()
        {
            var model = new Mock<ContactDetailsModel>().Object;
            var controller = new AboutOrganisationController(Mock.Of<ILogWrapper<AboutOrganisationController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.ContactDetails(model)).As<ViewResult>();

            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(model);
        }

        [Test]
        public static async Task Post_ContactDetails_ModelValid_ConvertsModelToSupplierContactsModel()
        {
            var contactDetailsModel = new Mock<ContactDetailsModel>().Object;
            var mapper = new Mock<IMapper>();
            var controller = new AboutOrganisationController(Mock.Of<ILogWrapper<AboutOrganisationController>>(),
                mapper.Object, Mock.Of<ISolutionsService>());

            await controller.ContactDetails(contactDetailsModel);

            mapper.Verify(x => x.Map<ContactDetailsModel, SupplierContactsModel>(contactDetailsModel));
        }

        [Test]
        public static async Task Post_ContactDetails_ModelValid_CallsSaveOnService()
        {
            var contactDetailsModel = new Mock<ContactDetailsModel>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mapper = new Mock<IMapper>();
            var supplierContactsModel = new Mock<SupplierContactsModel>().Object;
            mapper.Setup(x => x.Map<ContactDetailsModel, SupplierContactsModel>(contactDetailsModel))
                .Returns(supplierContactsModel);
            var controller = new AboutOrganisationController(Mock.Of<ILogWrapper<AboutOrganisationController>>(),
                mapper.Object, mockService.Object);

            await controller.ContactDetails(contactDetailsModel);

            mockService.Verify(x => x.SaveSupplierContacts(supplierContactsModel));
        }

        [Test]
        public static async Task Post_ContactDetails_ModelValid_RedirectToSolutionAction()
        {
            var contactDetailsModel = new ContactDetailsModel {SolutionId = new Fixture().Create<string>()};
            var controller = new AboutOrganisationController(Mock.Of<ILogWrapper<AboutOrganisationController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            var actual = (await controller.ContactDetails(contactDetailsModel))
                .As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be("Solution");
            actual.RouteValues["id"].Should().Be(contactDetailsModel.SolutionId);
        }
    }
}