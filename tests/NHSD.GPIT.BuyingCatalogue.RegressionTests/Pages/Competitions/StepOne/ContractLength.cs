using System.Runtime.CompilerServices;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOne
{
    public class ContractLength : PageBase
    {
        public ContractLength(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void CompetitionContractLength()
        {
            TextGenerators.NumberInputAddRandomNumber(CompetitionsDashboardObjects.ContractLengthInput, 6, 36);
            CommonActions.ClickSave();

            CommonActions.LedeText().Should().Be("Complete the following steps to carry out a competition.".FormatForComparison());
        }
    }
}
