using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class OrderTriageControllerTests
    {
        [Theory]
        [CommonAutoData]
        public static void Get_Index_ReturnsModel(
            string odsCode,
            OrderTriageController controller)
        {
            var result = controller.Index(odsCode);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Post_Index_InvalidModelState_ReturnsView(
            string odsCode,
            OrderTriageModel model,
            OrderTriageController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = controller.Index(odsCode, model);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_Index_NotSureTriageOption_RedirectsToView(
            string odsCode,
            OrderTriageModel model,
            OrderTriageController controller)
        {
            model.SelectedTriageOption = TriageOption.NotSure;

            var result = controller.Index(odsCode, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.NotReady));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_Index_Valid_RedirectsToTriageSelection(
            string odsCode,
            OrderTriageModel model,
            OrderTriageController controller)
        {
            model.SelectedTriageOption = TriageOption.Under40k;

            var result = controller.Index(odsCode, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.TriageSelection));
        }

        [Theory]
        [CommonAutoData]
        public static void Get_NotReady_ReturnsView(
            string odsCode,
            OrderTriageController controller)
        {
            var result = controller.NotReady(odsCode);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeOfType(typeof(GenericOrderTriageModel));
        }

        [Theory]
        [CommonAutoData]
        public static void Get_TriageSelection_ReturnsView(
            string odsCode,
            TriageOption option,
            OrderTriageController controller)
        {
            var result = controller.TriageSelection(odsCode, option);

            result.As<ViewResult>().Should().NotBeNull();
        }

        [Theory]
        [CommonInlineAutoData(TriageOption.Under40k, "Select yes if you’ve identified what you want to order")]
        [CommonInlineAutoData(TriageOption.Between40kTo250k, "Select yes if you’ve carried out a competition on the Buying Catalogue")]
        [CommonInlineAutoData(TriageOption.Over250k, "Select yes if you’ve carried out an Off-Catalogue Competition with suppliers")]
        public static void Post_TriageSelection_NoSelection_AddsModelError(
            TriageOption option,
            string expectedErrorMessage,
            string odsCode,
            TriageDueDiligenceModel model,
            OrderTriageController controller)
        {
            model.Selected = null;

            _ = controller.TriageSelection(odsCode, option, model);

            var modelStateErrors = controller.ModelState.Values.SelectMany(mse => mse.Errors.Select(e => e.ErrorMessage));

            modelStateErrors.Should().Contain(expectedErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_TriageSelection_InvalidModelState_ReturnsView(
            string odsCode,
            TriageOption option,
            TriageDueDiligenceModel model,
            OrderTriageController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = controller.TriageSelection(odsCode, option, model);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_TriageSelection_NotSelected_RedirectsToStepsNotCompleted(
            string odsCode,
            TriageOption option,
            TriageDueDiligenceModel model,
            OrderTriageController controller)
        {
            model.Selected = false;

            var result = controller.TriageSelection(odsCode, option, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.StepsNotCompleted));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_TriageSelection_Selected_Redirects(
            string odsCode,
            TriageOption option,
            TriageDueDiligenceModel model,
            OrderTriageController controller)
        {
            model.Selected = true;

            var result = controller.TriageSelection(odsCode, option, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderController.NewOrder));
            result.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(OrderController).ControllerName());
        }

        [Theory]
        [CommonInlineAutoData(TriageOption.Under40k, "Incomplete40k")]
        [CommonInlineAutoData(TriageOption.Between40kTo250k, "Incomplete40kTo250k")]
        [CommonInlineAutoData(TriageOption.Over250k, "IncompleteOver250k")]
        public static void Get_StepsNotCompleted_ReturnsView(
            TriageOption option,
            string expectedViewName,
            string odsCode,
            OrderTriageController controller)
        {
            var result = controller.StepsNotCompleted(odsCode, option);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be(expectedViewName);
        }
    }
}
