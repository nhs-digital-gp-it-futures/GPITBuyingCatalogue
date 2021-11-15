using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.Services
{
    internal sealed class StubbedUrlValidator : IUrlValidator
    {
        public Task<bool> IsValidUrl(string url)
            => Task.FromResult(true);
    }
}
