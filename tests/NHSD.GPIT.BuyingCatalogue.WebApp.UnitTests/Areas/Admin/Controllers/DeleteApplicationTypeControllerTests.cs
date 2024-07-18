using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DeleteApplicationTypeModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class DeleteApplicationTypeControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(DeleteApplicationTypeController).Should()
                .BeDecoratedWith<AuthorizeAttribute>(a => a.Policy == "AdminOnly");
            typeof(DeleteApplicationTypeController).Should()
                .BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Admin");
            typeof(DeleteApplicationTypeController).Should()
                .BeDecoratedWith<RouteAttribute>(r => r.Template == "admin/catalogue-solutions");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(DeleteApplicationTypeController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteApplicationTypeConfirmation_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            ApplicationType applicationType,
            [Frozen] ISolutionsService mockService,
            DeleteApplicationTypeController controller)
        {
            mockService.GetSolutionThin(Arg.Any<CatalogueItemId>()).Returns(catalogueItem);

            var actual = (await controller.DeleteApplicationTypeConfirmation(catalogueItem.Id, applicationType)).As<ViewResult>();

            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new DeleteApplicationTypeConfirmationModel(catalogueItem, applicationType), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteApplicationTypeConfirmation_InvalidId_ReturnsBadRequest(
            CatalogueItem catalogueItem,
            ApplicationType applicationType,
            [Frozen] ISolutionsService mockService,
            DeleteApplicationTypeController controller)
        {
            mockService.GetSolutionThin(Arg.Any<CatalogueItemId>()).Returns((CatalogueItem)null);

            var actual = (await controller.DeleteApplicationTypeConfirmation(catalogueItem.Id, applicationType)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().BeOfType<string>();
            actual.Value.As<string>().Should().Be($"No Solution found for Id: {catalogueItem.Id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteApplicationTypeConfirmation_Saves_And_RedirectsToDesktop(
            CatalogueItemId catalogueItemId,
            ApplicationType applicationType,
            DeleteApplicationTypeConfirmationModel model,
            ApplicationTypeDetail applicationTypeDetail,
            [Frozen] ISolutionsService mockService,
            DeleteApplicationTypeController controller)
        {
            mockService.GetApplicationType(catalogueItemId).Returns(applicationTypeDetail);

            var actual = (await controller.DeleteApplicationTypeConfirmation(catalogueItemId, applicationType, model)).As<RedirectToActionResult>();

            await mockService.Received().DeleteApplicationType(catalogueItemId, applicationType);
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ApplicationType));
            actual.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteApplicationTypeConfirmation_InvalidModelState_ReturnsView(
            string errorKey,
            string errorMessage,
            CatalogueItemId catalogueItemId,
            ApplicationType applicationType,
            DeleteApplicationTypeConfirmationModel model,
            DeleteApplicationTypeController controller)
        {
            controller.ModelState.AddModelError(errorKey, errorMessage);

            var actual = (await controller.DeleteApplicationTypeConfirmation(catalogueItemId, applicationType, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.ViewData.ModelState.IsValid.Should().BeFalse();
            actual.ViewData.ModelState.ErrorCount.Should().Be(1);
            actual.ViewData.ModelState.Keys.First().Should().Be(errorKey);
            actual.ViewData.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be(errorMessage);
        }
    }
}
