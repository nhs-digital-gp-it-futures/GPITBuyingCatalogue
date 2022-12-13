using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class AddCatalogueSolutionControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AddCatalogueSolutionController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_SuppliersInDatabase_ReturnsViewWithExpectedModel(
            List<Supplier> suppliers,
            [Frozen] Mock<ISuppliersService> mockService,
            AddCatalogueSolutionController controller)
        {
            mockService.Setup(s => s.GetAllActiveSuppliers())
                .ReturnsAsync(suppliers);

            var actual = (await controller.Index()).As<ViewResult>();

            mockService.Verify(s => s.GetAllActiveSuppliers());
            actual.Should().NotBeNull();
            actual.ViewName.Should().Be("Details");
            actual.Model.As<SolutionModel>()
                .SuppliersSelectList.Should()
                .BeEquivalentTo(suppliers.Select(s => new SelectOption<string>($"{s.Name} ({s.Id})", s.Id.ToString(CultureInfo.InvariantCulture))));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Index_ModelStateValid_AddsExpectedCatalogueItem(
            SolutionModel model,
            Mock<ISolutionsService> mockService,
            Mock<ISuppliersService> mockSuppliersService,
            int userId)
        {
            var frameworks = new List<FrameworkModel> { new() { Name = "DFOCVC", Selected = true, FrameworkId = "DFOCVC001" } };

            model.Frameworks = frameworks;

            mockService.Setup(s => s.GetSolutionByName(It.IsAny<string>()))
                .ReturnsAsync((CatalogueItem)null);

            var controller = GetController(mockService, mockSuppliersService, userId);

            await controller.Index(model);

            mockService.Verify(
                s => s.AddCatalogueSolution(
                    It.Is<CreateSolutionModel>(
                        c => c.Frameworks == frameworks
                            && c.Name == model.SolutionName
                            && c.SupplierId == model.SupplierId
                            && c.UserId == userId)));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Index_ModelStateValid_RedirectsToManageCatalogueSolutions(
            SolutionModel model,
            Mock<ISolutionsService> mockService,
            Mock<ISuppliersService> mockSuppliersService)
        {
            mockService.Setup(s => s.GetSolutionByName(It.IsAny<string>()))
                .ReturnsAsync((CatalogueItem)null);

            var controller = GetController(mockService, mockSuppliersService, 17);

            var actual = (await controller.Index(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Index_ModelStateNotValid_GetsSuppliers_ReturnsViewWithModel(
            SolutionModel model,
            [Frozen] Mock<ISolutionsService> mockService,
            [Frozen] Mock<ISuppliersService> mockSuppliersService,
            AddCatalogueSolutionController controller,
            List<Supplier> suppliers)
        {
            mockSuppliersService.Setup(s => s.GetAllActiveSuppliers())
                .ReturnsAsync(suppliers);

            mockService.Setup(s => s.GetSolutionByName(It.IsAny<string>()))
                .ReturnsAsync((CatalogueItem)null);

            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.Index(model)).As<ViewResult>();

            mockSuppliersService.Verify(s => s.GetAllActiveSuppliers());
            actual.Should().NotBeNull();
            actual.ViewName.Should().Be("Details");
            actual.Model.Should().BeEquivalentTo(model.WithSelectListItems(suppliers));
        }

        private static AddCatalogueSolutionController GetController(
            IMock<ISolutionsService> mockService,
            IMock<ISuppliersService> mockSuppliersService,
            int userId)
        {
            return new AddCatalogueSolutionController(mockService.Object, mockSuppliersService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(
                            new ClaimsIdentity(
                                new Claim[] { new(Framework.Constants.CatalogueClaims.UserId, userId.ToString()) })),
                    },
                },
            };
        }
    }
}
