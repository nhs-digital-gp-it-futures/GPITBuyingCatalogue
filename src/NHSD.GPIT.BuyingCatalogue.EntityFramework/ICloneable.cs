namespace NHSD.GPIT.BuyingCatalogue.EntityFramework;

public interface ICloneable<out T>
{
    T Clone(bool preserveIds = false);
}
