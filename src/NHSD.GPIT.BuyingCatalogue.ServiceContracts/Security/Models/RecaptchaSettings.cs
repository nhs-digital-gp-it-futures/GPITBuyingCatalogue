using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Security.Models;

[ExcludeFromCodeCoverage]
public class RecaptchaSettings
{
    private const string GoogleApiUrl = "https://www.google.com/recaptcha/api/";

    public static Uri GoogleRecaptchaApiUri => new(GoogleApiUrl);

    public bool IsEnabled { get; set; }

    public string SiteKey { get; set; }

    public string SecretKey { get; set; }
}
