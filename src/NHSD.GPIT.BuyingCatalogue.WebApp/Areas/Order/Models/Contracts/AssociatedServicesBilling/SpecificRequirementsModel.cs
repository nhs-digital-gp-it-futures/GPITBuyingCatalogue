using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.AssociatedServicesBilling
{
    public sealed class SpecificRequirementsModel : NavBaseModel
    {
        public const string NoOptionText = "No, I’ve agreed some specific requirements with the supplier";

        public SpecificRequirementsModel()
        {
        }

        public SpecificRequirementsModel(CallOffId callOffId, ContractFlags contractFlags)
        {
            CallOffId = callOffId;
            ProceedWithoutSpecificRequirements = !contractFlags.HasSpecificRequirements;
        }

        public CallOffId CallOffId { get; set; }

        public bool? ProceedWithoutSpecificRequirements { get; set; }

        public IList<SelectListItem> Options => new List<SelectListItem>
        {
            new("Yes", $"{true}"),
            new(NoOptionText, $"{false}"),
        };
    }
}
