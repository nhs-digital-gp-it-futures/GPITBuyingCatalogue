using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases
{
    public abstract class BuyerTestBase : TestBase
    {
        protected BuyerTestBase(
            LocalWebApplicationFactory factory,
            Type controller,
            string methodName,
            IDictionary<string, string> parameters,
            bool initialiseSession = false,
            IDictionary<string, object> sessionValues = null)
            : base(
                  factory,
                  GenerateUrlFromMethod(controller, methodName, parameters),
                  sessionValues)
        {
            BuyerLogin();

            if (initialiseSession)
                Task.Run(() => InitializeSessionAndSetValuesToSession()).Wait();

            if (sessionValues is not null && sessionValues.Any())
                Driver.Navigate().Refresh();
        }
    }
}
