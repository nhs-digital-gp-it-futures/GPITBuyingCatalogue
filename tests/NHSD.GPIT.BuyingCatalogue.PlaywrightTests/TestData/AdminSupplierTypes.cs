namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData;

public record SupplierAddress(
    string Line1,
    string Line2,
    string Line3,
    string Town,
    string Postcode,
    string Country);

public record SupplierContact(
    string FirstName,
    string LastName,
    string Department,
    string Phone,
    string Email);
