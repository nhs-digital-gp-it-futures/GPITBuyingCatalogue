using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.CatalogueSolutions
{
    public sealed class CatalogueSolutionsDeleteSolution
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private const string InternalOrgId = "CG-03F";
        private const string CatalogueItemName = "E2E With Contact With Single Price";
        private static readonly CallOffId CallOffId = new(90006, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "002");

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), CallOffId.ToString() },
                { nameof(CatalogueItemId), CatalogueItemId.ToString() },
                { nameof(CatalogueItemName), CatalogueItemName },
            };

        public CatalogueSolutionsDeleteSolution(
            LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.SelectSolution),
                  Parameters)
        {
        }

        [Fact]
        public void CatalogueSolutionsDeleteSolution_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsDeleteSolutionCancelLink).Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsDeleteSolution_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.EditSolution))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsDeleteSolution_CancelDelete_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsDeleteSolutionCancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.EditSolution))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsDeleteSolution_DeleteSolution_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.Index))
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(By.LinkText(CatalogueItemName)).Should().BeFalse();

            Driver
                .FindElements(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsAnySolutionRow)
                .Any()
                .Should()
                .BeFalse();
        }

        public Task InitializeAsync()
        {
            InitializeSessionHandler();

            InitializeServiceRecipientMemoryCacheHandler(InternalOrgId);

            NavigateToUrl(
                typeof(DeleteCatalogueSolutionController),
                nameof(DeleteCatalogueSolutionController.DeleteSolution),
                Parameters);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }
    }
}
