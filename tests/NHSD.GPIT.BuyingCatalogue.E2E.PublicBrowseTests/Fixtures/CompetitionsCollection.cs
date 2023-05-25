using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests;

[CollectionDefinition(nameof(CompetitionsCollection))]
public sealed class CompetitionsCollection : ICollectionFixture<LocalWebApplicationFactory>
{
}
