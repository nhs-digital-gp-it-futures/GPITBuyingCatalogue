using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Authorization
{
    internal sealed class ActionCollection
    {
        internal CommonActions CommonActions { get; set; }
        internal LoginActions LoginActions { get; set; }
    }
}
