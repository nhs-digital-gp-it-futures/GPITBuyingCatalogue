using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.OrderType
{
    public sealed class OrderType : PageBase
    {
        public OrderType(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        /// <summary>
        /// Choose the Order Type, Catalogue Solution or Associated Service.
        /// </summary>
        /// <param name="type">Catalogue Order Type, Defaults to Catalogue Solution.</param>
        public void ChooseOrderType(CatalogueItemType type = CatalogueItemType.Solution, AssociatedServiceType associatedServiceType = AssociatedServiceType.AssociatedServiceOther)
        {
            CommonActions.ClickRadioButtonWithValue(type.ToString());

            CommonActions.ClickSave();

            switch (type)
            {
                case CatalogueItemType.AssociatedService:
                    if (associatedServiceType == AssociatedServiceType.AssociatedServiceOther || associatedServiceType == AssociatedServiceType.AssociatedServiceSplit || associatedServiceType == AssociatedServiceType.AssociatedServiceMerger)
                    {
                        AssociatedServiceCorrectPage(associatedServiceType);
                    }

                    break;

                default:
                    CatalogueSolutionCorrectPage();
                    break;
            }
        }

        private void CatalogueSolutionCorrectPage()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.ReadyToStart)).Should().BeTrue();
        }

        private void AssociatedServiceCorrectPage(AssociatedServiceType associatedServiceType)
        {
            CommonActions.ClickRadioButtonWithValue(associatedServiceType.ToString());
            CommonActions.LedeText().Should().Be("Select the type of Associated Service you want to order.".FormatForComparison());
            CommonActions.ClickSave();
        }
    }
}
