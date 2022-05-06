using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class IntegrationCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            ISpecimenBuilder ComposerTransformation(ICustomizationComposer<Integration> composer) => composer
                .With(i => i.Qualifier, GetIntegrationQualifier);

            fixture.Customize<Integration>(ComposerTransformation);
        }

        private static string GetIntegrationQualifier()
        {
            var qualifiers = new List<string>
            {
                "IM1",
                "GP Connect",
            };

            return qualifiers[new Random().Next(qualifiers.Count)];
        }
    }
}
