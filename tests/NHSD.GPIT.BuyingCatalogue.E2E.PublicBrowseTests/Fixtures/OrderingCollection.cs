using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests;

[CollectionDefinition(nameof(OrderingCollection))]
public sealed class OrderingCollection : ICollectionFixture<LocalWebApplicationFactory>
{

}
