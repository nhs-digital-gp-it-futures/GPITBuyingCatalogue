using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace OrganisationImporterTests.AutoFixtureCustomizations;

public class InMemoryDbAutoMoqDataAttribute : AutoDataAttribute
{
    public InMemoryDbAutoMoqDataAttribute()
        : base(() => new Fixture()
            .Customize(new AutoMoqCustomization())
            .Customize(new InMemoryDbCustomization())
            .Customize(new CodeSystemsCustomization()))
    {
    }
}
