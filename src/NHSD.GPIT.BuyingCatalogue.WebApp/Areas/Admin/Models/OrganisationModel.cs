using System;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class OrganisationModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string OdsCode { get; set; }
    }
}
