using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DesktopBasedModels
{
    public sealed class AdditionalInformationModel : ApplicationTypeBaseModel
    {
        public AdditionalInformationModel()
        {
        }

        public AdditionalInformationModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/desktop";
            AdditionalInformation = ClientApplication?.NativeDesktopAdditionalInformation;
        }

        public override bool IsComplete => !string.IsNullOrWhiteSpace(AdditionalInformation);

        [StringLength(500)]
        public string AdditionalInformation { get; set; }
    }
}
