using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.YourAccount;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Controllers
{
    public static class YourAccountControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(YourAccountController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Identity");
            typeof(YourAccountController).Should().BeDecoratedWith<RouteAttribute>(r => r.Template == "Identity/YourAccount");
            typeof(YourAccountController).Should().BeDecoratedWith<AuthorizeAttribute>();

        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(YourAccountController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ReturnsDefaultViewWithModelSet(
            [Frozen] Mock<IOrganisationsService> mockOrganisationsService,
            Organisation organisation,
            YourAccountController controller)
        {
            mockOrganisationsService.Setup(x => x.GetOrganisationByInternalIdentifier(It.IsAny<string>()))
                .ReturnsAsync(organisation);

            var expectedModel = new YourAccountModel(organisation);
            var result = await controller.Index();
            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            actualResult.ViewName.Should().BeNull();

            var model = actualResult.Model.Should().BeAssignableTo<YourAccountModel>().Subject;
            model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.Title).Excluding(m => m.Caption));
            model.Title.Should().Be("Your account");
        }
    }
}
