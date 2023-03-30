using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.CatalogueSolutions
{
    [Collection(nameof(OrderingCollection))]
    public class EditCatalogueSolutionAssociatedServicesOnly : BuyerTestBase
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90014;
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        public EditCatalogueSolutionAssociatedServicesOnly(LocalWebApplicationFactory factory)
            : base(factory, typeof(CatalogueSolutionsController), nameof(CatalogueSolutionsController.EditSolutionAssociatedServicesOnly), Parameters)
        {
        }

        [Fact]
        public void EditSolutionAssociatedServicesOnly_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Which Catalogue Solution does the service help implement? - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GetNumberOfSelectedRadioButtons().Should().Be(1);
        }

        [Fact]
        public void EditSolutionAssociatedServicesOnly_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();
        }

        [Fact]
        public void EditSolutionAssociatedServicesOnly_ClickSave_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();
        }

        [Fact]
        public void EditSolutionAssociatedServicesOnly_ChangeSolution_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("E2E With Contact With Single Price");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ConfirmSolutionChangesAssociatedServicesOnly)).Should().BeTrue();
        }
    }
}
