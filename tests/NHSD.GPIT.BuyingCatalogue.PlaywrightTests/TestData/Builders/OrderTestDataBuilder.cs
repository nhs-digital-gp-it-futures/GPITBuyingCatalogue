using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.OrderType;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Builders;

public class OrderTestDataBuilder
{
    private readonly OrderTestData _data = new();

    public OrderTestDataBuilder WithBaseUrl(string url) { _data.BaseUrl = url; return this; }
    public OrderTestDataBuilder WithSupplier(string supplier) { _data.Supplier = supplier; return this; }
    public OrderTestDataBuilder WithSolution(string solution) { _data.Solution = solution; return this; }
    public OrderTestDataBuilder WithFundingType(string funding) { _data.FundingType = funding; return this; }
    public OrderTestDataBuilder AsAssociatedService()
    {
        _data.OrderType = OrderTypeOption.AssociatedService;
        return this;
    }
    public OrderTestDataBuilder WithQuantities(Dictionary<string, string> quantities)
    {
        _data.Quantities = quantities;
        return this;
    }

    public OrderTestData Build() => _data;
}
