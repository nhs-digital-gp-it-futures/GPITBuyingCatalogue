using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests;

[CollectionDefinition(nameof(AdminCollection))]
public sealed class AdminCollection : ICollectionFixture<LocalWebApplicationFactory>
{
}
