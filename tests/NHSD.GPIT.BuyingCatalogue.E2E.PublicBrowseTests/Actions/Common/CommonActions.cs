﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common
{
    internal sealed class CommonActions : ActionBase
    {
        public CommonActions(IWebDriver driver)
            : base(driver)
        {
        }

        // Click Actions
        public void ClickGoBackLink()
        {
            Driver.FindElement(CommonSelectors.GoBackLink).Click();
        }

        public bool GoBackLinkDisplayed()
        {
            try
            {
                Wait.Until(d => d.FindElement(CommonSelectors.GoBackLink));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ClickSave()
        {
            Driver.FindElement(CommonSelectors.SaveAndReturn).Click();
        }

        public bool SaveButtonDisplayed()
        {
            try
            {
                Wait.Until(d => d.FindElement(CommonSelectors.SaveAndReturn));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ClickFirstCheckbox() => Driver.FindElements(By.CssSelector("input[type=checkbox]")).First().Click();

        public string ClickCheckbox(By targetField, int index = 0)
        {
            var checkboxItems = Driver.FindElements(targetField);

            var selected = checkboxItems[index];

            selected.FindElement(By.TagName("input")).Click();
            return selected.FindElement(By.TagName("label")).Text;
        }

        public void ClickSection(By targetField, string section)
        {
            Driver.FindElements(targetField)
                .Single(s => s.Text.Contains(section)).FindElement(By.TagName("a"))
                .Click();
        }

        public void SelectDropdownItem(By targetField, int index = 0)
        {
            var select = Driver.FindElement(targetField);
            new SelectElement(select).SelectByIndex(index);
        }

        public void ClickRadioButtonWithText(string label)
        {
            var radioButtonItems = Driver.FindElements(CommonSelectors.RadioButtonItems);

            radioButtonItems.Single(r => r.FindElement(By.TagName("label")).Text == label).FindElement(By.TagName("input")).Click();
        }

        // testing
        public bool ErrorSummaryDisplayed()
        {
            try
            {
                Driver.FindElement(CommonSelectors.NhsErrorSection);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool PageLoadedCorrectGetIndex(
            Type controller,
            string methodName)
        {
            if (controller.BaseType != typeof(Controller))
                throw new InvalidOperationException($"{nameof(controller)} is not a type of {nameof(Controller)}");

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName), $"{nameof(methodName)} should not be null");

            Wait.Until(d => d.FindElement(CommonSelectors.Header1));

            var controllerRoute =
                (controller.GetCustomAttributes(typeof(RouteAttribute), false)
                            ?.FirstOrDefault() as RouteAttribute)
                                ?.Template;

            var methodRoute =
                (controller.GetMethods()
                .Where(m =>
                    m.Name == methodName
                    && m.GetCustomAttributes(typeof(HttpGetAttribute), false)
                        .Any())
                        ?.FirstOrDefault()
                        .GetCustomAttributes(typeof(HttpGetAttribute), false)
                            .FirstOrDefault() as HttpGetAttribute)?.Template;

            var absoluteRoute = methodRoute switch
            {
                null => controllerRoute,
                _ => methodRoute[0] != '~'
                    ? controllerRoute + "/" + methodRoute
                    : methodRoute[2..],
            };

            var driverUrl = new Uri(Driver.Url);

            var actionUrl = new Uri("https://www.fake.com/" + absoluteRoute, UriKind.Absolute);

            for (int i = 0; i < actionUrl.Segments.Length; i++)
            {
                if (!actionUrl.Segments[i].StartsWith("%7B"))
                {
                    if (driverUrl.Segments[i] != actionUrl.Segments[i])
                        return false;
                }
            }

            return true;
        }

        internal string PageTitle()
        {
            return Driver.FindElement(CommonSelectors.Header1).Text;
        }
    }
}
