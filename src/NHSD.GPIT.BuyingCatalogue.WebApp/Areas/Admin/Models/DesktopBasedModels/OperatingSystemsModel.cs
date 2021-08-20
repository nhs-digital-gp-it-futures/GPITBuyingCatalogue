using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DesktopBasedModels
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

            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/desktop";

            Description = ClientApplication?.NativeDesktopOperatingSystemsDescription;
        }

        public override bool IsComplete => !string.IsNullOrWhiteSpace(Description);

        [Required(ErrorMessage = "Enter supported operating systems information")]
        [StringLength(1000)]
        public string Description { get; set; }
    }
}
