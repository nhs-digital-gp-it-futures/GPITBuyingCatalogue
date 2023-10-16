using System;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    internal sealed class OrderCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            ISpecimenBuilder ComposerTransformation(ICustomizationComposer<Order> composer) => composer
                .FromFactory(new OrderSpecimenBuilder(fixture))
                .Without(x => x.Revision)
                .Do(x => x.Revision = 1)
                .With(o => o.OrderType, OrderTypeEnum.Solution)
                .Without(o => o.IsDeleted)
                .Without(o => o.IsTerminated)
                .Without(o => o.OrderTermination)
                .Without(o => o.LastUpdatedByUser)
                .Without(o => o.OrderItems)
                .Without(o => o.Contract)
                .Without(o => o.OrderRecipients);

            fixture.Customize<Order>(ComposerTransformation);
        }

        private sealed class OrderSpecimenBuilder : ISpecimenBuilder
        {
            private readonly IFixture fixture;

            public OrderSpecimenBuilder(IFixture fixture)
            {
                this.fixture = fixture;
            }

            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(Order)))
                    return new NoSpecimen();

                var item = new Order
                {
                    IsDeleted = false,
                };

                AddOrderItems(item, context);

                return item;
            }

            private static void AddOrderItemRecipients(Order order, OrderItem item, ISpecimenContext context)
            {
                foreach (var recipient in order.OrderRecipients)
                {
                    var orderItemRecipient = context.Create<OrderItemRecipient>();
                    orderItemRecipient.OrderId = item.OrderId;
                    orderItemRecipient.OrderItem = item;
                    orderItemRecipient.CatalogueItemId = item.CatalogueItemId;
                    orderItemRecipient.OdsCode = recipient.OdsCode;
                    orderItemRecipient.Recipient = recipient;
                    recipient.OrderItemRecipients.Add(orderItemRecipient);
                }
            }

            private void AddOrderItems(Order item, ISpecimenContext context)
            {
                var recipients = context.CreateMany<OrderRecipient>();

                foreach (var recipient in recipients)
                {
                    recipient.OrderId = item.Id;
                    recipient.Order = item;
                    item.OrderRecipients.Add(recipient);
                }

                var orderItems = fixture.Build<OrderItem>()
                .FromFactory(new OrderItemCustomization.OrderItemSpeciminBuilder())
                .Without(oi => oi.OrderItemFunding)
                .With(oi => oi.Order, item)
                .With(oi => oi.OrderId, item.Id)
                .Without(oi => oi.OrderItemPrice)
                .Without(oi => oi.CatalogueItem)
                .Without(oi => oi.CatalogueItemId)
                .CreateMany();

                foreach (var orderItem in orderItems)
                {
                    AddOrderItemRecipients(item, orderItem, context);
                    item.OrderItems.Add(orderItem);
                }
            }
        }
    }
}
