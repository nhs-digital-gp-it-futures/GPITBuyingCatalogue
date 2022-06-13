﻿using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection.CatalogueSolutions
{
    public class SelectSolutionModelValidator : AbstractValidator<SelectSolutionModel>
    {
        public const string NoSelectionMadeErrorMessage = "Select a Catalogue Solution";

        public SelectSolutionModelValidator()
        {
            RuleFor(x => x.SelectedCatalogueSolutionId)
                .NotEmpty()
                .WithMessage(NoSelectionMadeErrorMessage);
        }
    }
}
