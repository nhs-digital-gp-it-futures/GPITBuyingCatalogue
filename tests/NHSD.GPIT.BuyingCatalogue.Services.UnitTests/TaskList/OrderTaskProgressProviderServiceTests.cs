using System;
using FluentAssertions;
using LinqKit;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList
{
    public static class OrderTaskProgressProviderServiceTests
    {
        [Theory]
        [MockAutoData]
        public static void Constructors_VerifyGuardClauses(OrderTaskProgressProviderService service)
        {
            var values = Enum.GetValues<OrderTaskListStatus>();
            values.ForEach(v => service.ProviderFor(v).Should().NotBeNull());
        }
    }
}
