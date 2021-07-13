using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class SolutionSectionsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(SolutionSectionsController).Should()
                .BeDecoratedWith<AuthorizeAttribute>(x => x.Policy == "AdminOnly");
            typeof(SolutionSectionsController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Admin");
            typeof(SolutionSectionsController).Should()
                .BeDecoratedWith<RouteAttribute>(x => x.Template == "admin/catalogue-solutions/manage/{solutionId}");
        }

        [Fact]
        public static void Constructor_NullService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                    () =>
                        _ = new SolutionSectionsController(null))
                .ParamName.Should()
                .Be("solutionsService");
        }

        [Fact]
        public static async Task Get_EditDescription_ReturnsViewWithExpectedModel()
        {
            var expected = new CatalogueItem
            {
                CatalogueItemId = new EntityFramework.Ordering.Models.CatalogueItemId(999999, "999"),
                Solution = new Solution
                {
                    Summary = "XYZ Summary",
                    FullDescription = "XYZ description",
                    AboutUrl = "Fake url",
                },
                Name = "Fake Solution",
            };

            var mockSolutionService = new Mock<ISolutionsService>();
            mockSolutionService.Setup(s => s.GetSolution(It.IsAny<EntityFramework.Ordering.Models.CatalogueItemId>()))
                .ReturnsAsync(expected);

            var controller = new SolutionSectionsController(mockSolutionService.Object);

            var actual = (await controller.EditDescription(expected.CatalogueItemId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            var model = actual.Model.As<EditDescriptionModel>();
            model.SolutionName.Should().BeEquivalentTo("Fake Solution");
            model.Summary.Should().BeEquivalentTo("XYZ Summary");
            model.Description.Should().BeEquivalentTo("XYZ description");
            model.Link.Should().BeEquivalentTo("Fake url");
        }

        [Fact]
        public static void Post_EditDescription_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(SolutionSectionsController)
                .GetMethods()
                .First(x => x.Name == nameof(SolutionSectionsController.EditDescription)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(SolutionSectionsController.EditDescription).ToLowerCaseHyphenated());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SolutionDescription_InvalidModel_DoesNotCallService([Frozen] CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionSectionsController(mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.EditDescription(id, new Mock<EditDescriptionModel>().Object);

            mockService.Verify(
                s => s.SaveSolutionDescription(It.IsAny<CatalogueItemId>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditDescription_InvalidModel_ReturnsViewWithModel([Frozen] CatalogueItemId id)
        {
            var mockEditDescriptionModel = new Mock<EditDescriptionModel>().Object;
            var controller = new SolutionSectionsController(Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.EditDescription(id, mockEditDescriptionModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockEditDescriptionModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditDescription_ValidModel_CallsSaveSolutionDescriptionOnService(
            [Frozen] CatalogueItemId id,
            EditDescriptionModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionSectionsController(mockService.Object);

            await controller.EditDescription(id, model);

            mockService.Verify(s => s.SaveSolutionDescription(id, model.Summary, model.Description, model.Link));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditDescription_ValidModel_RedirectsToExpectedAction(
            [Frozen] CatalogueItemId id,
            EditDescriptionModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionSectionsController(mockService.Object);

            var actual = (await controller.EditDescription(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.ControllerName.Should().Be("CatalogueSolutions");
            actual.RouteValues["solutionId"].Should().Be(model.SolutionId);
        }
    }
}
