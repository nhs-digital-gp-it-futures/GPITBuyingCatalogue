using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.DevelopmentPlans
{
    public sealed class SaveWorkOffPlanModel
    {
        public int Id { get; init; }

        public CatalogueItemId SolutionId { get; init; }

        public string StandardId { get; init; }

        public string Details { get; init; }

        public DateTime CompletionDate { get; init; }
    }
}
