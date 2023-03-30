using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests;

[CollectionDefinition(nameof(SharedContextCollection))]
public sealed class SharedContextCollection : ICollectionFixture<LocalWebApplicationFactory>
{

}
