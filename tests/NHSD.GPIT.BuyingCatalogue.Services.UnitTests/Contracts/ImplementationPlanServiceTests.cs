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
using Contract = NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models.Contract;

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
            dbContext.ChangeTracker.Clear();

            var output = await service.GetDefaultImplementationPlan();

            output.Should().BeEquivalentTo(plan);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddImplementationPlan_ImplementationPlanExists_ReturnsExistingContract(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ImplementationPlanService service,
            ImplementationPlan implementationPlan)
        {
            var contract = new Contract { OrderId = orderId, ImplementationPlan = implementationPlan, };

            dbContext.Contracts.Add(contract);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var output = await service.AddImplementationPlan(orderId, contract.Id);

            output.Should().NotBeNull();
            output.OrderId.Should().Be(orderId);
            output.ImplementationPlan.Should().NotBeNull();
            output.ImplementationPlan.Id.Should().Be(implementationPlan.Id);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddImplementationPlan_ImplementationPlanDoesNotExist_ReturnsContract(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ImplementationPlanService service)
        {
            var contract = new Contract { OrderId = orderId, ImplementationPlan = null};

            dbContext.Contracts.Add(contract);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var output = await service.AddImplementationPlan(orderId, contract.Id);

            output.Should().NotBeNull();
            output.OrderId.Should().Be(orderId);
            output.ImplementationPlan.Should().NotBeNull();
            output.ImplementationPlan.IsDefault.Should().BeFalse();
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void AddBespokeMilestone_NullOrEmptyName_ThrowsException(
            string name,
            int orderId,
            int contractId,
            string paymentTrigger,
            ImplementationPlanService service)
        {
            FluentActions
                .Awaiting(
                    () => service.AddBespokeMilestone(
                        orderId,
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
            int orderId,
            int contractId,
            string name,
            ImplementationPlanService service)
        {
            FluentActions
                .Awaiting(
                    () => service.AddBespokeMilestone(
                        orderId,
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
            int orderId,
            int contractId,
            string name,
            ImplementationPlanService service)
        {
            FluentActions
                .Awaiting(
                    () => service.AddBespokeMilestone(
                        orderId,
                        contractId,
                        name,
                        paymentTrigger))
                .Should()
                .ThrowAsync<ArgumentException>(nameof(contractId));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddBespokeMilestone_NullImplementationPlan_PlanAndMilestoneCreated(
            [Frozen] BuyingCatalogueDbContext context,
            string paymentTrigger,
            string name,
            Contract contract,
            Order order,
            ImplementationPlanService service)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            contract.Order = order;
            contract.ImplementationPlan = null;
            context.Contracts.Add(contract);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            await service.AddBespokeMilestone(
                order.Id,
                contract.Id,
                name,
                paymentTrigger);

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
            Order order,
            Contract contract,
            ImplementationPlanService service)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            contract.Order = order;
            contract.ImplementationPlan = new ImplementationPlan();

            context.Contracts.Add(contract);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            await service.AddBespokeMilestone(
                order.Id,
                contract.Id,
                name,
                paymentTrigger);

            var actual = await context.Contracts.FirstAsync(f => f.Id == contract.Id);
            actual.ImplementationPlan.Should().NotBeNull();
            actual.ImplementationPlan.Milestones.Should().NotBeNull();
            actual.ImplementationPlan.Milestones.Count.Should().Be(1);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetMilestone_NoMilestoneExists_ReturnsNull(
            int orderId,
            int milestoneId,
            ImplementationPlanService service)
        {
            var milestone = await service.GetMilestone(orderId, milestoneId);

            milestone.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetMilestone_MilestoneExists_ReturnsMilestone(
            [Frozen] BuyingCatalogueDbContext dbContext,
            ImplementationPlan plan,
            ImplementationPlanMilestone milestone,
            Order order,
            Contract contract,
            ImplementationPlanService service)
        {
            contract.Order = order;

            plan.Milestones.Add(milestone);
            plan.Contract = contract;
            dbContext.ImplementationPlans.Add(plan);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var output = await service.GetMilestone(order.Id, milestone.Id);

            output.Id.Should().Be(milestone.Id);
            output.Title.Should().Be(milestone.Title);
            output.PaymentTrigger.Should().Be(milestone.PaymentTrigger);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void EditMilestone_NullOrEmptyName_ThrowsException(
           string name,
           int orderId,
           int milestoneId,
           string paymentTrigger,
           ImplementationPlanService service)
        {
            FluentActions
                .Awaiting(
                    () => service.EditMilestone(
                        orderId,
                        milestoneId,
                        name,
                        paymentTrigger))
                .Should()
                .ThrowAsync<ArgumentNullException>(nameof(name));
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void EditMilestone_NullOrEmptyPaymentTrigger_ThrowsException(
            string paymentTrigger,
            int orderId,
            int milestoneId,
            string name,
            ImplementationPlanService service)
        {
            FluentActions
                .Awaiting(
                    () => service.EditMilestone(
                        orderId,
                        milestoneId,
                        name,
                        paymentTrigger))
                .Should()
                .ThrowAsync<ArgumentNullException>(nameof(paymentTrigger));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task EditMilestone_ExistingMilestone_MilestoneUpdated(
            [Frozen] BuyingCatalogueDbContext context,
            string paymentTrigger,
            string name,
            ImplementationPlan plan,
            ImplementationPlanMilestone milestone,
            Order order,
            Contract contract,
            ImplementationPlanService service)
        {
            contract.Order = order;

            plan.Milestones.Add(milestone);
            plan.Contract = contract;
            context.ImplementationPlans.Add(plan);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            await service.EditMilestone(
                order.Id,
                milestone.Id,
                name,
                paymentTrigger);

            var actual = await context.ImplementationPlanMilestones.FirstAsync(f => f.Id == milestone.Id);
            actual.Title.Should().Be(name);
            actual.PaymentTrigger.Should().Be(paymentTrigger);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeleteMilestone_ExistingMilestone_MilestoneDeleted(
            [Frozen] BuyingCatalogueDbContext context,
            ImplementationPlan plan,
            ImplementationPlanMilestone milestone,
            Order order,
            Contract contract,
            ImplementationPlanService service)
        {
            contract.Order = order;

            plan.Milestones.Add(milestone);
            plan.Contract = contract;
            context.ImplementationPlans.Add(plan);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var before = await context.ImplementationPlanMilestones.FirstOrDefaultAsync(f => f.Id == milestone.Id);
            before.Should().NotBeNull();

            await service.DeleteMilestone(
                order.Id,
                milestone.Id);

            var actual = await context.ImplementationPlanMilestones.FirstOrDefaultAsync(f => f.Id == milestone.Id);
            actual.Should().BeNull();
        }
    }
}
