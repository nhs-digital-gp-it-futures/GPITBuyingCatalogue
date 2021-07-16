using AutoFixture;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class AddSolutionModelCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<AddSolutionModel>(
                c => c.With(m => m.SupplierId, fixture.Create<int>().ToString));
        }
    }
}
