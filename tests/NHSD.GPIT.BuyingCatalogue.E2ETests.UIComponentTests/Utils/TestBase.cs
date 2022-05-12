using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.UrlGenerators;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Actions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils
{
    public abstract class TestBase
    {
        private readonly Uri uri;

        public TestBase(LocalWebApplicationFactory factory,
                Type controller,
                string methodName,
                IDictionary<string, string>? parameters = null
            )
        {
            Factory = factory;
            Driver = factory.Driver;
            CommonActions = new CommonActions(Driver);
            YesNoRadioButtonActions = new YesNoRadioButtonAction(Driver);
            RadioListsActions = new RadioListsActions(Driver);
            CheckboxesActions = new CheckboxesActions(Driver);

            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));

            uri = new Uri(factory.RootUri!);

            NavigateToUrl(UrlGenerator.GenerateUrlFromMethod(
                controller,
                methodName,
                parameters));
        }

        public LocalWebApplicationFactory Factory { get; }

        public IWebDriver Driver { get; }

        public WebDriverWait Wait { get; }

        internal CommonActions CommonActions { get; }

        internal YesNoRadioButtonAction YesNoRadioButtonActions { get; }

        internal RadioListsActions RadioListsActions { get; }

        internal CheckboxesActions CheckboxesActions { get; }

        private void NavigateToUrl(string relativeUri)
        {
            var combinedUri = new Uri(uri, relativeUri);
            Driver.Navigate().GoToUrl(combinedUri);
        }
    }
}
