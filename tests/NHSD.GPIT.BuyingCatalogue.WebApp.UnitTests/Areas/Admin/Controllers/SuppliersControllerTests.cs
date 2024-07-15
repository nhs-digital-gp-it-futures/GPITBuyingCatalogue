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
        [MockAutoData]
        public static async Task Get_Index_GetsAllSuppliers(
            IList<Supplier> suppliers,
            [Frozen] ISuppliersService mockSuppliersService,
            SuppliersController controller)
        {
            mockSuppliersService.GetAllSuppliers(Arg.Any<string>()).Returns(suppliers);

            await controller.Index();

            await mockSuppliersService.Received().GetAllSuppliers(Arg.Any<string>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_ActiveSuppliers_DoesntShowInactiveItems(
            IList<Supplier> suppliers,
            [Frozen] ISuppliersService mockSuppliersService,
            SuppliersController controller)
        {
            suppliers.ToList().ForEach(s => s.IsActive = true);
            mockSuppliersService.GetAllSuppliers(Arg.Any<string>()).Returns(suppliers);

            var result = (await controller.Index()).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.As<ManageSuppliersModel>().ShowInactiveItems.Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_SearchTerm_DisableScripting(
            string searchTerm,
            IList<Supplier> suppliers,
            [Frozen] ISuppliersService mockSuppliersService,
            SuppliersController controller)
        {
            suppliers.ToList().ForEach(s => s.IsActive = true);
            suppliers.First().IsActive = false;
            mockSuppliersService.GetAllSuppliers(searchTerm).Returns(suppliers);

            var result = (await controller.Index(searchTerm)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.As<ManageSuppliersModel>().DisableScripting.Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_ReturnsViewWithExpectedViewModel(
            IList<Supplier> suppliers,
            [Frozen] ISuppliersService mockSuppliersService,
            SuppliersController controller)
        {
            var expectedResult = new ManageSuppliersModel(suppliers);

            mockSuppliersService.GetAllSuppliers(Arg.Any<string>()).Returns(suppliers);

            var actual = (await controller.Index()).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditSupplier_ReturnsViewWithExpectedViewModel(
            Supplier supplier,
            [Frozen] ISuppliersService mockSuppliersService,
            SuppliersController controller)
        {
            var expectedResult = new EditSupplierModel(supplier)
            {
                BackLink = "testUrl",
            };

            mockSuppliersService.GetSupplier(supplier.Id).Returns(supplier);

            var actual = (await controller.EditSupplier(supplier.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedResult);

            await mockSuppliersService.Received().GetSupplier(Arg.Any<int>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSupplier_StatusNotChanged_ReturnsRedirectToActionResult(
            Supplier supplier,
            [Frozen] ISuppliersService mockSuppliersService,
            EditSupplierModel model,
            SuppliersController controller)
        {
            supplier.IsActive = model.SupplierStatus;

            mockSuppliersService.GetSupplier(supplier.Id).Returns(supplier);

            var actual = (await controller.EditSupplier(supplier.Id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SuppliersController.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSupplier_Activate_MissingInformation_ReturnsExpectedView(
            Supplier supplier,
            [Frozen] ISuppliersService mockSuppliersService,
            SuppliersController controller)
        {
            supplier.IsActive = true;
            supplier.Address = null;
            supplier.Name = null;
            supplier.SupplierContacts = new List<SupplierContact>();

            var model = new EditSupplierModel(supplier);

            supplier.IsActive = false;

            mockSuppliersService.GetSupplier(supplier.Id).Returns(supplier);

            var actual = (await controller.EditSupplier(supplier.Id, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(model);

            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ErrorCount.Should().Be(1);
            controller.ModelState.Should().ContainKey(nameof(model.SupplierStatus));
            controller.ModelState[nameof(model.SupplierStatus)]?.Errors.Should().Contain(x => x.ErrorMessage == "Mandatory section incomplete");

            await mockSuppliersService.Received().GetSupplier(supplier.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSupplier_Deactivate_PublishedSolutions_ReturnsExpectedView(
            Supplier supplier,
            [Frozen] ISuppliersService mockSuppliersService,
            SuppliersController controller)
        {
            supplier.IsActive = false;

            var model = new EditSupplierModel(supplier);

            supplier.IsActive = true;

            mockSuppliersService.GetSupplier(supplier.Id).Returns(supplier);
            mockSuppliersService.GetAllSolutionsForSupplier(supplier.Id).Returns(new List<CatalogueItem>() { new CatalogueItem() { PublishedStatus = PublicationStatus.Published } });

            var actual = (await controller.EditSupplier(supplier.Id, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(model);

            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.ErrorCount.Should().Be(1);
            controller.ModelState.Should().ContainKey(nameof(model.SupplierStatus));
            controller.ModelState[nameof(model.SupplierStatus)]?.Errors.Should().Contain(x => x.ErrorMessage == "Cannot set to inactive while 1 solutions for this supplier are still published");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSupplier_UpdateActiveStatus_ReturnsRedirectToAction(
            Supplier supplier,
            [Frozen] ISuppliersService mockSuppliersService,
            SuppliersController controller)
        {
            supplier.IsActive = false;

            var model = new EditSupplierModel(supplier);

            supplier.IsActive = true;

            mockSuppliersService.GetSupplier(supplier.Id).Returns(supplier);
            mockSuppliersService.GetAllSolutionsForSupplier(supplier.Id).Returns(new List<CatalogueItem>() { new CatalogueItem() { PublishedStatus = PublicationStatus.Draft } });
            mockSuppliersService.UpdateSupplierActiveStatus(supplier.Id, model.SupplierStatus).Returns(supplier);

            var actual = (await controller.EditSupplier(supplier.Id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SuppliersController.Index));
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Post_AddSupplierDetails_AddsSupplier_RedirectsCorrectly(
            EditSupplierDetailsModel model,
            [Frozen] ISuppliersService mockSuppliersService,
            Supplier supplier,
            SuppliersController controller)
        {
            mockSuppliersService.GetSupplierByName(model.SupplierName).Returns((Supplier)null);
            mockSuppliersService.GetSupplierByLegalName(model.SupplierLegalName).Returns((Supplier)null);
            mockSuppliersService.AddSupplier(Arg.Any<ServiceContracts.Models.EditSupplierModel>()).Returns(supplier);

            var actual = (await controller.AddSupplierDetails(model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(SuppliersController.EditSupplier));
            actual.ControllerName.Should().Be(typeof(SuppliersController).ControllerName());
            actual.RouteValues["supplierId"].Should().Be(supplier.Id);

            await mockSuppliersService.Received().AddSupplier(Arg.Any<ServiceContracts.Models.EditSupplierModel>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditSupplierDetails_ReturnsViewWithExpectedViewModel(
            Supplier supplier,
            [Frozen] ISuppliersService mockSuppliersService,
            SuppliersController controller)
        {
            mockSuppliersService.GetSupplier(supplier.Id).Returns(supplier);

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
        [MockAutoData]
        public static async Task Post_EditSupplierDetails_SupplierNameExists_MatchesThisSupplier_RedirectsCorrectly(
            EditSupplierDetailsModel model,
            [Frozen] ISuppliersService mockSuppliersService,
            Supplier existingSupplier,
            SuppliersController controller)
        {
            mockSuppliersService.GetSupplierByName(model.SupplierName).Returns(existingSupplier);
            mockSuppliersService.GetSupplierByLegalName(model.SupplierLegalName).Returns((Supplier)null);
            mockSuppliersService.EditSupplierDetails(existingSupplier.Id, Arg.Any<ServiceContracts.Models.EditSupplierModel>()).Returns(existingSupplier);

            var actual = (await controller.EditSupplierDetails(existingSupplier.Id, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(SuppliersController.EditSupplier));
            actual.ControllerName.Should().Be(typeof(SuppliersController).ControllerName());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSupplierDetails_SupplierLegalNameExists_MatchesThisSupplier_RedirectsCorrectly(
            EditSupplierDetailsModel model,
            [Frozen] ISuppliersService mockSuppliersService,
            Supplier existingSupplier,
            SuppliersController controller)
        {
            mockSuppliersService.GetSupplierByName(model.SupplierName).Returns((Supplier)null);
            mockSuppliersService.GetSupplierByLegalName(model.SupplierLegalName).Returns(existingSupplier);
            mockSuppliersService.EditSupplierDetails(existingSupplier.Id, Arg.Any<ServiceContracts.Models.EditSupplierModel>()).Returns(existingSupplier);

            var actual = (await controller.EditSupplierDetails(existingSupplier.Id, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(SuppliersController.EditSupplier));
            actual.ControllerName.Should().Be(typeof(SuppliersController).ControllerName());
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditSupplierAddress_ReturnsViewWithExpectedViewModel(
            Supplier supplier,
            [Frozen] ISuppliersService mockSuppliersService,
            SuppliersController controller)
        {
            mockSuppliersService.GetSupplier(supplier.Id).Returns(supplier);

            var expectedResult = new EditSupplierAddressModel(supplier);

            var actual = (await controller.EditSupplierAddress(supplier.Id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSupplierAddress_EditsAddress_RedirectsCorrectly(
            EditSupplierAddressModel model,
            [Frozen] ISuppliersService mockSuppliersService,
            Supplier supplier,
            SuppliersController controller)
        {
            mockSuppliersService.EditSupplierAddress(supplier.Id, Arg.Any<Address>()).Returns(supplier);

            var actual = (await controller.EditSupplierAddress(supplier.Id, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(SuppliersController.EditSupplier));
            actual.ControllerName.Should().Be(typeof(SuppliersController).ControllerName());
            actual.RouteValues["supplierId"].Should().Be(supplier.Id);

            await mockSuppliersService.Received().EditSupplierAddress(supplier.Id, Arg.Any<Address>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ManageSupplierContacts_ReturnsViewWithExpectedViewModel(
            Supplier supplier,
            [Frozen] ISuppliersService mockSuppliersService,
            SuppliersController controller)
        {
            mockSuppliersService.GetSupplier(supplier.Id).Returns(supplier);

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
        [MockAutoData]
        public static async Task Get_AddSupplierContact_ReturnsViewWithExpectedViewModel(
            Supplier supplier,
            [Frozen] ISuppliersService mockSuppliersService,
            SuppliersController controller)
        {
            mockSuppliersService.GetSupplier(supplier.Id).Returns(supplier);

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
        [MockAutoData]
        public static async Task Post_AddSupplierContact_AddsContact_RedirectsCorrectly(
            EditContactModel model,
            [Frozen] ISuppliersService mockSuppliersService,
            Supplier supplier,
            SuppliersController controller)
        {
            mockSuppliersService.GetSupplier(supplier.Id).Returns(supplier);
            mockSuppliersService.AddSupplierContact(supplier.Id, Arg.Any<SupplierContact>()).Returns(supplier);

            var actual = (await controller.AddSupplierContact(supplier.Id, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(SuppliersController.ManageSupplierContacts));
            actual.ControllerName.Should().Be(typeof(SuppliersController).ControllerName());
            actual.RouteValues["supplierId"].Should().Be(supplier.Id);

            await mockSuppliersService.Received().AddSupplierContact(supplier.Id, Arg.Any<SupplierContact>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditSupplierContact_ReturnsViewWithExpectedViewModel(
            Supplier supplier,
            List<CatalogueItem> solutions,
            [Frozen] ISuppliersService mockSuppliersService,
            SuppliersController controller)
        {
            mockSuppliersService.GetSupplier(supplier.Id).Returns(supplier);

            mockSuppliersService.GetSolutionsReferencingSupplierContact(supplier.SupplierContacts.First().Id).Returns(solutions);

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
        [MockAutoData]
        public static async Task Post_EditSupplierContact_InvalidViewModel_ReturnsViewWithExpectedViewModel(
            Supplier supplier,
            List<CatalogueItem> solutions,
            [Frozen] ISuppliersService mockSuppliersService,
            EditContactModel model,
            SuppliersController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            mockSuppliersService.GetSolutionsReferencingSupplierContact(supplier.SupplierContacts.First().Id).Returns(solutions);

            var actual = (await controller.EditSupplierContact(supplier.Id, supplier.SupplierContacts.First().Id, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(x => x.SolutionsReferencingThisContact));
            actual.Model.As<EditContactModel>().SolutionsReferencingThisContact.Should().BeEquivalentTo(solutions);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSupplierContact_ReturnsRedirectToAction(
            Supplier supplier,
            [Frozen] ISuppliersService mockSuppliersService,
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

            mockSuppliersService.EditSupplierContact(supplier.Id, supplier.SupplierContacts.First().Id, updatedContact).Returns(supplier);

            var actual = (await controller.EditSupplierContact(supplier.Id, supplier.SupplierContacts.First().Id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SuppliersController.ManageSupplierContacts));
            actual.RouteValues["supplierId"].Should().Be(supplier.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteSupplierContact_ReturnsViewWithExpectedViewModel(
            Supplier supplier,
            [Frozen] ISuppliersService mockSuppliersService,
            SuppliersController controller)
        {
            mockSuppliersService.GetSupplier(supplier.Id).Returns(supplier);

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
        [MockAutoData]
        public static async Task Post_DeleteSupplierContact_EditsContact_RedirectsCorrectly(
            DeleteContactModel model,
            [Frozen] ISuppliersService mockSuppliersService,
            Supplier supplier,
            SuppliersController controller)
        {
            mockSuppliersService.DeleteSupplierContact(supplier.Id, Arg.Any<int>()).Returns(supplier);

            var actual = (await controller.DeleteSupplierContact(supplier.Id, supplier.SupplierContacts.First().Id, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(SuppliersController.ManageSupplierContacts));
            actual.ControllerName.Should().Be(typeof(SuppliersController).ControllerName());
            actual.RouteValues["supplierId"].Should().Be(supplier.Id);

            await mockSuppliersService.Received().DeleteSupplierContact(supplier.Id, supplier.SupplierContacts.First().Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_FilterSearchSuggestions_ReturnsResults(
            string searchTerm,
            List<Supplier> searchResults,
            [Frozen] ISuppliersService mockSuppliersService,
            SuppliersController controller)
        {
            var expected = searchResults.Select(r => new HtmlEncodedSuggestionSearchResult(
                r.Name,
                r.Id.ToString(),
                "testUrl"));

            var supplier = searchResults.First();

            mockSuppliersService.GetSuppliersBySearchTerm(searchTerm).Returns(searchResults);

            var result = (await controller.FilterSearchSuggestions(searchTerm)).As<JsonResult>();

            result.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(expected, opt => opt.ExcludingMissingMembers());
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_FilterSearchSuggestions_NoResults(
            string searchTerm,
            [Frozen] ISuppliersService mockSuppliersService,
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

            mockSuppliersService.GetSuppliersBySearchTerm(searchTerm).Returns(Array.Empty<Supplier>());

            var result = (await controller.FilterSearchSuggestions(searchTerm)).As<JsonResult>();

            result.Should().NotBeNull();
            result.Value.As<IEnumerable<HtmlEncodedSuggestionSearchResult>>().Should().BeEmpty();
        }
    }
}
