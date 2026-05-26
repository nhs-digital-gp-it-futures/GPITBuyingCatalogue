namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Builders;

public class OrderTestDataBuilder
{
    private readonly OrderTestData _data = new();

    public OrderTestDataBuilder WithBaseUrl(string url)
    {
        _data.BaseUrl = url;
        return this;
    }

    public OrderTestDataBuilder WithSupplier(string supplier)
    {
        _data.Supplier = supplier;
        return this;
    }

    public OrderTestDataBuilder WithSolution(string solution)
    {
        _data.Solution = solution;
        return this;
    }

    public OrderTestData Build() => _data;
}
