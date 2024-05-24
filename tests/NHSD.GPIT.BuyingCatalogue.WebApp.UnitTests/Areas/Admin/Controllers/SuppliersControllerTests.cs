using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class SuppliersControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SuppliersController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_GetsAllSuppliers(
            IList<Supplier> suppliers,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            SuppliersController controller)
        {
            mockSuppliersService.Setup(s => s.GetAllSuppliers(It.IsAny<string>())).ReturnsAsync(suppliers);

            await controller.Index();

            mockSuppliersService.Verify(o => o.GetAllSuppliers(It.IsAny<string>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ActiveSuppliers_DoesntShowInactiveItems(
            IList<Supplier> suppliers,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            SuppliersController controller)
        {
            suppliers.ToList().ForEach(s => s.IsActive = true);
            mockSuppliersService.Setup(s => s.GetAllSuppliers(It.IsAny<string>())).ReturnsAsync(suppliers);

            var result = (await controller.Index()).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.As<ManageSuppliersModel>().ShowInactiveItems.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_SearchTerm_DisableScripting(
            string searchTerm,
            IList<Supplier> suppliers,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            SuppliersController controller)
        {
            suppliers.ToList().ForEach(s => s.IsActive = true);
            suppliers.First().IsActive = false;
            mockSuppliersService.Setup(s => s.GetAllSuppliers(searchTerm)).ReturnsAsync(suppliers);

            var result = (await controller.Index(searchTerm)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.As<ManageSuppliersModel>().DisableScripting.Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ReturnsViewWithExpectedViewModel(
            IList<Supplier> suppliers,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            SuppliersController controller)
        {
            var expectedResult = new ManageSuppliersModel(suppliers);

            mockSuppliersService.Setup(o => o.GetAllSuppliers(It.IsAny<string>())).ReturnsAsync(suppliers);

            var actual = (await controller.Index()).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditSupplier_ReturnsViewWithExpectedViewModel(
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            SuppliersController controller)
        {
            var expectedResult = new EditSupplierModel(supplier)
            {
                BackLink = "testUrl",
            };

            mockSuppliersService.Setup(s => s.GetSupplier(It.IsAny<int>())).ReturnsAsync(supplier);

            var actual = (await controller.EditSupplier(It.IsAny<int>())).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedResult);

            mockSuppliersService.Verify(s => s.GetSupplier(It.IsAny<int>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSupplier_StatusNotChanged_ReturnsRedirectToActionResult(
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            EditSupplierModel model,
            SuppliersController controller)
        {
            supplier.IsActive = model.SupplierStatus;

            mockSuppliersService.Setup(s => s.GetSupplier(supplier.Id)).ReturnsAsync(supplier);

            var actual = (await controller.EditSupplier(supplier.Id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SuppliersController.Index));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSupplier_Activate_MissingInformation_ReturnsExpectedView(
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            SuppliersController controller)
        {
            supplier.IsActive = true;
            supplier.Address = null;
            supplier.Name = null;
            supplier.SupplierContacts = new List<SupplierContact>();

            var model = new EditSupplierModel(supplier);

            supplier.IsActive = false;

            mockSuppliersService.Setup(s => s.GetSupplier(supplier.Id)).ReturnsAsync(supplier);

            var actual = (await controller.EditSupplier(supplier.Id, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(model);

            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ErrorCount.Should().Be(1);
            controller.ModelState.Should().ContainKey(nameof(model.SupplierStatus));
            controller.ModelState[nameof(model.SupplierStatus)]?.Errors.Should().Contain(x => x.ErrorMessage == "Mandatory section incomplete");

            mockSuppliersService.Verify(s => s.GetSupplier(supplier.Id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSupplier_Deactivate_PublishedSolutions_ReturnsExpectedView(
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            SuppliersController controller)
        {
            supplier.IsActive = false;

            var model = new EditSupplierModel(supplier);

            supplier.IsActive = true;

            mockSuppliersService.Setup(s => s.GetSupplier(supplier.Id)).ReturnsAsync(supplier);
            mockSuppliersService.Setup(x => x.GetAllSolutionsForSupplier(supplier.Id))
                .ReturnsAsync(new List<CatalogueItem>() { new CatalogueItem() { PublishedStatus = PublicationStatus.Published } });

            var actual = (await controller.EditSupplier(supplier.Id, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(model);

            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ErrorCount.Should().Be(1);
            controller.ModelState.Should().ContainKey(nameof(model.SupplierStatus));
            controller.ModelState[nameof(model.SupplierStatus)]?.Errors.Should().Contain(x => x.ErrorMessage == "Cannot set to inactive while 1 solutions for this supplier are still published");

            mockSuppliersService.VerifyAll();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSupplier_UpdateActiveStatus_ReturnsRedirectToAction(
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            SuppliersController controller)
        {
            supplier.IsActive = false;

            var model = new EditSupplierModel(supplier);

            supplier.IsActive = true;

            mockSuppliersService.Setup(s => s.GetSupplier(supplier.Id)).ReturnsAsync(supplier);
            mockSuppliersService.Setup(x => x.GetAllSolutionsForSupplier(supplier.Id))
                .ReturnsAsync(new List<CatalogueItem>() { new CatalogueItem() { PublishedStatus = PublicationStatus.Draft } });
            mockSuppliersService.Setup(x => x.UpdateSupplierActiveStatus(supplier.Id, model.SupplierStatus))
                .ReturnsAsync(supplier);

            var actual = (await controller.EditSupplier(supplier.Id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SuppliersController.Index));
            mockSuppliersService.VerifyAll();
        }

        [Theory]
        [CommonAutoData]
        public static void Get_AddSupplierDetails_ReturnsViewWithExpectedViewModel(
            SuppliersController controller)
        {
            var expectedResult = new EditSupplierDetailsModel
            {
                BackLink = "testUrl",
            };

            var actual = controller.AddSupplierDetails().As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().Be("EditSupplierDetails");
            actual.Model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddSupplierDetails_AddsSupplier_RedirectsCorrectly(
            EditSupplierDetailsModel model,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            Supplier supplier,
            SuppliersController controller)
        {
            mockSuppliersService.Setup(s => s.GetSupplierByName(model.SupplierName)).ReturnsAsync((Supplier)null);
            mockSuppliersService.Setup(s => s.GetSupplierByLegalName(model.SupplierLegalName)).ReturnsAsync((Supplier)null);
            mockSuppliersService.Setup(s => s.AddSupplier(It.IsAny<ServiceContracts.Models.EditSupplierModel>())).ReturnsAsync(supplier);

            var actual = (await controller.AddSupplierDetails(model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(SuppliersController.EditSupplier));
            actual.ControllerName.Should().Be(typeof(SuppliersController).ControllerName());
            actual.RouteValues["supplierId"].Should().Be(supplier.Id);

            mockSuppliersService.Verify(s => s.AddSupplier(It.IsAny<ServiceContracts.Models.EditSupplierModel>()), Times.Once());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditSupplierDetails_ReturnsViewWithExpectedViewModel(
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            SuppliersController controller)
        {
            mockSuppliersService.Setup(s => s.GetSupplier(supplier.Id)).ReturnsAsync(supplier);

            var expectedResult = new EditSupplierDetailsModel(supplier)
            {
                BackLink = "testUrl",
            };

            var actual = (await controller.EditSupplierDetails(supplier.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSupplierDetails_SupplierNameExists_MatchesThisSupplier_RedirectsCorrectly(
            EditSupplierDetailsModel model,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            Supplier existingSupplier,
            SuppliersController controller)
        {
            mockSuppliersService.Setup(s => s.GetSupplierByName(model.SupplierName)).ReturnsAsync(existingSupplier);
            mockSuppliersService.Setup(s => s.GetSupplierByLegalName(model.SupplierLegalName)).ReturnsAsync((Supplier)null);
            mockSuppliersService.Setup(s => s.EditSupplierDetails(existingSupplier.Id, It.IsAny<ServiceContracts.Models.EditSupplierModel>())).ReturnsAsync(existingSupplier);

            var actual = (await controller.EditSupplierDetails(existingSupplier.Id, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(SuppliersController.EditSupplier));
            actual.ControllerName.Should().Be(typeof(SuppliersController).ControllerName());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSupplierDetails_SupplierLegalNameExists_MatchesThisSupplier_RedirectsCorrectly(
            EditSupplierDetailsModel model,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            Supplier existingSupplier,
            SuppliersController controller)
        {
            mockSuppliersService.Setup(s => s.GetSupplierByName(model.SupplierName)).ReturnsAsync((Supplier)null);
            mockSuppliersService.Setup(s => s.GetSupplierByLegalName(model.SupplierLegalName)).ReturnsAsync(existingSupplier);
            mockSuppliersService.Setup(s => s.EditSupplierDetails(existingSupplier.Id, It.IsAny<ServiceContracts.Models.EditSupplierModel>())).ReturnsAsync(existingSupplier);

            var actual = (await controller.EditSupplierDetails(existingSupplier.Id, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(SuppliersController.EditSupplier));
            actual.ControllerName.Should().Be(typeof(SuppliersController).ControllerName());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditSupplierAddress_ReturnsViewWithExpectedViewModel(
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            SuppliersController controller)
        {
            mockSuppliersService.Setup(s => s.GetSupplier(supplier.Id)).ReturnsAsync(supplier);

            var expectedResult = new EditSupplierAddressModel(supplier);

            var actual = (await controller.EditSupplierAddress(supplier.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSupplierAddress_EditsAddress_RedirectsCorrectly(
            EditSupplierAddressModel model,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            Supplier supplier,
            SuppliersController controller)
        {
            mockSuppliersService.Setup(s => s.EditSupplierAddress(supplier.Id, It.IsAny<Address>())).ReturnsAsync(supplier);

            var actual = (await controller.EditSupplierAddress(supplier.Id, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(SuppliersController.EditSupplier));
            actual.ControllerName.Should().Be(typeof(SuppliersController).ControllerName());
            actual.RouteValues["supplierId"].Should().Be(supplier.Id);

            mockSuppliersService.Verify(s => s.EditSupplierAddress(supplier.Id, It.IsAny<Address>()), Times.Once());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ManageSupplierContacts_ReturnsViewWithExpectedViewModel(
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            SuppliersController controller)
        {
            mockSuppliersService.Setup(s => s.GetSupplier(supplier.Id)).ReturnsAsync(supplier);

            var expectedResult = new ManageSupplierContactsModel(supplier)
            {
                BackLink = "testUrl",
            };

            var actual = (await controller.ManageSupplierContacts(supplier.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddSupplierContact_ReturnsViewWithExpectedViewModel(
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            SuppliersController controller)
        {
            mockSuppliersService.Setup(s => s.GetSupplier(supplier.Id)).ReturnsAsync(supplier);

            var expectedResult = new EditContactModel(supplier)
            {
                BackLink = "testUrl",
            };

            var actual = (await controller.AddSupplierContact(supplier.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().Be("EditSupplierContact");
            actual.Model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddSupplierContact_AddsContact_RedirectsCorrectly(
            EditContactModel model,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            Supplier supplier,
            SuppliersController controller)
        {
            mockSuppliersService.Setup(s => s.GetSupplier(supplier.Id)).ReturnsAsync(supplier);
            mockSuppliersService.Setup(s => s.AddSupplierContact(supplier.Id, It.IsAny<SupplierContact>())).ReturnsAsync(supplier);

            var actual = (await controller.AddSupplierContact(supplier.Id, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(SuppliersController.ManageSupplierContacts));
            actual.ControllerName.Should().Be(typeof(SuppliersController).ControllerName());
            actual.RouteValues["supplierId"].Should().Be(supplier.Id);

            mockSuppliersService.Verify(s => s.AddSupplierContact(supplier.Id, It.IsAny<SupplierContact>()), Times.Once());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditSupplierContact_ReturnsViewWithExpectedViewModel(
            Supplier supplier,
            List<CatalogueItem> solutions,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            SuppliersController controller)
        {
            mockSuppliersService.Setup(s => s.GetSupplier(supplier.Id)).ReturnsAsync(supplier);

            mockSuppliersService.Setup(s => s.GetSolutionsReferencingSupplierContact(supplier.SupplierContacts.First().Id))
                .ReturnsAsync(solutions);

            var expectedResult = new EditContactModel(supplier.SupplierContacts.First(), supplier, solutions)
            {
                BackLink = "testUrl",
            };

            var actual = (await controller.EditSupplierContact(supplier.Id, supplier.SupplierContacts.First().Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSupplierContact_InvalidViewModel_ReturnsViewWithExpectedViewModel(
            Supplier supplier,
            List<CatalogueItem> solutions,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            EditContactModel model,
            SuppliersController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            mockSuppliersService.Setup(s => s.GetSolutionsReferencingSupplierContact(supplier.SupplierContacts.First().Id))
                .ReturnsAsync(solutions);

            var actual = (await controller.EditSupplierContact(supplier.Id, supplier.SupplierContacts.First().Id, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(x => x.SolutionsReferencingThisContact));
            actual.Model.As<EditContactModel>().SolutionsReferencingThisContact.Should().BeEquivalentTo(solutions);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSupplierContact_ReturnsRedirectToAction(
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            EditContactModel model,
            SuppliersController controller)
        {
            var updatedContact = new SupplierContact
            {
                Id = model.ContactId!.Value,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Department = model.Department,
            };

            mockSuppliersService.Setup(s => s.EditSupplierContact(supplier.Id, supplier.SupplierContacts.First().Id, updatedContact))
                .ReturnsAsync(supplier);

            var actual = (await controller.EditSupplierContact(supplier.Id, supplier.SupplierContacts.First().Id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SuppliersController.ManageSupplierContacts));
            actual.RouteValues["supplierId"].Should().Be(supplier.Id);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteSupplierContact_ReturnsViewWithExpectedViewModel(
            Supplier supplier,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            SuppliersController controller)
        {
            mockSuppliersService.Setup(s => s.GetSupplier(supplier.Id)).ReturnsAsync(supplier);

            var expectedResult = new DeleteContactModel(supplier.SupplierContacts.First(), supplier.Name)
            {
                BackLink = "testUrl",
            };

            var actual = (await controller.DeleteSupplierContact(supplier.Id, supplier.SupplierContacts.First().Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteSupplierContact_EditsContact_RedirectsCorrectly(
            DeleteContactModel model,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            Supplier supplier,
            SuppliersController controller)
        {
            mockSuppliersService.Setup(s => s.DeleteSupplierContact(supplier.Id, It.IsAny<int>())).ReturnsAsync(supplier);

            var actual = (await controller.DeleteSupplierContact(supplier.Id, supplier.SupplierContacts.First().Id, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(SuppliersController.ManageSupplierContacts));
            actual.ControllerName.Should().Be(typeof(SuppliersController).ControllerName());
            actual.RouteValues["supplierId"].Should().Be(supplier.Id);

            mockSuppliersService.Verify(s => s.DeleteSupplierContact(supplier.Id, supplier.SupplierContacts.First().Id), Times.Once());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_FilterSearchSuggestions_ReturnsResults(
            string searchTerm,
            List<Supplier> searchResults,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            SuppliersController controller)
        {
            var expected = searchResults.Select(r => new HtmlEncodedSuggestionSearchResult(
                r.Name,
                r.Id.ToString(),
                "testUrl"));

            var supplier = searchResults.First();

            mockSuppliersService.Setup(s => s.GetSuppliersBySearchTerm(searchTerm))
                .ReturnsAsync(searchResults);

            var result = (await controller.FilterSearchSuggestions(searchTerm)).As<JsonResult>();

            result.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(expected, opt => opt.ExcludingMissingMembers());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_FilterSearchSuggestions_NoResults(
            string searchTerm,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            SuppliersController controller)
        {
            controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                Request =
                {
                    Headers =
                    {
                        Referer = "http://www.test.com",
                    },
                },
            };

            mockSuppliersService.Setup(s => s.GetSuppliersBySearchTerm(searchTerm))
                .ReturnsAsync(Array.Empty<Supplier>());

            var result = (await controller.FilterSearchSuggestions(searchTerm)).As<JsonResult>();

            result.Should().NotBeNull();
            result.Value.As<IEnumerable<HtmlEncodedSuggestionSearchResult>>().Should().BeEmpty();
        }
    }
}
