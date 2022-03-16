using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations.AutoFixtureExtensions;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal sealed class CataloguePriceCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<CataloguePrice> composer) => composer
                .FromFactory(new CataloguePriceSpecimenBuilder())
                .Without(p => p.CataloguePriceTiers)
                .Without(p => p.CatalogueItem)
                .Without(p => p.CatalogueItemId)
                .Without(p => p.CataloguePriceId)
                .Without(p => p.TimeUnit)
                .With(p => p.PublishedStatus, PublicationStatus.Published);

            fixture.Customize<CataloguePrice>(ComposerTransformation);
        }

        private sealed class CataloguePriceSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(CataloguePrice)))
                    return new NoSpecimen();

                var id = context.Create<int>();
                var price = new CataloguePrice
                {
                    CataloguePriceId = id,
                };

                if (price.CataloguePriceType == CataloguePriceType.Flat)
                    AddOnePriceTier(price, context);
                else
                    AddMultiplePriceTiers(price, context);

                AddTimeUnit(price, context);

                return price;
            }

            private static void AddOnePriceTier(CataloguePrice price, ISpecimenContext context)
            {
                var tier = context.Create<CataloguePriceTier>();

                tier.LowerRange = 1;
                tier.UpperRange = null;
                tier.CataloguePrice = price;
                tier.CataloguePriceId = price.CataloguePriceId;
                price.CataloguePriceTiers.Add(tier);
            }

            private static void AddMultiplePriceTiers(CataloguePrice price, ISpecimenContext context)
            {
                var tiers = context.CreateMany<CataloguePriceTier>().ToList();
                var lastUpperRange = 0;
                for (int i = 0; i < tiers.Count; i++)
                {
                    tiers[i].LowerRange = lastUpperRange + 1;

                    tiers[i].UpperRange = i == tiers.Count - 1
                        ? null :
                        (lastUpperRange = context.CreateIntWithRange(lastUpperRange + 1, 500));

                    tiers[i].CataloguePrice = price;
                    tiers[i].CataloguePriceId = price.CataloguePriceId;
                    price.CataloguePriceTiers.Add(tiers[i]);
                }
            }

            private static void AddTimeUnit(CataloguePrice price, ISpecimenContext context)
            {
                if (price.ProvisioningType == ProvisioningType.Patient)
                    price.TimeUnit = TimeUnit.PerYear;
                else
                    price.TimeUnit = context.Create<TimeUnit?>();
            }
        }
    }
}
