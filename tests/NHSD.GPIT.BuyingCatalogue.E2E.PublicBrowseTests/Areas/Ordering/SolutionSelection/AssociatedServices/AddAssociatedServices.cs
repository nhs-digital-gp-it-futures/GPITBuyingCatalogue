using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.AssociatedServices
{
    [Collection(nameof(OrderingCollection))]
    public class AddAssociatedServices : BuyerTestBase
    {
        private const string InternalOrgId = "IB-QWO";
        private static readonly CallOffId CallOffId = new(90004, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        public AddAssociatedServices(LocalWebApplicationFactory factory)
            : base(factory, typeof(AssociatedServicesController), nameof(AssociatedServicesController.AddAssociatedServices), Parameters)
        {
        }

        [Fact]
        public void AddAssociatedServices_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Do you want to add any Associated Services to your order? - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(AssociatedServicesObjects.AdditionalServicesRequired).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AddAssociatedServices_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void AddAssociatedServices_NoInput_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.AddAssociatedServices)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                AssociatedServicesObjects.AdditionalServicesRequiredErrorMessage,
                $"Error:{AddAssociatedServicesModelValidator.AdditionalServicesRequiredMissingErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void AddAssociatedServices_SelectYes_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("Yes");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.SelectAssociatedServices)).Should().BeTrue();
        }

        [Fact]
        public void AddAssociatedServices_SelectNo_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("No");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ReviewSolutionsController),
                nameof(ReviewSolutionsController.ReviewSolutions)).Should().BeTrue();
        }
    }
}
