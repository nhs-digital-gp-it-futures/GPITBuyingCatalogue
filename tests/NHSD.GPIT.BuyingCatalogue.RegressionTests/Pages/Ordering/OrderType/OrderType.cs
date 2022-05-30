using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.OrderType
{
    public sealed class OrderType : PageBase
    {
        public OrderType(IWebDriver driver, CommonActions commonActions) : base(driver, commonActions)
        {
        }

        /// <summary>
        /// Choose the Order Type, Catalogue Solution or Associated Service
        /// </summary>
        /// <param name="type">Catalogue Order Type, Defaults to Catalogue Solution</param>
        public void ChooseOrderType(CatalogueItemType type = CatalogueItemType.Solution)
        {
            CommonActions.ClickRadioButtonWithValue(type.ToString());

            CommonActions.ClickSave();

            switch (type)
            {
                case CatalogueItemType.AssociatedService:
                    AssociatedServiceCorrectPage();
                    break;

                default:
                    CatalogueSolutionCorrectPage();
                    break;
            }
        }

        private void CatalogueSolutionCorrectPage()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.Index)).Should().BeTrue();
        }

        private void AssociatedServiceCorrectPage()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.ReadyToStart)).Should().BeTrue();
        }
    }
}
