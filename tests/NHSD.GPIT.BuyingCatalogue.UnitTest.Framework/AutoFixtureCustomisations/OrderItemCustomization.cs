using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations.AutoFixtureExtensions;

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
                .Without(oi => oi.OrderItemRecipients)
                .Without(oi => oi.CatalogueItem)
                .Without(oi => oi.CatalogueItemId);

            fixture.Customize<OrderItem>(ComposerTransformation);
        }

        private sealed class OrderItemSpeciminBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(OrderItem)))
                    return new NoSpecimen();

                var item = new OrderItem();

                AddOrderItemRecipients(item, context);
                AddOrderItemPrice(item, context);
                AddOrderItemFunding(item, context);
                AddOrderItemCatalogueItem(item, context);

                return item;
            }

            private static void AddOrderItemRecipients(OrderItem item, ISpecimenContext context)
            {
                var recipients = context.CreateMany<OrderItemRecipient>();

                foreach (var recipient in recipients)
                {
                    recipient.OrderItem = item;
                    recipient.OrderId = item.OrderId;
                    recipient.CatalogueItemId = item.CatalogueItemId;
                    item.OrderItemRecipients.Add(recipient);
                }
            }

            private static void AddOrderItemFunding(OrderItem item, ISpecimenContext context)
            {
                var funding = context.Create<OrderItemFunding>();

                funding.CatalogueItemId = item.CatalogueItemId;
                funding.OrderId = item.OrderId;
                funding.OrderItem = item;

                var totalQuantity = item.OrderItemRecipients.Sum(oir => oir.Quantity);

                var totalCost = item.OrderItemPrice.CalculateTotalCost(item.GetTotalRecipientQuantity());

                funding.TotalPrice = totalCost;

                funding.CentralAllocation = context.CreateDecimalWithRange(0, funding.TotalPrice);
                funding.LocalAllocation = funding.TotalPrice - funding.CentralAllocation;

                item.OrderItemFunding = funding;
            }

            private static void AddOrderItemPrice(OrderItem item, ISpecimenContext context)
            {
                var price = context.Create<OrderItemPrice>();

                price.CatalogueItemId = item.CatalogueItemId;
                price.OrderId = item.OrderId;
                price.OrderItem = item;

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
