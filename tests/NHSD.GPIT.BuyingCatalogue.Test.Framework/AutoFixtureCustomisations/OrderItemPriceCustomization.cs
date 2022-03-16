using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations.AutoFixtureExtensions;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal sealed class OrderItemPriceCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<OrderItemPrice> composer) => composer
                .FromFactory(new OrderItemPriceSpeciminBuilder())
                .Without(oip => oip.OrderItem)
                .Without(oip => oip.OrderId)
                .Without(oip => oip.OrderItemPriceTiers)
                .Without(oip => oip.CatalogueItemId)
                .With(oip => oip.EstimationPeriod, EntityFramework.Catalogue.Models.TimeUnit.PerMonth)
                .With(oip => oip.CurrencyCode, "GBP");

            fixture.Customize<OrderItemPrice>(ComposerTransformation);
        }

        private sealed class OrderItemPriceSpeciminBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(OrderItemPrice)))
                    return new NoSpecimen();

                var price = new OrderItemPrice();

                if (price.CataloguePriceType == CataloguePriceType.Flat)
                    AddOnePriceTier(price, context);
                else
                    AddMultiplePriceTiers(price, context);

                return price;
            }

            private static void AddOnePriceTier(OrderItemPrice price, ISpecimenContext context)
            {
                var tier = context.Create<OrderItemPriceTier>();

                tier.LowerRange = 1;
                tier.UpperRange = null;
                tier.OrderId = price.OrderId;
                tier.CatalogueItemId = price.CatalogueItemId;
                tier.OrderItemPrice = price;
                price.OrderItemPriceTiers.Add(tier);
            }

            private static void AddMultiplePriceTiers(OrderItemPrice price, ISpecimenContext context)
            {
                var tiers = context.CreateMany<OrderItemPriceTier>().ToList();
                var lastUpperRange = 0;
                for (int i = 0; i < tiers.Count; i++)
                {
                    tiers[i].LowerRange = lastUpperRange + 1;

                    tiers[i].UpperRange = i == tiers.Count - 1
                        ? null :
                        (lastUpperRange = context.CreateIntWithRange(lastUpperRange + 1, 500));

                    tiers[i].OrderId = price.OrderId;
                    tiers[i].CatalogueItemId = price.CatalogueItemId;
                    tiers[i].OrderItemPrice = price;
                    price.OrderItemPriceTiers.Add(tiers[i]);
                }
            }


        }
    }
}
