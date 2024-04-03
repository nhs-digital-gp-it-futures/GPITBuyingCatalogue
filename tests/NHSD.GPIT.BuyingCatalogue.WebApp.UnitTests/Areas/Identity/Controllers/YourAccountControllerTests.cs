using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.YourAccount;
using NSubstitute;
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
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(YourAccountController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_ReturnsDefaultViewWithModelSet(
            [Frozen] IOrganisationsService mockOrganisationsService,
            Organisation organisation,
            YourAccountController controller)
        {
            mockOrganisationsService.GetOrganisationByInternalIdentifier(Arg.Any<string>()).ReturnsForAnyArgs(Task.FromResult(organisation));

            var expectedModel = new YourAccountModel(organisation)
            {
                Title = YourAccountController.YourAccountTitle,
            };

            var result = await controller.Index();
            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            var model = actualResult.Model.Should().BeAssignableTo<YourAccountModel>().Subject;
            model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.Caption));
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData(true)]
        [MockInlineAutoData(false)]
        public static async Task Get_ManageEmailNotifications_ReturnsDefaultViewWithModelSet(
            bool? saved,
            [Frozen] IEmailPreferenceService mockEmailPreferenceService,
            UserEmailPreferenceModel userEmailPreferenceModel,
            YourAccountController controller)
        {
            var preferences = new List<UserEmailPreferenceModel> { userEmailPreferenceModel };

            mockEmailPreferenceService.Get(Arg.Any<int>()).ReturnsForAnyArgs(Task.FromResult(preferences));

            var expectedModel = new ManageEmailPreferencesModel()
            {
                Title = YourAccountController.ManageEmailNotificationsTitle,
                EmailPreferences = preferences,
            };

            if (saved.HasValue)
            {
                expectedModel.Saved = saved.Value;
            }

            var result = saved.HasValue
                ? await controller.ManageEmailNotifications(saved.Value)
                : await controller.ManageEmailNotifications();

            var actualResult = result.Should().BeAssignableTo<ViewResult>().Subject;

            var model = actualResult.Model.Should().BeAssignableTo<ManageEmailPreferencesModel>().Subject;
            model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.Caption));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ManageEmailNotifications_Redirects(
            UserEmailPreferenceModel userEmailPreferenceModel,
            YourAccountController controller)
        {
            var preferences = new List<UserEmailPreferenceModel> { userEmailPreferenceModel };

            var model = new ManageEmailPreferencesModel()
            {
                Title = YourAccountController.ManageEmailNotificationsTitle,
                EmailPreferences = preferences,
            };

            var result = await controller.ManageEmailNotifications(model);

            var actualResult = result.Should().BeAssignableTo<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(YourAccountController.ManageEmailNotifications));
            actualResult.RouteValues.Should().BeEquivalentTo(new Dictionary<string, bool>
            {
                { "saved", true },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Invalid(
            UserEmailPreferenceModel userEmailPreferenceModel,
            YourAccountController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var preferences = new List<UserEmailPreferenceModel> { userEmailPreferenceModel };

            var model = new ManageEmailPreferencesModel()
            {
                Title = YourAccountController.ManageEmailNotificationsTitle,
                EmailPreferences = preferences,
            };

            var result = await controller.ManageEmailNotifications(model);
            result.Should().BeAssignableTo<ViewResult>();
        }
    }
}
