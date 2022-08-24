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
            .FromFactory(new OrderSpeciminBuilder(fixture))
                .Without(o => o.IsDeleted)
                .Without(o => o.LastUpdatedByUser)
                .Without(o => o.OrderItems);

            fixture.Customize<Order>(ComposerTransformation);
        }

        private sealed class OrderSpeciminBuilder : ISpecimenBuilder
        {
            private readonly IFixture fixture;

            public OrderSpeciminBuilder(IFixture fixture)
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
                AddOrderItems(item);

                return item;
            }

            private void AddOrderItems(Order item)
            {
                var orderItems = fixture.Build<OrderItem>()
                .FromFactory(new OrderItemCustomization.OrderItemSpeciminBuilder())
                .Without(oi => oi.OrderItemFunding)
                .With(oi => oi.Order, item)
                .With(oi => oi.OrderId, item.Id)
                .Without(oi => oi.OrderItemPrice)
                .Without(oi => oi.OrderItemRecipients)
                .Without(oi => oi.CatalogueItem)
                .Without(oi => oi.CatalogueItemId)
                .CreateMany();

                foreach (var orderItem in orderItems)
                {
                    item.OrderItems.Add(orderItem);
                }
            }
        }
    }
}
