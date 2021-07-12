using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnumsNET;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;
using PublicationStatus = NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models.PublicationStatus;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class CatalogueSolutionsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(CatalogueSolutionsController).Should()
                .BeDecoratedWith<AuthorizeAttribute>(x => x.Policy == "AdminOnly");
            typeof(CatalogueSolutionsController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Admin");
            typeof(CatalogueSolutionsController).Should()
                .BeDecoratedWith<RouteAttribute>(x => x.Template == "admin/catalogue-solutions");
        }

        [Fact]
        public static void Constructor_NullService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                    () =>
                        _ = new CatalogueSolutionsController(null, null))
                .ParamName.Should()
                .Be("solutionsService");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_SolutionsInDatabase_ReturnsViewWithExpectedModel(
            List<CatalogueItem> solutions)
        {
            var expected = solutions.Select(s => new CatalogueModel(s)).ToList();
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetAllSolutions(null))
                .ReturnsAsync(solutions);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.Index()).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            var model = actual.Model.As<CatalogueSolutionsModel>();
            model.Solutions.Should().BeEquivalentTo(expected);
            model.HasSelected.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Index_StatusInput_SetsSelectedOnModel_ReturnsViewWithModel(
            List<CatalogueItem> solutions)
        {
            var status = Enums.GetValues<PublicationStatus>()[new Random().Next(0, Enums.GetMemberCount<PublicationStatus>())];
            var expected = solutions.Select(s => new CatalogueModel(s)).ToList();
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetAllSolutions(status))
                .ReturnsAsync(solutions);
            var controller = new CatalogueSolutionsController(mockService.Object, Mock.Of<IUsersService>());

            var actual = (await controller.Index(status)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            var model = actual.Model.As<CatalogueSolutionsModel>();
            model.Solutions.Should().BeEquivalentTo(expected);
            model.HasSelected.Should().BeTrue();
            model.AllPublicationStatuses.Single(p => p.Checked).Id.Should().Be((int)status);
        }

        [Fact]
        public static async Task Get_ManageSolution_ReturnsViewWithExpectedModel()
        {
            var expected = new CatalogueItem
            {
                CatalogueItemId = new EntityFramework.Ordering.Models.CatalogueItemId(999999, "999"),
                Solution = new Solution
                {
                    LastUpdated = DateTime.UtcNow,
                    LastUpdatedBy = new Guid("00000000-BBBB-CCCC-DDDD-AAAAAAAAAAAA"),
                },
                Supplier = new Supplier
                {
                    Name = "Fake Supplier",
                },
                Name = "Fake Solution",
            };

            var mockSolutionService = new Mock<ISolutionsService>();
            mockSolutionService.Setup(s => s.GetSolution(It.IsAny<EntityFramework.Ordering.Models.CatalogueItemId>()))
                .ReturnsAsync(expected);

            var mockUsersService = new Mock<IUsersService>();
            mockUsersService.Setup(u => u.GetUser(It.IsAny<string>()))
                .ReturnsAsync(new AspNetUser { FirstName = "Bob", LastName = "Smith" });
            var controller = new CatalogueSolutionsController(mockSolutionService.Object, mockUsersService.Object);

            var actual = (await controller.ManageCatalogueSolution(expected.CatalogueItemId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            var model = actual.Model.As<ManageCatalogueSolutionModel>();
            model.Solution.Should().BeEquivalentTo(expected);
            model.LastUpdatedByName.Should().BeEquivalentTo("Bob Smith");
        }
    }
}
