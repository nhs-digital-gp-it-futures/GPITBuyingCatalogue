using System;
using AutoFixture;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class HostingTypeSectionModelCustomization : ICustomization
    {
        private static CatalogueItem catalogueItem;

        public void Customize(IFixture fixture)
        {
            fixture.Customize<HostingTypeSectionModel>(c => c.FromFactory(new MethodInvoker(new GreedyConstructorQuery()))
                .Without(m => m.SolutionId)
                .Without(m => m.SolutionName));
        }

        private sealed class HostingTypeSectionModelSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(HostingTypeSectionModel)))
                    return new NoSpecimen();

                return new HostingTypeSectionModel(catalogueItem);
            }
        }
    }
}
