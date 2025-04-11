using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared.ServiceRecipients
{
    public class SelectSublocationsModelValidator : AbstractValidator<SelectSublocationsModel>
    {
        public const string NoSublocationSelectedMessage = "Select the sublocations for this order";

        public SelectSublocationsModelValidator()
        {
            RuleFor(x => x.RenderedSublocations)
                .Must(HaveMadeASelection)
                .WithMessage(NoSublocationSelectedMessage)
                .OverridePropertyName("RenderedSublocations[0].Value");
        }

        private static bool HaveMadeASelection(IReadOnlyList<SelectOption<bool>> checkboxSelections)
        {
            if (checkboxSelections is null or { Count: 0 })
                return false;

            var checkedCheckboxSelectionCount =
                checkboxSelections.Count(x => x.Value);

            return checkedCheckboxSelectionCount > 0;
        }
    }
}
