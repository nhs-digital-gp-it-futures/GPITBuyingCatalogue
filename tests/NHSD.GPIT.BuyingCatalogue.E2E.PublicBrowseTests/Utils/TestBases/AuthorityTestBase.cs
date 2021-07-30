using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases
{
    public abstract class AuthorityTestBase : TestBase
    {
        protected AuthorityTestBase(
            LocalWebApplicationFactory factory,
            string urlArea = "",
            bool initialiseSession = false,
            IDictionary<string, object> sessionValues = null)
            : base(factory, urlArea, sessionValues)
        {
            AuthorityLogin();

            if (initialiseSession)
                InitializeSessionAndSetValuesToSession();

            if (sessionValues is not null && sessionValues.Any())
                Driver.Navigate().Refresh();
        }
    }
}
