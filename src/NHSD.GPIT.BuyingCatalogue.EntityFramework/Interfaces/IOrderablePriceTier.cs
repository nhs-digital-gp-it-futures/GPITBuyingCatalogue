namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;

public interface IOrderablePriceTier : IPriceTier
{
    public decimal ListPrice { get; set; }
}
