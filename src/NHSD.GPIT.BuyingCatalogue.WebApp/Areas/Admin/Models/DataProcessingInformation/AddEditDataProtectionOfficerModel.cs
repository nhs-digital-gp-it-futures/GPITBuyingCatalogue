using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;

public class AddEditDataProtectionOfficerModel : NavBaseModel
{
    public AddEditDataProtectionOfficerModel()
    {
    }

    public AddEditDataProtectionOfficerModel(
        Solution solution)
    {
        SolutionName = solution.CatalogueItem.Name;

        Name = solution.DataProcessingInformation?.Officer?.Name;
        PhoneNumber = solution.DataProcessingInformation?.Officer?.PhoneNumber;
        EmailAddress = solution.DataProcessingInformation?.Officer?.EmailAddress;
    }

    public string SolutionName { get; set; }

    [StringLength(200)]
    public string Name { get; set; }

    [StringLength(256)]
    public string EmailAddress { get; set; }

    [StringLength(35)]
    public string PhoneNumber { get; set; }
}
