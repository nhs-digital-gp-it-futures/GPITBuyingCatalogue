using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions
{
    public class Interoperability : PageBase
    {
        public Interoperability(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddInteroperability(string solutionId)
        {
            CommonActions.ClickLinkElement(AddSolutionObjects.SolutionInteroperabilityLink(solutionId));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(InteroperabilityController),
                nameof(InteroperabilityController.Interoperability))
                .Should().BeTrue();
        }

        public void AddNHSAppIntegrations()
        {
            CommonActions.ClickLinkElement(InteroperabilityObjects.NHSAppIntegrationsLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(InteroperabilityController),
                nameof(InteroperabilityController.AddNhsAppIntegration))
                .Should().BeTrue();

            CommonActions.ClickAllCheckboxes();
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(InteroperabilityController),
                nameof(InteroperabilityController.Interoperability))
                .Should().BeTrue();

            CommonActions.ClickSave();
        }

        public void AddIM1Integrations(ProviderOrConsumer providerOrConsumer)
        {
            var interopIntegrationTypes = GetIm1Integrations();

            foreach (var type in interopIntegrationTypes)
            {
                CommonActions.ClickLinkElement(InteroperabilityObjects.IM1IntegrationsLink);
                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(InteroperabilityController),
                    nameof(InteroperabilityController.AddIm1Integration))
                    .Should().BeTrue();

                CommonActions.SelectDropDownItemByValue(InteroperabilityObjects.SelectedIntegrationType, type);
                CommonActions.SelectDropDownItemByValue(InteroperabilityObjects.SelectedProviderOrConsumer, providerOrConsumer.ToString());

                TextGenerators.TextInputAddText(InteroperabilityObjects.IntegratesWith, 100);

                TextGenerators.TextInputAddText(E2ETests.Framework.Objects.Common.CommonSelectors.Description, 1000);
                CommonActions.ClickSave();
            }
        }

        public void AddGPConnect1Integrations(ProviderOrConsumer providerOrConsumer)
        {
            var interopGPIntegrationTypes = GetGPConnectIntegrations();

            foreach (var type in interopGPIntegrationTypes)
            {
                CommonActions.ClickLinkElement(InteroperabilityObjects.GPConnectIntegrationsLink);
                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(InteroperabilityController),
                    nameof(InteroperabilityController.AddGpConnectIntegration))
                    .Should().BeTrue();
                CommonActions.SelectDropDownItemByValue(InteroperabilityObjects.SelectedIntegrationType, type);

                CommonActions.SelectDropDownItemByValue(InteroperabilityObjects.SelectedIntegrationType, type);
                CommonActions.SelectDropDownItemByValue(InteroperabilityObjects.SelectedProviderOrConsumer, providerOrConsumer.ToString());

                TextGenerators.TextInputAddText(E2ETests.Framework.Objects.Common.CommonSelectors.AdditionalInfoTextArea, 1000);
                CommonActions.ClickSave();
            }
        }

        public List<string> GetIm1Integrations()
        {
            var im1IntegrationTypes = Enum.GetValues(typeof(InteroperabilityIm1IntegrationType))
            .Cast<InteroperabilityIm1IntegrationType>()
            .Select(v => v.ToString())
            .ToList();

            List<string> im1Integrations = new List<string>(im1IntegrationTypes.Select(v => v.ToString().Replace("_", " ")));

            return im1Integrations;
        }

        public List<string> GetGPConnectIntegrations()
        {
            var gpConnectIntegrationTypes = Enum.GetValues(typeof(InteropGpConnectIntegrationType))
            .Cast<InteropGpConnectIntegrationType>()
            .Select(v => v.ToString())
            .ToList();

            List<string> gpConnectIntegrations = new List<string>(gpConnectIntegrationTypes.Select(v => v.ToString().Replace("_", " ")));

            return gpConnectIntegrations;
        }
    }
}
