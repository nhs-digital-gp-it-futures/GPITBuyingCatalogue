using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class AddCatalogueSolutionControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AddCatalogueSolutionController).Should()
                .BeDecoratedWith<AuthorizeAttribute>(x => x.Policy == "AdminOnly");

            typeof(AddCatalogueSolutionController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Admin");

            typeof(AddCatalogueSolutionController).Should()
                .BeDecoratedWith<RouteAttribute>(x => x.Template == "admin/catalogue-solutions/add-solution");
        }

        [Fact]
        public static void Constructor_NullService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                    () =>
                        _ = new AddCatalogueSolutionController(null))
                .ParamName.Should()
                .Be("solutionsService");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_SuppliersInDatabase_ReturnsViewWithExpectedModel(
            List<Supplier> suppliers,
            Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetAllSuppliers())
                .ReturnsAsync(suppliers);
            var controller = new AddCatalogueSolutionController(mockService.Object);

            var actual = (await controller.Index()).As<ViewResult>();

            mockService.Verify(s => s.GetAllSuppliers());
            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.As<AddSolutionModel>()
                .SuppliersSelectList.Should()
                .BeEquivalentTo(suppliers.Select(s => new SelectListItem($"{s.Name} ({s.Id})", s.Id)));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Index_ModelStateValid_AddsExpectedCatalogueItem(
            AddSolutionModel model,
            Mock<ISolutionsService> mockService,
            Guid userId)
        {
            var frameworks = new List<FrameworkModel> { new() { Name = "DFOCVC", Selected = true, FrameworkId = "DFOCVC001" } };

            model.Frameworks = frameworks;
            var controller = GetController(mockService, userId);

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
            AddSolutionModel model,
            Mock<ISolutionsService> mockService)
        {
            var controller = GetController(mockService, Guid.NewGuid());

            var actual = (await controller.Index(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Index_ModelStateNotValid_GetsSuppliers_ReturnsViewWithModel(
            AddSolutionModel model,
            Mock<ISolutionsService> mockService,
            List<Supplier> suppliers)
        {
            mockService.Setup(s => s.GetAllSuppliers())
                .ReturnsAsync(suppliers);
            var controller = new AddCatalogueSolutionController(mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.Index(model)).As<ViewResult>();

            mockService.Verify(s => s.GetAllSuppliers());
            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(model.WithSelectListItems(suppliers));
        }

        private static AddCatalogueSolutionController GetController(
            IMock<ISolutionsService> mockService,
            Guid userId)
        {
            return new(mockService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(
                            new ClaimsIdentity(
                                new Claim[] { new(Framework.Constants.Claims.UserId, userId.ToString()) })),
                    },
                },
            };
        }
    }
}
