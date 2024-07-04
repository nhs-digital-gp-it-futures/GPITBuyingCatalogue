namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public class Contract
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public ImplementationPlan ImplementationPlan { get; set; }

        public virtual ContractBilling ContractBilling { get; set; }

        public virtual Order Order { get; set; }
    }
}
