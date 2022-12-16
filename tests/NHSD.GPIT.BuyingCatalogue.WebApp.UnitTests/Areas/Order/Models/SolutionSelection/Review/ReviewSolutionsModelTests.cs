﻿using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Review;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.Review
{
    public static class ReviewSolutionsModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_CatalogueSolutionOnly_PropertiesSetCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems = order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution).ToList();

            var model = new ReviewSolutionsModel(order, internalOrgId);

            model.CallOffId.Should().BeEquivalentTo(order.CallOffId);
            model.CatalogueSolutions.Should().BeEquivalentTo(order.OrderItems);
            model.AdditionalServices.Should().BeEmpty();
            model.AssociatedServices.Should().BeEmpty();
            model.AllOrderItems.Should().HaveCount(1);
            model.ContractLength.Should().Be(order.MaximumTerm);
            model.InternalOrgId.Should().BeEquivalentTo(internalOrgId);
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_CatalogueSolutionAndAdditionalService_PropertiesSetCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.Last().CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            order.OrderItems = order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType != 0).ToList();

            var model = new ReviewSolutionsModel(order, internalOrgId);

            model.CallOffId.Should().BeEquivalentTo(order.CallOffId);
            model.CatalogueSolutions.Should().HaveCount(1);
            model.AdditionalServices.Should().HaveCount(1);
            model.AssociatedServices.Should().BeEmpty();
            model.AllOrderItems.Should().HaveCount(2);
            model.ContractLength.Should().Be(order.MaximumTerm);
            model.InternalOrgId.Should().BeEquivalentTo(internalOrgId);
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_CatalogueSolutionAndAssociatedService_PropertiesSetCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.Last().CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            order.OrderItems = order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType != 0).ToList();

            var model = new ReviewSolutionsModel(order, internalOrgId);

            model.CallOffId.Should().BeEquivalentTo(order.CallOffId);
            model.CatalogueSolutions.Should().HaveCount(1);
            model.AdditionalServices.Should().BeEmpty();
            model.AssociatedServices.Should().HaveCount(1);
            model.AllOrderItems.Should().HaveCount(2);
            model.ContractLength.Should().Be(order.MaximumTerm);
            model.InternalOrgId.Should().BeEquivalentTo(internalOrgId);
        }
    }
}
