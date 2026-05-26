using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.OrderType;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Generators;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData;

public class OrderTestData
{
    public string BaseUrl { get; set; } = "https://localhost:5001";

    public string Email { get; set; } = "suesmith@email.com";
    public string Password { get; set; } = "Pass123$";

    public OrderTypeOption OrderType { get; set; } = OrderTypeOption.CatalogueSolution;
    public FrameworkType Framework { get; set; } = FrameworkType.TechInnovation;

    public string Description { get; set; } = "Test Order";
    public string Supplier { get; set; } = "EMIS Health";
    public string Solution { get; set; } = "Emis Web GP";
    public string FundingType { get; set; } = "Local funding";

    public string FirstName { get; set; } = TestDataGenerator.FirstName();
    public string LastName { get; set; } = TestDataGenerator.LastName();
    public string Phone { get; set; } = TestDataGenerator.PhoneNumber();
    public string ContactEmail { get; set; } = TestDataGenerator.Email();

    private static readonly DateTime OrderStartDate = DateTime.Today.AddMonths(1);
    private static readonly DateTime DeliveryDate = OrderStartDate.AddDays(15);

    public string StartDay { get; set; } = OrderStartDate.Day.ToString("D2");
    public string StartMonth { get; set; } = OrderStartDate.Month.ToString("D2");
    public string StartYear { get; set; } = OrderStartDate.Year.ToString();
    public string InitialPeriod { get; set; } = "6";
    public string Duration { get; set; } = "12";

    public string Sublocation { get; set; } = "02T";
    public string[] Practices { get; set; } = { "BANKFIELD SURGERY", "BEECHWOOD MEDICAL CENTRE" };

    public Dictionary<string, string> Quantities { get; set; } = new()
    {
        { "BANKFIELD SURGERY", "2"  },
        { "BEECHWOOD MEDICAL CENTRE", "20" }
    };

    public string DeliveryDay { get; set; } = DeliveryDate.Day.ToString("D2");
    public string DeliveryMonth { get; set; } = DeliveryDate.Month.ToString("D2");
    public string DeliveryYear { get; set; } = DeliveryDate.Year.ToString();
}
