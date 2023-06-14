using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.DesktopBasedModels
{
    public sealed class OperatingSystemsModel : ApplicationTypeBaseModel
    {
        public OperatingSystemsModel()
        {
        }

        public OperatingSystemsModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            Description = ClientApplication?.NativeDesktopOperatingSystemsDescription;
        }

        [StringLength(1000)]
        public string Description { get; set; }
    }
}
