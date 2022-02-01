﻿using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Actions
{
    internal sealed class RadioListsActions : ActionBase
    {
        public RadioListsActions(IWebDriver driver) : base(driver)
        {
        }

        internal string GetElementChecked(By by) => Driver.FindElement(by).GetDomProperty("checked");
    }
}
