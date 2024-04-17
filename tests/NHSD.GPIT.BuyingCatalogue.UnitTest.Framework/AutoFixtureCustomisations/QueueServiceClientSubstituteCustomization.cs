using AutoFixture;
using Azure.Storage.Queues;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public class QueueServiceClientSubstituteCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<QueueServiceClient>(_ => new SubstituteRelaySpecimenBuilder<QueueServiceClient>());
        }
    }
}
