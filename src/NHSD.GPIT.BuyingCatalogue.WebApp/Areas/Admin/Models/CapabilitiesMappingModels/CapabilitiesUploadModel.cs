using Microsoft.AspNetCore.Http;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilitiesMappingModels;

public class CapabilitiesUploadModel
{
    public IFormFile CapabilitiesCsv { get; set; }
}
