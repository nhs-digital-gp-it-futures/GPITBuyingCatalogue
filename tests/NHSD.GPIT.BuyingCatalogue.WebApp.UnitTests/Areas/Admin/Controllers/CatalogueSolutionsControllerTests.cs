using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

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
                        _ = new CatalogueSolutionsController(null))
                .ParamName.Should()
                .Be("solutionsService");
        }
        
        [Fact]
        public static void Get_Index_HttpGetAttribute_Present()
        {
            typeof(CatalogueSolutionsController)
                .GetMethod(nameof(CatalogueSolutionsController.Index))
                .GetCustomAttribute<HttpGetAttribute>()
                .Should()
                .NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_SolutionsInDatabase_ReturnsExpectedResult(
            [Frozen] Mock<ISolutionsService> service,
            List<CatalogueItem> solutions,
            CatalogueSolutionsController controller)
        {
            var expected = solutions.Select(s => new CatalogueModel(s)).ToList();
            service.Setup(s => s.GetAllSolutions())
                .ReturnsAsync(solutions);

            var actual = (await controller.Index()).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.As<CatalogueSolutionsModel>().Solutions.Should().BeEquivalentTo(expected);
        }
    }
}
