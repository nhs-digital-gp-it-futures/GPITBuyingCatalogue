﻿using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared
{
    public class SelectCapabilitiesModelValidator : AbstractValidator<SelectCapabilitiesModel>
    {
        public const string NoSelectionMadeErrorMessage = "Select a Capability";

        public SelectCapabilitiesModelValidator()
        {
            RuleFor(x => x)
                .Must(HaveMadeASelection)
                .WithMessage(NoSelectionMadeErrorMessage);
        }

        private static bool HaveMadeASelection(SelectCapabilitiesModel model) => model.SelectedItems?.Any(x => x.Selected) ?? false;
    }
}
