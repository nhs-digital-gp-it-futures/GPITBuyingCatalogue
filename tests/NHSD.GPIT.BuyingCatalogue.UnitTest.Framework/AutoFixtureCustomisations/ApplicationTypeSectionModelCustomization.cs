using AutoFixture;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class ApplicationTypeSectionModelCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<ApplicationTypeSectionModel>(c => c.FromFactory(new MethodInvoker(new GreedyConstructorQuery()))
                .Without(m => m.SolutionId)
                .Without(m => m.SolutionName));
        }
    }
}
