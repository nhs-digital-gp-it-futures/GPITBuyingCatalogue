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
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<OrderItem> composer) => composer
                .FromFactory(new OrderItemSpeciminBuilder())
                .Without(oi => oi.OrderItemFunding)
                .Without(oi => oi.Order)
                .Without(oi => oi.OrderId)
                .Without(oi => oi.OrderItemPrice)
                .Without(oi => oi.CatalogueItem)
                .Without(oi => oi.CatalogueItemId);

            fixture.Customize<OrderItem>(ComposerTransformation);
        }

        public sealed class OrderItemSpeciminBuilder : ISpecimenBuilder
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

                funding.CatalogueItemId = item.CatalogueItemId;
                funding.OrderId = item.OrderId;
                funding.OrderItem = item;

                item.OrderItemFunding = funding;
            }

            private static void AddOrderItemPrice(OrderItem item, ISpecimenContext context)
            {
                var price = context.Create<OrderItemPrice>();

                price.CatalogueItemId = item.CatalogueItemId;
                price.OrderId = item.OrderId;
                price.OrderItem = item;
                price.OrderItemPriceTiers.ForEach(x => x.CatalogueItemId = price.CatalogueItemId);

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
