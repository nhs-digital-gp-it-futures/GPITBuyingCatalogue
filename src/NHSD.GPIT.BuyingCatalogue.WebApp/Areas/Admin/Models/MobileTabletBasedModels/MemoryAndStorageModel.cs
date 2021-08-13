using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.MobileTabletBasedModels
{
    public class MemoryAndStorageModel : ApplicationTypeBaseModel
    {
        public MemoryAndStorageModel()
            : base()
        {
            MemorySizes = Framework.Constants.SelectLists.MemorySizes;
        }

        public MemoryAndStorageModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/mobiletablet";
            MemorySizes = Framework.Constants.SelectLists.MemorySizes;

            SelectedMemorySize = ClientApplication?.MobileMemoryAndStorage?.MinimumMemoryRequirement;
            Description = ClientApplication?.MobileMemoryAndStorage?.Description;
        }

        public override bool IsComplete =>
            !string.IsNullOrWhiteSpace(SelectedMemorySize) &&
            !string.IsNullOrWhiteSpace(Description);

        [Required(ErrorMessage = "Select a minimum memory size")]
        public string SelectedMemorySize { get; set; }

        public List<SelectListItem> MemorySizes { get; set; }

        [Required(ErrorMessage = "Enter storage space information")]
        [StringLength(300)]
        public string Description { get; set; }
    }
}
