using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.OrderType;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Generators;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData;

public class OrderTestData
{
    // App
    public string BaseUrl { get; set; } = "https://localhost:5001";

    // Login
    public string Email { get; set; } = "suesmith@email.com";
    public string Password { get; set; } = "Pass123$";

    // Order type
    public OrderTypeOption OrderType { get; set; } = OrderTypeOption.CatalogueSolution;
    public FrameworkType Framework { get; set; } = FrameworkType.TechInnovation;

    // Order details
    public string Description { get; set; } = "Test Order";
    public string Supplier { get; set; } = "EMIS Health";
    public string Solution { get; set; } = "Emis Web GP";
    public string FundingType { get; set; } = "Local funding";

    // Primary contact — auto-generated per test run
    public string FirstName { get; set; } = TestDataGenerator.FirstName();
    public string LastName { get; set; } = TestDataGenerator.LastName();
    public string Phone { get; set; } = TestDataGenerator.PhoneNumber();
    public string ContactEmail { get; set; } = TestDataGenerator.Email();

    // Dates — calculated relative to today so they never go stale
    private static readonly DateTime OrderStartDate = DateTime.Today.AddMonths(1);
    private static readonly DateTime DeliveryDate = OrderStartDate.AddDays(15);

    // Timescales (one month from run date)
    public string StartDay { get; set; } = OrderStartDate.Day.ToString("D2");
    public string StartMonth { get; set; } = OrderStartDate.Month.ToString("D2");
    public string StartYear { get; set; } = OrderStartDate.Year.ToString();
    public string InitialPeriod { get; set; } = "6";
    public string Duration { get; set; } = "12";

    // Service recipients
    public string Sublocation { get; set; } = "02T";
    public string[] Practices { get; set; } = { "BANKFIELD SURGERY", "BEECHWOOD MEDICAL CENTRE" };

    // Quantities
    public Dictionary<string, string> Quantities { get; set; } = new()
    {
        { "BANKFIELD SURGERY", "2"  },
        { "BEECHWOOD MEDICAL CENTRE", "20" }
    };

    // Delivery dates (15 days after order start date)
    public string DeliveryDay { get; set; } = DeliveryDate.Day.ToString("D2");
    public string DeliveryMonth { get; set; } = DeliveryDate.Month.ToString("D2");
    public string DeliveryYear { get; set; } = DeliveryDate.Year.ToString();
}
