using NHSD.GPIT.BuyingCatalogue.E2ETests.Common.Actions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse
{
    internal sealed class CommonActions : ActionBase
    {
        public CommonActions(IWebDriver driver) : base(driver)
        {
        }

        internal string PageTitle()
        {
            return Driver.FindElement(Objects.PublicBrowse.CommonObjects.PageTitle).Text;
        }
    }
}
