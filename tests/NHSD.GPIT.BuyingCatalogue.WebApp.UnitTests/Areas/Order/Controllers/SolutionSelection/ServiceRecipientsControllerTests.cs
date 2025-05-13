using System.Reflection;
using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection
{
    public static class ServiceRecipientsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(ServiceRecipientsController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(ServiceRecipientsController).Should()
                .BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            ConstructorInfo[] constructors = typeof(ServiceRecipientsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static void UploadOrSelectServiceRecipients_Get_ReturnsViewWithModel(
            string internalOrgId,
            CallOffId callOffId,
            ServiceRecipientsController controller)
        {
            var result = controller.UploadOrSelectServiceRecipients(internalOrgId, callOffId);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<UploadOrSelectServiceRecipientModel>().Subject;

            model.Should().NotBeNull();
            model.Caption.Should().Be($"Order {callOffId}");
            model.BackLink.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static void UploadOrSelectServiceRecipients_Post_InvalidModel_ReturnsViewWithModel(
            UploadOrSelectServiceRecipientModel model,
            string internalOrgId,
            CallOffId callOffId,
            ServiceRecipientsController controller)
        {
            controller.ModelState.AddModelError("SomeError", "Error message");

            var result = controller.UploadOrSelectServiceRecipients(model, internalOrgId, callOffId);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var returnedModel = viewResult.Model.Should().BeOfType<UploadOrSelectServiceRecipientModel>().Subject;

            returnedModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static void UploadOrSelectServiceRecipients_Post_UploadRecipients_RedirectsToImportController(
            UploadOrSelectServiceRecipientModel model,
            string internalOrgId,
            CallOffId callOffId,
            ServiceRecipientsController controller)
        {
            model.ShouldUploadRecipients = true;

            var result = controller.UploadOrSelectServiceRecipients(model, internalOrgId, callOffId);

            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be(nameof(ImportServiceRecipientsController.Index));
            redirectToActionResult.ControllerName.Should()
                .Be(typeof(ImportServiceRecipientsController).ControllerName());
        }

        [Theory]
        [MockAutoData]
        public static void
            UploadOrSelectServiceRecipients_Post_DoNotUploadRecipients_RedirectsToSelectServiceRecipientsAction(
                UploadOrSelectServiceRecipientModel model,
                string internalOrgId,
                CallOffId callOffId,
                ServiceRecipientsController controller)
        {
            model.ShouldUploadRecipients = false;

            var result = controller.UploadOrSelectServiceRecipients(model, internalOrgId, callOffId);

            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should()
                .Be(nameof(ServiceRecipientsController.SelectSublocations));
            redirectToActionResult.ControllerName.Should()
                .Be(typeof(ServiceRecipientsController).ControllerName());
        }
    }
}
