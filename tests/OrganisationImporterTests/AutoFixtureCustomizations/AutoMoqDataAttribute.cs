using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace OrganisationImporterTests.AutoFixtureCustomizations;

public class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute()
        : base(() => new Fixture()
            .Customize(new AutoMoqCustomization())
            .Customize(new CodeSystemsCustomization()))
    {
    }
}
