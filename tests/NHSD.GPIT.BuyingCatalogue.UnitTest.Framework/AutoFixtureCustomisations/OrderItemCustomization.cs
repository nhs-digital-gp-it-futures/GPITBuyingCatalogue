using System;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    internal sealed class OrderItemCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<OrderItem> composer)
            {
                return composer
                    .FromFactory(new OrderItemSpecimenBuilder())
                    .Without(oi => oi.OrderItemFunding)
                    .Without(oi => oi.Order)
                    .Without(oi => oi.OrderId)
                    .Without(oi => oi.OrderItemPrice)
                    .Without(oi => oi.CatalogueItem)
                    .Without(oi => oi.CatalogueItemId);
            }

            fixture.Customize<OrderItem>(ComposerTransformation);
        }

        public sealed class OrderItemSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(OrderItem)))
                    return new NoSpecimen();

                var item = new OrderItem();

                AddOrderItemCatalogueItem(item, context);
                AddOrderItemPrice(item, context);
                AddOrderItemFunding(item, context);

                return item;
            }

            private static void AddOrderItemFunding(OrderItem item, ISpecimenContext context)
            {
                var funding = context.Create<OrderItemFunding>();

                funding.OrderItem = item;
                funding.OrderItemId = item.Id;

                item.OrderItemFunding = funding;
            }

            private static void AddOrderItemPrice(OrderItem item, ISpecimenContext context)
            {
                var price = context.Create<OrderItemPrice>();

                price.OrderItem = item;
                price.OrderItemId = item.Id;
                price.OrderItemPriceTiers.ForEach(x => x.OrderItemPriceId = price.Id);

                item.OrderItemPrice = price;
            }

            private static void AddOrderItemCatalogueItem(OrderItem item, ISpecimenContext context)
            {
                var solution = context.Create<CatalogueItem>();

                item.CatalogueItemId = solution.Id;
                item.CatalogueItem = solution;
            }
        }
    }
}
