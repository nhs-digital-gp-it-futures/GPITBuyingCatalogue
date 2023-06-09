using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Contracts;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Contracts
{
    public class ImplementationPlanServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ImplementationPlanService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetDefaultImplementationPlan_NoPlanExists_ReturnsNull(
            ImplementationPlanService service)
        {
            var plan = await service.GetDefaultImplementationPlan();

            plan.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetDefaultImplementationPlan_PlanExists_ReturnsPlan(
            [Frozen] BuyingCatalogueDbContext dbContext,
            ImplementationPlanService service)
        {
            var plan = new ImplementationPlan { IsDefault = true };

            dbContext.ImplementationPlans.Add(plan);

            await dbContext.SaveChangesAsync();

            var output = await service.GetDefaultImplementationPlan();

            output.Should().Be(plan);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void AddBespokeMilestone_NullOrEmptyName_ThrowsException(
            string name,
            int contractId,
            string paymentTrigger,
            ImplementationPlanService service)
        {
            FluentActions
                .Awaiting(
                    () => service.AddBespokeMilestone(
                        contractId,
                        name,
                        paymentTrigger))
                .Should()
                .ThrowAsync<ArgumentNullException>(nameof(name));
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void AddBespokeMilestone_NullOrEmptyPaymentTrigger_ThrowsException(
            string paymentTrigger,
            int contractId,
            string name,
            ImplementationPlanService service)
        {
            FluentActions
                .Awaiting(
                    () => service.AddBespokeMilestone(
                        contractId,
                        name,
                        paymentTrigger))
                .Should()
                .ThrowAsync<ArgumentNullException>(nameof(paymentTrigger));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void AddBespokeMilestone_NullContract_ThrowsException(
            string paymentTrigger,
            int contractId,
            string name,
            ImplementationPlanService service)
        {
            FluentActions
                .Awaiting(
                    () => service.AddBespokeMilestone(
                        contractId,
                        name,
                        paymentTrigger))
                .Should()
                .ThrowAsync<ArgumentException>(nameof(contractId));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddBespokeMilestone_NullImplementationPlan_IpAndMilestoneCreated(
            [Frozen] BuyingCatalogueDbContext context,
            string paymentTrigger,
            string name,
            Contract contract,
            ImplementationPlanService service)
        {
            contract.ImplementationPlan = null;
            context.Contracts.Add(contract);
            await context.SaveChangesAsync();

            var result = await service.AddBespokeMilestone(
                contract.Id,
                name,
                paymentTrigger);
            result.Should().NotBe(0);

            var actual = await context.Contracts.FirstAsync(f => f.Id == contract.Id);
            actual.ImplementationPlan.Should().NotBeNull();
            actual.ImplementationPlan.Milestones.Should().NotBeNull();
            actual.ImplementationPlan.Milestones.Count.Should().Be(1);

            var newMilestone = actual.ImplementationPlan.Milestones.First();
            newMilestone.Title.Should().Be(name);
            newMilestone.PaymentTrigger.Should().Be(paymentTrigger);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddBespokeMilestone_ExistingImplementationPlan_MilestoneAdded(
            [Frozen] BuyingCatalogueDbContext context,
            string paymentTrigger,
            string name,
            Contract contract,
            ImplementationPlanService service)
        {
            var numberOfMilestones = contract.ImplementationPlan?.Milestones?.Count;
            context.Contracts.Add(contract);

            await context.SaveChangesAsync();

            var result = await service.AddBespokeMilestone(
                contract.Id,
                name,
                paymentTrigger);
            result.Should().NotBe(0);

            var actual = await context.Contracts.FirstAsync(f => f.Id == contract.Id);
            actual.ImplementationPlan.Should().NotBeNull();
            actual.ImplementationPlan.Should().BeEquivalentTo(contract.ImplementationPlan);
            actual.ImplementationPlan.Milestones.Should().NotBeNull();
            actual.ImplementationPlan.Milestones.Count.Should().Be(numberOfMilestones + 1);
        }
    }
}
