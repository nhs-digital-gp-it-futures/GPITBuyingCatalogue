using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilitiesMappingModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Capabilities;

public class CapabilitiesUploadModelValidator : AbstractValidator<CapabilitiesUploadModel>
{
    public CapabilitiesUploadModelValidator()
    {
        RuleFor(x => x.CapabilitiesCsv)
            .IsValidCsv();
    }
}
