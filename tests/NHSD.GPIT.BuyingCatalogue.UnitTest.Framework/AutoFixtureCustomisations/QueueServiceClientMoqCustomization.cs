using AutoFixture;
using Azure.Storage.Queues;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    [ExcludesAutoCustomization]
    public class QueueServiceClientMoqCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<QueueServiceClient>(_ => new MoqRelaySpecimenBuilder<QueueServiceClient>());
        }
    }
}
