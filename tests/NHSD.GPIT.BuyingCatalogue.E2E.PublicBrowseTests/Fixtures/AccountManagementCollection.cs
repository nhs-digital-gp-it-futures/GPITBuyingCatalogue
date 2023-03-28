using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests;

[CollectionDefinition(nameof(AccountManagementCollection))]
public sealed class AccountManagementCollection : ICollectionFixture<LocalWebApplicationFactory>
{
}
