using System;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class WorkOffPlanCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<WorkOffPlan> composer) => composer
                .FromFactory(new WorkOffPlanSpecimenBuilder())
                .Without(wp => wp.Standard)
                .Without(wp => wp.StandardId)
                .Without(wp => wp.Solution)
                .Without(wp => wp.SolutionId);

            fixture.Customize<WorkOffPlan>(ComposerTransformation);
        }

        private sealed class WorkOffPlanSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(WorkOffPlan)))
                    return new NoSpecimen();

                var standard = context.Create<Standard>();

                var workOffPlan = new WorkOffPlan
                {
                    Standard = standard,
                    StandardId = standard.Id,
                };

                return workOffPlan;
            }
        }
    }
}
