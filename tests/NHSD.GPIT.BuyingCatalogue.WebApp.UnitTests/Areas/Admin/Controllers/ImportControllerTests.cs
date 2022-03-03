using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
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
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ImportController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_ImportGpPracticeList_ReturnsExpectedResult(
            ImportController systemUnderTest)
        {
            var result = systemUnderTest.ImportGpPracticeList().As<ViewResult>();

            result.ViewName.Should().BeNull();
            result.Model.Should().BeAssignableTo<ImportGpPracticeListModel>();
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_ImportGpPracticeList_ReturnsExpectedResult(
            string emailAddress,
            ImportGpPracticeListModel model,
            [Frozen] Mock<IBackgroundJobClient> mockBackgroundJobClient,
            [Frozen] Mock<IUsersService> mockUsersService,
            ImportController systemUnderTest)
        {
            model.CsvUrl = Url;
            Job job = null;

            mockUsersService
                .Setup(x => x.GetUser(UserId))
                .ReturnsAsync(new AspNetUser { Email = emailAddress });

            mockBackgroundJobClient
                .Setup(x => x.Create(It.IsAny<Job>(), It.IsAny<IState>()))
                .Callback<Job, IState>((j, _) => job = j)
                .Returns(string.Empty);

            var result = (await systemUnderTest.ImportGpPracticeList(model)).As<RedirectToActionResult>();

            mockUsersService.VerifyAll();
            mockBackgroundJobClient.VerifyAll();

            job.Type.Should().BeAssignableTo<IGpPracticeService>();
            job.Method.Name.Should().Be(nameof(IGpPracticeService.ImportGpPracticeData));
            job.Args[0].Should().BeEquivalentTo(new Uri(Url));
            job.Args[1].Should().Be(emailAddress);

            result.ActionName.Should().Be(nameof(ImportController.ImportGpPracticeListConfirmation));
        }

        [Theory]
        [CommonAutoData]
        public static void Get_ImportGpPracticeListConfirmation_ReturnsExpectedResult(
            ImportController systemUnderTest)
        {
            var result = systemUnderTest.ImportGpPracticeListConfirmation().As<ViewResult>();

            result.ViewName.Should().BeNull();
            result.Model.Should().BeAssignableTo<NavBaseModel>();
        }
    }
}
