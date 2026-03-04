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
            ArgumentNullException.ThrowIfNull(catalogueItem);

            Description = ApplicationTypeDetail?.NativeDesktopOperatingSystemsDescription;
        }

        [StringLength(1000)]
        public string Description { get; set; }
    }
}
