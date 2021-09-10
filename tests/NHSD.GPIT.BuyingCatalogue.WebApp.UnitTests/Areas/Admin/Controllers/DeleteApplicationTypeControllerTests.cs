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
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
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
        public static void Get_DeleteApplicationTypeConfirmation_ValidId_ReturnsViewWithExpectedModel(
         CatalogueItem catalogueItem,
         ClientApplicationType clientApplicationType,
         DeleteApplicationTypeController controller)
        {
            var actual = controller.DeleteApplicationTypeConfirmation(catalogueItem.Id, clientApplicationType).As<ViewResult>();

            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new DeleteApplicationTypeConfirmationModel(catalogueItem.Id, clientApplicationType));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteApplicationTypeConfirmation_Saves_And_RedirectsToDesktop(
            CatalogueItemId catalogueItemId,
            ClientApplicationType clientApplicationType,
            DeleteApplicationTypeConfirmationModel model,
            ClientApplication clientApplication,
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
    }
}
