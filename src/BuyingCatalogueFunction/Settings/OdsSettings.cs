using System;

namespace BuyingCatalogueFunction.Settings
{
    public class OdsSettings
    {
        public OdsSettings(Uri organisationsUri, Uri searchUri)
        {
            OrganisationsUri = organisationsUri;
            SearchUri = searchUri;
        }

        public Uri OrganisationsUri { get; }

        public Uri SearchUri { get; }
    }
}
