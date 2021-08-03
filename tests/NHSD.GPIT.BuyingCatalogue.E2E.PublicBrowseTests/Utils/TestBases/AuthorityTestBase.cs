namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases
{
    public abstract class AuthorityTestBase : TestBase
    {
        protected AuthorityTestBase(
            LocalWebApplicationFactory factory,
            string urlArea = "")
            : base(factory, urlArea)
        {
            AuthorityLogin();
        }
    }
}
