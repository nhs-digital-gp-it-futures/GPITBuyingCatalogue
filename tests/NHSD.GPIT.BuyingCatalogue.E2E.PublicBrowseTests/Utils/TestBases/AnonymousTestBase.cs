namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases
{
    public abstract class AnonymousTestBase : TestBase
    {
        protected AnonymousTestBase(LocalWebApplicationFactory factory, string urlArea = "")
            : base(factory, urlArea)
        {
        }
    }
}
