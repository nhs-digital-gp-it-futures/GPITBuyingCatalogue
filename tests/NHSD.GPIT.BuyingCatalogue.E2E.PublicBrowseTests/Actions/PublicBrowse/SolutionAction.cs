﻿using NHSD.GPIT.BuyingCatalogue.E2ETests.Common.Actions;
using OpenQA.Selenium;
using System;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse
{
    internal class SolutionAction : ActionBase
    {
        public SolutionAction(IWebDriver driver) : base(driver)
        {
        }

        internal bool SolutionNameDisplayed()
        {
            return Driver.FindElement(Objects.PublicBrowse.SolutionObjects.SolutionName).Displayed;
        }

        internal void WaitUntilSolutionNameDisplayed()
        {
            Wait.Until(s => SolutionNameDisplayed());
        }
    }
}