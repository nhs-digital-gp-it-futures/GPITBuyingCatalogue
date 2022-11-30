namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

public partial class OdsOrganisation
{
    public void UpdateFrom(OdsOrganisation source)
    {
        if (source == null)
        {
            return;
        }

        Name = source.Name;
        IsActive = source.IsActive;
        AddressLine1 = source.AddressLine1;
        AddressLine2 = source.AddressLine2;
        AddressLine3 = source.AddressLine3;
        Town = source.Town;
        County = source.County;
        Postcode = source.Postcode;
        Country = source.Country;
    }
}
