using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed class Framework : IAudited
    {
        //public Framework(string id, string name)
        //{
        //    Id = id;
        //    Name = name;
        //}

        public string Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string Description { get; set; }

        public string Owner { get; set; }

        public bool LocalFundingOnly { get; set; }

        public DateTime? ActiveDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }
    }
}
