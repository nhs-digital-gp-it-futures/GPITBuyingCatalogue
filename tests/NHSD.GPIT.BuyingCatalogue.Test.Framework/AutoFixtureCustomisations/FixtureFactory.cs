using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal static class FixtureFactory
    {
        private static readonly ICustomization[] Customizations =
        {
            new AutoMoqCustomization(),
            new OrderCustomization(),
            new CallOffIdCustomization(),
            new CatalogueItemIdCustomization(),
        };

        internal static IFixture Create() => Create(Customizations);

        internal static IFixture Create(params ICustomization[] customizations) =>
            new Fixture().Customize(new CompositeCustomization(Customizations.Union(customizations)));
    }
}
