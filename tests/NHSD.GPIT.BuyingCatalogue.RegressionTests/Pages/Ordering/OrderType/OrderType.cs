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
        /// <param name="type">Catalogue Order Type, Defaults to Catalogue Solution. </param>
        /// <param name="frameworkType">Choose the desired framework. </param>
        public void ChooseOrderType(FrameworkType frameworkType = FrameworkType.Tech_Innovation, CatalogueItemType type = CatalogueItemType.Solution, AssociatedServiceType associatedServiceType = AssociatedServiceType.AssociatedServiceOther)
        {
            string frameWork = frameworkType.ToString().Replace("_", " ");
            CommonActions.ClickSave();

            switch (type)
            {
                case CatalogueItemType.AssociatedService:
                    if (associatedServiceType == AssociatedServiceType.AssociatedServiceOther || associatedServiceType == AssociatedServiceType.AssociatedServiceSplit || associatedServiceType == AssociatedServiceType.AssociatedServiceMerger)
                    {
                        AssociatedServiceCorrectPage(associatedServiceType, frameWork);
                    }

                    break;

                default:
                    CatalogueSolutionCorrectPage(frameWork);
                    break;
            }
        }

        public void ChooseFrameworkType(string frameWork)
        {
            CommonActions.HintText().Should().Be("Select the procurement framework that the solution you want to order is available on.".FormatForComparison());
            CommonActions.ClickRadioButtonWithText(frameWork);
            CommonActions.ClickSave();
        }

        private void CatalogueSolutionCorrectPage(string frameWork)
        {
            CommonActions.ClickFirstRadio();
            CommonActions.ClickSave();
            ChooseFrameworkType(frameWork);
        }

        private void AssociatedServiceCorrectPage(AssociatedServiceType associatedServiceType, string frameWork)
        {
            CommonActions.ClickLastRadio();
            CommonActions.ClickSave();
            CommonActions.HintText().Should().Be("Select the type of Associated Service you want to order.".FormatForComparison());

            CommonActions.ClickRadioButtonWithValue(associatedServiceType.ToString());
            CommonActions.ClickSave();
            ChooseFrameworkType(frameWork);
        }
    }
}
