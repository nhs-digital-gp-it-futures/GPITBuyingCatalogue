using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.Services
{
    internal sealed class StubbedUrlValidator : IUrlValidator
    {
        public bool IsValidUrl(string url)
            => true;
    }
}
