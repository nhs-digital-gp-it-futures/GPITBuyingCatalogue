using System;
using Microsoft.AspNetCore.Http;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    public sealed class PublicBrowseSettings
    {
        public string BaseAddress { get; set; }

        public PathString LoginPath { get; set; }

        public PathString LogoutPath { get; set; }

        public Uri LoginAddress
        {
            get
            {
                return new(BaseAddress + LoginPath);
            }
        }

        public Uri LogoutAddress
        {
            get
            {
                return new(BaseAddress + LogoutPath);
            }
        }
    }
}
