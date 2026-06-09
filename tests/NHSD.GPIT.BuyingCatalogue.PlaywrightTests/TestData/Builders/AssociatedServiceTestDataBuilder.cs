namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Builders;

public class AssociatedServiceTestDataBuilder
{
    private readonly AssociatedServiceTestData _data = new();

    public AssociatedServiceTestDataBuilder WithBaseUrl(string url)
    {
        _data.BaseUrl = url;
        return this;
    }

    public AssociatedServiceTestDataBuilder WithServiceCategory(string category)
    {
        _data.AssociatedServiceCategory = category;
        return this;
    }

    public AssociatedServiceTestDataBuilder WithSupplier(string supplier, bool isMerger = false)
    {
        _data.Supplier = supplier;
        _data.IsMerger = isMerger;
        return this;
    }

    public AssociatedServiceTestDataBuilder WithCatalogueSolutionAndAssociatedService(string solution, string variant)
    {
        _data.SolutionWithAssociatedService = solution;
        _data.HasServiceVariant = true;
        _data.AssociatedService = variant;
        _data.RequiresQuantities = true;
        return this;
    }

    public AssociatedServiceTestDataBuilder WithCatalogueSolutionForMerger(string solution)
    {
        _data.SolutionWithAssociatedService = solution;
        _data.HasServiceVariant = false;
        _data.RequiresQuantities = false;
        return this;
    }

    public AssociatedServiceTestDataBuilder WithPracticesAndRecipientToBeMerged(string[] practices, string recipientToBeMerged)
    {
        _data.Practices = practices;
        _data.RequiresRecipientToBeMerged = true;
        _data.RecipientToBeMerged = recipientToBeMerged;
        return this;
    }

    public AssociatedServiceTestDataBuilder WithFundingFilter(string filter)
    {
        _data.FundingFilter = filter;
        return this;
    }

    public AssociatedServiceTestData Build() => _data;
}
