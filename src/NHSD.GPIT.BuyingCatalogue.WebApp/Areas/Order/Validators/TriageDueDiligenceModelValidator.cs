using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators
{
    public class TriageDueDiligenceModelValidator : AbstractValidator<TriageDueDiligenceModel>
    {
        public const string NoSelectionUnder40kErrorMessage = "Select yes if you’ve identified what you want to order";
        public const string NoSelectionBetween40kTo250KErrorMessage = "Select yes if you’ve carried out a competition on the Buying Catalogue";
        public const string NoSelectionOver250KErrorMessage = "Select yes if you’ve carried out an Off-Catalogue Competition with suppliers";

        public TriageDueDiligenceModelValidator()
        {
            RuleFor(m => m.Selected)
                .NotNull()
                .WithMessage(CorrectErrorMessage);
        }

        private string CorrectErrorMessage(TriageDueDiligenceModel model, bool? selected)
        {
            _ = selected;
            return model.Option switch
            {
                OrderTriageValue.Under40K => NoSelectionUnder40kErrorMessage,
                OrderTriageValue.Between40KTo250K => NoSelectionBetween40kTo250KErrorMessage,
                OrderTriageValue.Over250K => NoSelectionOver250KErrorMessage,
                _ => throw new FormatException(nameof(model.Option)),
            };
        }
    }
}
