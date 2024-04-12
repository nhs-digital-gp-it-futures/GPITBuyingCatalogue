using AutoFixture;
using Azure.Storage.Blobs;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    [ExcludesAutoCustomization]
    public class BlobServiceClientMoqCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<BlobServiceClient>(_ => new MoqRelaySpecimenBuilder<BlobServiceClient>());
        }
    }
}
