namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public class ContractFlags
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public bool? HasSpecificRequirements { get; set; }

        public bool? UseDefaultBilling { get; set; }

        public bool? UseDefaultDataProcessing { get; set; }

        public bool? UseDefaultImplementationPlan { get; set; }
    }
}
