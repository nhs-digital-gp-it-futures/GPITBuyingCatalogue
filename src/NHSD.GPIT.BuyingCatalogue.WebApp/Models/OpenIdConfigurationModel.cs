using System;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models;

public class OpenIdConfigurationModel
{
    public Uri Authority { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }
}
