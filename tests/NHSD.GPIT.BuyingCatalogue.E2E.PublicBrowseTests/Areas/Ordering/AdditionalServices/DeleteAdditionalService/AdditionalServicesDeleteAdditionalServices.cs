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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.AdditionalServices
{
    public sealed class AdditionalServicesDeleteAdditionalServices
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private const string OdsCode = "03F";
        private const string CatalogueItemName = "E2E No Contact Single Price Additional Service";
        private static readonly CallOffId CallOffId = new(90007, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "002A999");

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(OdsCode), OdsCode },
                { nameof(CallOffId), CallOffId.ToString() },
                { nameof(CatalogueItemId), CatalogueItemId.ToString() },
                { nameof(CatalogueItemName), CatalogueItemName },
            };

        public AdditionalServicesDeleteAdditionalServices(
            LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.SelectAdditionalService),
                  Parameters)
        {
        }

        [Fact]
        public void AdditionalServicesDeleteAdditionalServices_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsDeleteSolutionCancelLink).Should().BeTrue();
        }

        [Fact]
        public void AdditionalServicesDeleteAdditionalService_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AdditionalServicesDeleteAdditionalServices_CancelDelete_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsDeleteSolutionCancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AdditionalServicesDeleteAdditionalServices_DeleteSolution_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.Index))
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

            InitializeMemoryCacheHander(OdsCode);

            NavigateToUrl(
                typeof(DeleteAdditionalServiceController),
                nameof(DeleteAdditionalServiceController.DeleteAdditionalService),
                Parameters);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }
    }
}
