using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ImportModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class ImportControllerTests
    {
        private const int UserId = 1;
        private const string Url = "http://www.google.com";

        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(ImportController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Policy == "AdminOnly");
            typeof(ImportController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Admin");
            typeof(ImportController).Should().BeDecoratedWith<RouteAttribute>(r => r.Template == "admin/import");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ImportController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static void Get_ImportGpPracticeList_ReturnsExpectedResult(
            ImportController systemUnderTest)
        {
            var result = systemUnderTest.ImportGpPracticeList().As<ViewResult>();

            result.ViewName.Should().BeNull();
            result.Model.Should().BeAssignableTo<ImportGpPracticeListModel>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ImportGpPracticeList_ModelContainsErrors_ReturnsExpectedResult(
            ImportGpPracticeListModel model,
            ImportController systemUnderTest)
        {
            systemUnderTest.ModelState.AddModelError("key", "errorMessage");

            var result = (await systemUnderTest.ImportGpPracticeList(model)).As<ViewResult>();

            result.ViewName.Should().BeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ImportGpPracticeList_ReturnsExpectedResult(
            string emailAddress,
            ImportGpPracticeListModel model,
            [Frozen] IUsersService mockUsersService,
            ImportController systemUnderTest)
        {
            model.CsvUrl = Url;

            mockUsersService.GetUser(UserId).Returns(new AspNetUser { Email = emailAddress });

            var result = (await systemUnderTest.ImportGpPracticeList(model)).As<RedirectToActionResult>();

            result.ActionName.Should().Be(nameof(ImportController.ImportGpPracticeListConfirmation));
        }

        [Theory]
        [MockAutoData]
        public static void Get_ImportGpPracticeListConfirmation_ReturnsExpectedResult(
            ImportController systemUnderTest)
        {
            var result = systemUnderTest.ImportGpPracticeListConfirmation().As<ViewResult>();

            result.ViewName.Should().BeNull();
            result.Model.Should().BeAssignableTo<NavBaseModel>();
        }
    }
}
