using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    internal sealed class OrderCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            ISpecimenBuilder ComposerTransformation(ICustomizationComposer<Order> composer)
            {
                return composer
                    .FromFactory(new OrderSpecimenBuilder(fixture))
                    .Without(x => x.Revision)
                    .Do(x => x.Revision = 1)
                    .With(o => o.OrderType, OrderTypeEnum.Solution)
                    .With(o => o.OrderNumber, () => new Random().Next(1, 999999))
                    .Without(o => o.IsDeleted)
                    .Without(o => o.IsTerminated)
                    .Without(o => o.OrderTermination)
                    .Without(o => o.LastUpdatedByUser)
                    .Without(o => o.OrderItems)
                    .Without(o => o.Contract)
                    .Without(o => o.OrderSublocations)
                    .Without(o => o.OrderRecipients);
            }

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

                AddOrderingParty(item, context);
                AddOrderSublocations(item, context);
                AddOrderItems(item, context);

                return item;
            }

            private static void AddOrderingParty(Order order, ISpecimenContext context)
            {
                var organisation = context.Create<Organisation>();

                order.OrderingParty = organisation;
                order.OrderingPartyId = organisation.Id;
            }

            private static void AddOrderSublocations(Order order, ISpecimenContext context)
            {
                IEnumerable<OrderSublocation> sublocations = context.CreateMany<OrderSublocation>();

                foreach (OrderSublocation sublocation in sublocations)
                {
                    sublocation.OrderId = order.Id;
                    sublocation.OwnerOdsCode = order.OrderingParty.ExternalIdentifier;

                    IEnumerable<OrderSublocationRecipient> orderSublocationRecipients =
                        context.CreateMany<OrderSublocationRecipient>();

                    foreach (OrderSublocationRecipient osr in orderSublocationRecipients)
                    {
                        osr.ParentSublocationOdsCode = sublocation.SublocationOdsCode;

                        sublocation.SublocationRecipients.Add(osr);
                    }

                    order.OrderSublocations.Add(sublocation);
                }
            }

            private static void AddOrderItemSublocationRecipients(Order order, OrderItem item, ISpecimenContext context)
            {
                foreach (OrderSublocationRecipient recipient in order.FlattenedRecipients)
                {
                    var orderItemRecipient = context.Create<OrderItemSublocationRecipient>();
                    orderItemRecipient.OrderId = item.OrderId;
                    orderItemRecipient.OrderItem = item;
                    orderItemRecipient.CatalogueItemId = item.CatalogueItemId;
                    orderItemRecipient.OdsCode = recipient.RecipientOdsCode;
                    orderItemRecipient.Recipient = recipient;
                    recipient.OrderItemSublocationRecipients.Add(orderItemRecipient);
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
                    AddOrderItemSublocationRecipients(item, orderItem, context);
                    item.OrderItems.Add(orderItem);
                }
            }
        }
    }
}
