using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class HomeControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(HomeController).Should().BeDecoratedWith<AuthorizeAttribute>(x => x.Policy == "AdminOnly");
            typeof(HomeController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Admin");
            typeof(HomeController).Should().BeDecoratedWith<RouteAttribute>(x => x.Template == "admin");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(HomeController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Fact]
        public static void Get_BuyerOrganisations_RouteAttribute_ExpectedTemplate()
        {
            typeof(HomeController)
                .GetMethod(nameof(HomeController.BuyerOrganisations))
                .GetCustomAttribute<HttpGetAttribute>()
                .Template.Should()
                .Be("buyer-organisations");
        }

        [Fact]
        public static async Task Get_BuyerOrganisations_GetsAllOrganisations()
        {
            var mockOrganisationService = new Mock<IOrganisationsService>();
            var mockOrganisations = new Mock<IList<Organisation>>().Object;
            mockOrganisationService.Setup(o => o.GetAllOrganisations())
                .ReturnsAsync(mockOrganisations);

            var controller = new HomeController(
                mockOrganisationService.Object);

            await controller.BuyerOrganisations();

            mockOrganisationService.Verify(o => o.GetAllOrganisations());
        }

        [Fact]
        public static async Task Get_BuyerOrganisations_ReturnsViewWithExpectedViewModel()
        {
            var mockOrganisationService = new Mock<IOrganisationsService>();
            var mockOrganisations = new Mock<IList<Organisation>>().Object;
            mockOrganisationService.Setup(o => o.GetAllOrganisations())
                .ReturnsAsync(mockOrganisations);

            var expectedOrganisationModels = mockOrganisations.Select(
                o => new OrganisationModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    OdsCode = o.OdsCode,
                }).ToList();

            var controller = new HomeController(
                mockOrganisationService.Object);

            var actual = (await controller.BuyerOrganisations()).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.As<ListOrganisationsModel>().Organisations.Should().BeEquivalentTo(expectedOrganisationModels);
        }

        [Fact]
        public static void Get_Index_ReturnsDefaultView()
        {
            var controller = new HomeController(
                Mock.Of<IOrganisationsService>());

            var result = controller.Index().As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
        }
    }
}
