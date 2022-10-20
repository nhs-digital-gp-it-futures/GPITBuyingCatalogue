using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.FundingSource;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo
{
    internal class SelectSolutionAndServices : PageBase
    {
        public SelectSolutionAndServices(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public static List<string> SelectSolutionServices(string solutionName, bool isAssociatedServiceOnly, IEnumerable<string>? associatedServices, IEnumerable<string>? additionalServices)
        {
            var names = new List<string>();

            if (isAssociatedServiceOnly)
            {
                if (associatedServices != default && associatedServices.All(a => a != string.Empty))
                {
                    foreach (var associatedService in associatedServices)
                    {
                        names.Add(associatedService);
                    }
                }
            }
            else
            {
                names.Add(solutionName);
                if (additionalServices != default && additionalServices.All(a => a != string.Empty))
                {
                    foreach (var additionalService in additionalServices)
                    {
                        names.Add(additionalService);
                    }
                }

                if (associatedServices != default && associatedServices.All(a => a != string.Empty))
                {
                    foreach (var associatedService in associatedServices)
                    {
                        names.Add(associatedService);
                    }
                }
            }

            return names;
        }
    }
}
