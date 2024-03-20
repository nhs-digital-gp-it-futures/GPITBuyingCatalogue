using Microsoft.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.Gen2;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.CompetitionToOrder;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.Dashboard;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOne;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOne.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition.SelectFilterType;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo.NonPrice;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.View_Result;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Dashboard;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepOne;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin
{
    public class AdminPages
    {
        public AdminPages(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
        {
            AdminDashboard = new AdminDashboard(driver, commonActions);
            CapabilitiesAndEpicsMappings = new CapabilitiesAndEpicsMappings(driver, commonActions);
        }

        internal AdminDashboard AdminDashboard { get; }

        internal CapabilitiesAndEpicsMappings CapabilitiesAndEpicsMappings { get; }
    }
}
