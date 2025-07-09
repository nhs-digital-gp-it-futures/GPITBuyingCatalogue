using System;
using System.Collections.Generic;
using System.Linq;
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
                    .With(o => o.Revision, 1)
                    .With(o => o.OrderType, OrderTypeEnum.Solution)
                    .With(o => o.OrderNumber, () => new Random().Next(1, 999999))
                    .With(o => o.MaximumTerm, () => new Random().Next(1, 120))
                    .Without(o => o.IsDeleted)
                    .Without(o => o.IsTerminated)
                    .Without(o => o.OrderTermination)
                    .Without(o => o.LastUpdatedByUser)
                    .Without(o => o.OrderItems)
                    .Without(o => o.Contract)
                    .Without(o => o.OrderSublocations);
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

                var item = new Order { IsDeleted = false, Id = new Random().Next(10001, 999999) };

                AddOrderingParty(item);
                AddOrderSublocations(item);
                AddOrderItems(item);

                return item;
            }

            private void AddOrderingParty(Order order)
            {
                Organisation orderingParty = fixture.Build<Organisation>()
                    .Without(o => o.Orders)
                    .Without(o => o.RelatedOrganisationOrganisations)
                    .Without(o => o.RelatedOrganisationRelatedOrganisationNavigations)
                    .Create();

                order.OrderingParty = orderingParty;
                order.OrderingPartyId = orderingParty.Id;
            }

            private void AddOrderItemSublocationRecipients(Order order, OrderItem item)
            {
                foreach (OrderSublocationRecipient recipient in order.FlattenedRecipients)
                {
                    OrderItemSublocationRecipient orderItemRecipient = fixture.Build<OrderItemSublocationRecipient>()
                        .With(oisr => oisr.OrderId, order.Id)
                        .With(oisr => oisr.OrderItem, item)
                        .With(oisr => oisr.CatalogueItemId, item.CatalogueItemId)
                        .With(oisr => oisr.RecipientOdsCode, recipient.RecipientOdsCode)
                        .With(oisr => oisr.Recipient, recipient)
                        .Create();
                    recipient.OrderItemSublocationRecipients.Add(orderItemRecipient);
                }
            }

            private void AddOrderSublocations(Order order)
            {
                List<OrderSublocation> sublocations = fixture.Build<OrderSublocation>()
                    .With(os => os.Order, order)
                    .With(os => os.OrderId, order.Id)
                    .With(os => os.OwnerOdsCode, order.OrderingParty.ExternalIdentifier)
                    .Without(os => os.SublocationRecipients)
                    .CreateMany()
                    .ToList();

                foreach (OrderSublocation sublocation in sublocations)
                {
                    List<OrderSublocationRecipient> orderSublocationRecipients =
                        fixture.Build<OrderSublocationRecipient>()
                            .With(osr => osr.Order, order)
                            .With(osr => osr.OrderId, order.Id)
                            .With(osr => osr.ParentSublocation, sublocation)
                            .With(osr => osr.ParentSublocationOdsCode, sublocation.SublocationOdsCode)
                            .Without(osr => osr.OrderItemSublocationRecipients)
                            .CreateMany()
                            .ToList();

                    sublocation.SublocationRecipients = orderSublocationRecipients;
                }

                order.OrderSublocations = sublocations;
            }

            private void AddOrderItems(Order item)
            {
                IEnumerable<OrderItem> orderItems = fixture.Build<OrderItem>()
                    .FromFactory(new OrderItemCustomization.OrderItemSpecimenBuilder())
                    .Without(oi => oi.OrderItemFunding)
                    .With(oi => oi.Order, item)
                    .With(oi => oi.OrderId, item.Id)
                    .Without(oi => oi.OrderItemPrice)
                    .Without(oi => oi.CatalogueItem)
                    .Without(oi => oi.CatalogueItemId)
                    .CreateMany();

                foreach (var orderItem in orderItems)
                {
                    AddOrderItemSublocationRecipients(item, orderItem);
                    item.OrderItems.Add(orderItem);
                }
            }
        }
    }
}
