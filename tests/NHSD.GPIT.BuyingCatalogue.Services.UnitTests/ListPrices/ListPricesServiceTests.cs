using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.Services.ListPrices;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.ListPrices
{
    public static class ListPricesServiceTests
    {
        [Theory]
        [CommonAutoData]
        public static Task SaveSolutionListPrice_NullModel_ThrowsException(ListPricesService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.SaveListPrice(default, null));
        }

        [Theory]
        [CommonAutoData]
        public static Task UpdateSolutionListPrice_NullModel_ThrowsException(ListPricesService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateListPrice(default, null));
        }
    }
}
