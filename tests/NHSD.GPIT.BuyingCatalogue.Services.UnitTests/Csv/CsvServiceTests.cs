using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.Services.Csv;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Csv
{
    public static class CsvServiceTests
    {
        [Theory]
        [CommonAutoData]
        public static Task CreateFullOrderCsvAsync_NullOrder_ThrowsException(
            CsvService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateFullOrderCsvAsync(null, null));
        }

        [Theory]
        [CommonAutoData]
        public static Task CreatePatientNumberCsvAsync_NullOrder_ThrowsException(
            CsvService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.CreatePatientNumberCsvAsync(null, null));
        }
    }
}
