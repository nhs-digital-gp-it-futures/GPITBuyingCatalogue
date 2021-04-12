using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    [ExcludeFromCodeCoverage]
    public sealed class DisabledErrorMessageSettings
    {
        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }
    }
}
