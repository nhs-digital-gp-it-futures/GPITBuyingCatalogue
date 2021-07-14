using AutoFixture;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class CreateSolutionModelCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CreateSolutionModel>(
                c => c.With(m => m.SupplierId, fixture.Create<int>().ToString));
        }
    }
}
