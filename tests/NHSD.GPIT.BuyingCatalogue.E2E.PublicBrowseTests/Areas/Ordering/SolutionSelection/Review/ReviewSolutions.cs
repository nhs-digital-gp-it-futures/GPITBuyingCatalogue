using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Review
{
    public class ReviewSolutions : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90013;

        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        public ReviewSolutions(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ReviewSolutionsController),
                  nameof(ReviewSolutionsController.ReviewSolutions),
                  Parameters)
        {
        }

        [Fact]
        public void ReviewSolutions_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Catalogue Solution and services - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(ReviewSolutionsObjects.CatalogueSolutionSectionTitle).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ReviewSolutionsObjects.AdditionalServicesSectionTitle).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ReviewSolutionsObjects.AssociatedServicesSectionTitle).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ReviewSolutionsObjects.ContinueButton).Should().BeTrue();
        }

        [Fact]
        public void ReviewSolutions_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void ReviewSolutions_ClickContinueButton_ExpectedResult()
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.ContinueButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }
    }
}
