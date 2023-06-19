using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(DeleteApplicationTypeController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteApplicationTypeConfirmation_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            ApplicationType clientApplicationType,
            [Frozen] Mock<ISolutionsService> mockService,
            DeleteApplicationTypeController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(It.IsAny<CatalogueItemId>())).ReturnsAsync(catalogueItem);

            var actual = (await controller.DeleteApplicationTypeConfirmation(catalogueItem.Id, clientApplicationType)).As<ViewResult>();

            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new DeleteApplicationTypeConfirmationModel(catalogueItem, clientApplicationType), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteApplicationTypeConfirmation_InvalidId_ReturnsBadRequest(
            CatalogueItem catalogueItem,
            ApplicationType clientApplicationType,
            [Frozen] Mock<ISolutionsService> mockService,
            DeleteApplicationTypeController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(It.IsAny<CatalogueItemId>())).ReturnsAsync((CatalogueItem)null);

            var actual = (await controller.DeleteApplicationTypeConfirmation(catalogueItem.Id, clientApplicationType)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().BeOfType<string>();
            actual.Value.As<string>().Should().Be($"No Solution found for Id: {catalogueItem.Id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteApplicationTypeConfirmation_Saves_And_RedirectsToDesktop(
            CatalogueItemId catalogueItemId,
            ApplicationType clientApplicationType,
            DeleteApplicationTypeConfirmationModel model,
            ApplicationTypeDetail clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            DeleteApplicationTypeController controller)
        {
            mockService.Setup(s => s.GetClientApplication(catalogueItemId)).ReturnsAsync(clientApplication);

            var actual = (await controller.DeleteApplicationTypeConfirmation(catalogueItemId, clientApplicationType, model)).As<RedirectToActionResult>();

            mockService.Verify(s => s.DeleteClientApplication(catalogueItemId, clientApplicationType));
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ClientApplicationType));
            actual.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteApplicationTypeConfirmation_InvalidModelState_ReturnsView(
            string errorKey,
            string errorMessage,
            CatalogueItemId catalogueItemId,
            ApplicationType clientApplicationType,
            DeleteApplicationTypeConfirmationModel model,
            DeleteApplicationTypeController controller)
        {
            controller.ModelState.AddModelError(errorKey, errorMessage);

            var actual = (await controller.DeleteApplicationTypeConfirmation(catalogueItemId, clientApplicationType, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.ViewData.ModelState.IsValid.Should().BeFalse();
            actual.ViewData.ModelState.ErrorCount.Should().Be(1);
            actual.ViewData.ModelState.Keys.First().Should().Be(errorKey);
            actual.ViewData.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be(errorMessage);
        }
    }
}
