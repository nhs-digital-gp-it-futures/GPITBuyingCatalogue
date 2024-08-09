using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
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
            CommonActions.ClickLinkElement(InteroperabilityObjects.IM1IntegrationsLink);
            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(InteroperabilityController),
                    nameof(InteroperabilityController.AddIm1Integration))
                .Should()
                .BeTrue();

            var integrationTypes = CommonActions.GetAllDropdownOptions(InteroperabilityObjects.SelectedIntegrationType);

            for (var i = 0; i < integrationTypes.Count; i++)
            {
                var type = integrationTypes[i];

                CommonActions.SelectDropDownItemByValue(InteroperabilityObjects.SelectedIntegrationType, type);
                CommonActions.SelectDropDownItemByValue(
                    InteroperabilityObjects.SelectedProviderOrConsumer,
                    (providerOrConsumer == ProviderOrConsumer.Consumer).ToString());

                TextGenerators.TextInputAddText(InteroperabilityObjects.IntegratesWith, 100);

                TextGenerators.TextInputAddText(E2ETests.Framework.Objects.Common.CommonSelectors.Description, 1000);
                CommonActions.ClickSave();

                if ((integrationTypes.Count - 1) == i) continue;

                CommonActions.ClickLinkElement(InteroperabilityObjects.IM1IntegrationsLink);
                CommonActions.PageLoadedCorrectGetIndex(
                        typeof(InteroperabilityController),
                        nameof(InteroperabilityController.AddIm1Integration))
                    .Should()
                    .BeTrue();
            }
        }

        public void AddGPConnect1Integrations(ProviderOrConsumer providerOrConsumer)
        {
            CommonActions.ClickLinkElement(InteroperabilityObjects.GPConnectIntegrationsLink);
            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(InteroperabilityController),
                    nameof(InteroperabilityController.AddGpConnectIntegration))
                .Should()
                .BeTrue();

            var integrationTypes = CommonActions.GetAllDropdownOptions(InteroperabilityObjects.SelectedIntegrationType);

            for (var i = 0; i < integrationTypes.Count; i++)
            {
                var type = integrationTypes[i];

                CommonActions.SelectDropDownItemByValue(InteroperabilityObjects.SelectedIntegrationType, type);
                CommonActions.SelectDropDownItemByValue(
                    InteroperabilityObjects.SelectedProviderOrConsumer,
                    (providerOrConsumer == ProviderOrConsumer.Consumer).ToString());

                TextGenerators.TextInputAddText(
                    E2ETests.Framework.Objects.Common.CommonSelectors.AdditionalInfoTextArea,
                    1000);
                CommonActions.ClickSave();

                if ((integrationTypes.Count - 1) == i) continue;

                CommonActions.ClickLinkElement(InteroperabilityObjects.GPConnectIntegrationsLink);
                CommonActions.PageLoadedCorrectGetIndex(
                        typeof(InteroperabilityController),
                        nameof(InteroperabilityController.AddGpConnectIntegration))
                    .Should()
                    .BeTrue();
            }
        }
    }
}
