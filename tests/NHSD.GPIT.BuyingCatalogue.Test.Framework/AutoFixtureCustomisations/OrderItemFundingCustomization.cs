using System;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations.AutoFixtureExtensions;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal sealed class OrderItemFundingCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<OrderItemFunding> composer) => composer
                .FromFactory(new OrderItemFundingSpeciminBuilder())
                .Without(oif => oif.CatalogueItemId)
                .Without(oif => oif.OrderId)
                .Without(oif => oif.CentralAllocation)
                .Without(oif => oif.LocalAllocation)
                .Without(oif => oif.OrderItem);

            fixture.Customize<OrderItemFunding>(ComposerTransformation);
        }

        private sealed class OrderItemFundingSpeciminBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(OrderItemFunding)))
                    return new NoSpecimen();

                var item = new OrderItemFunding();

                var split = context.CreateIntWithRange(0, (int)item.TotalPrice);

                item.LocalAllocation = split;
                item.CentralAllocation = item.TotalPrice - split;

                return item;
            }
        }
    }
}
