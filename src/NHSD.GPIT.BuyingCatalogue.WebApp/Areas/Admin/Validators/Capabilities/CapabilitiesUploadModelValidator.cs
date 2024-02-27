using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilitiesMappingModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Capabilities;

public class CapabilitiesUploadModelValidator : AbstractValidator<Gen2UploadModel>
{
    public CapabilitiesUploadModelValidator()
    {
        RuleFor(x => x.File)
            .IsValidCsv();
    }
}
