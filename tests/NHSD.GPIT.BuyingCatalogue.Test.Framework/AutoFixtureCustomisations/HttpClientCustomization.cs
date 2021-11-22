using System.Net.Http;
using AutoFixture;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class HttpClientCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<HttpClient>(_ => new HttpClientSpecimenBuilder());
        }
    }
}
