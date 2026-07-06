using System;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Generators;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData;

public class AdminTestData
{
    public string BaseUrl { get; set; } = "https://localhost:5001";

    public string Email { get; set; } = "bobsmith@email.com";
    public string Password { get; set; } = "Pass123$";

    public string Organisation = "NHS HUMBER AND NORTH YORKSHIRE INTEGRATED CARE BOARD";
    public string AccountType = "Buyer";
    public string AccountStatus = "Active";

    public string FirstName = TestDataGenerator.FirstName();
    public string LastName = TestDataGenerator.LastName();

    public string UserEmail =
        $"{TestDataGenerator.FirstName().ToLower()}.{TestDataGenerator.LastName().ToLower()}.{Guid.NewGuid().ToString("N")[..6]}@nhs.net";

    public string EmailDomain =
        $"@test{DateTime.UtcNow:HHmmssfff}.net";
}
