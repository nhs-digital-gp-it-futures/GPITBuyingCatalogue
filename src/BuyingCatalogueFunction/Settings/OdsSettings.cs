using System;

namespace BuyingCatalogueFunction.Settings
{
    public class OdsSettings
    {
        public OdsSettings(
            Uri organisationsUri,
            Uri relationshipsUri,
            Uri rolesUri,
            Uri searchUri)
        {
            OrganisationsUri = organisationsUri;
            RelationshipsUri = relationshipsUri;
            RolesUri = rolesUri;
            SearchUri = searchUri;
        }

        public Uri OrganisationsUri { get; }

        public Uri RelationshipsUri { get; }

        public Uri RolesUri { get; }

        public Uri SearchUri { get; }
   }
}
