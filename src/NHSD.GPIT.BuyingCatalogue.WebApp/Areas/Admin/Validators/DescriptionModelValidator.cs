﻿using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class DescriptionModelValidator : AbstractValidator<DescriptionModel>
    {
        public DescriptionModelValidator(
            IUrlValidator urlValidator)
        {
            RuleFor(m => m.Summary)
                .NotEmpty()
                .WithMessage("Enter a summary");

            RuleFor(m => m.Link)
                .IsValidUrl(urlValidator)
                .Unless(m => string.IsNullOrWhiteSpace(m.Link));
        }
    }
}
