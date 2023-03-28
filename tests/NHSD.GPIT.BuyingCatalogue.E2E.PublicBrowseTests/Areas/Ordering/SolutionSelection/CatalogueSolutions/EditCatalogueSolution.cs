using System.Collections.Generic;
using System.Linq;
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
    public class EditCatalogueSolution : BuyerTestBase
    {
        private const string InternalOrgId = "IB-QWO";
        private const int OrderId = 90012;
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly CatalogueItemId ExistingSolutionId = new(99998, "001");
        private static readonly CatalogueItemId NewSolutionId = new(99998, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        public EditCatalogueSolution(LocalWebApplicationFactory factory)
            : base(factory, typeof(CatalogueSolutionsController), nameof(CatalogueSolutionsController.EditSolution), Parameters)
        {
        }

        private OrderItem ExistingSolution => GetEndToEndDbContext().OrderItems
            .FirstOrDefault(x => x.OrderId == OrderId && x.CatalogueItemId == ExistingSolutionId);

        private OrderItem NewSolution => GetEndToEndDbContext().OrderItems
            .FirstOrDefault(x => x.OrderId == OrderId && x.CatalogueItemId == NewSolutionId);

        [Fact]
        public void EditCatalogueSolution_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("Catalogue Solutions - E2E Test Supplier With Contact".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GetNumberOfSelectedRadioButtons().Should().Be(1);
        }

        [Fact]
        public void EditCatalogueSolution_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();

            ExistingSolution.Should().NotBeNull();
            NewSolution.Should().BeNull();
        }

        [Fact]
        public void EditCatalogueSolution_ClickSave_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();

            ExistingSolution.Should().NotBeNull();
            NewSolution.Should().BeNull();
        }

        [Fact]
        public void EditCatalogueSolution_ChangeSolution_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("E2E With Contact With Single Price");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ConfirmSolutionChanges)).Should().BeTrue();

            ExistingSolution.Should().NotBeNull();
            NewSolution.Should().BeNull();
        }
    }
}
