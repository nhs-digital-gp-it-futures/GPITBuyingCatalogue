using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils
{
    internal sealed class StubbedUrlValidator : IUrlValidator
    {
        public bool IsValidUrl(string url)
            => true;
    }
}
