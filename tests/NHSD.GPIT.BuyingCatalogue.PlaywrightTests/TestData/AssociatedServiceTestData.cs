using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.OrderType;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData;

public class AssociatedServiceTestData : OrderTestData
{

    public string AssociatedServiceCategory { get; set; } = "Something else";
    public string SolutionWithAssociatedService { get; set; } = "Emis Web GP";
    public bool HasServiceVariant { get; set; } = true;
    public string AssociatedService { get; set; } = "Engineering";
    public bool RequiresQuantities { get; set; } = true;
    public bool RequiresRecipientToBeMerged { get; set; } = false;
    public string RecipientToBeMerged { get; set; } = string.Empty;
    public bool IsMerger { get; set; } = false;
    public string FundingFilter { get; set; } = "Engineering";

    public AssociatedServiceTestData()
    {
        OrderType = OrderTypeOption.AssociatedService;
    }
}
