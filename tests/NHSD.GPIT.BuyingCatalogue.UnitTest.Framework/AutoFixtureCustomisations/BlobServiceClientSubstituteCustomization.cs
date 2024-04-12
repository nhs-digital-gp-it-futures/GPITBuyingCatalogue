using AutoFixture;
using Azure.Storage.Blobs;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public class BlobServiceClientSubstituteCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<BlobServiceClient>(_ => new SubstituteRelaySpecimenBuilder<BlobServiceClient>());
        }
    }
}
