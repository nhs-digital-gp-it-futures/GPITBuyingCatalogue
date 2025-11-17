using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Shared;

    public static class SelectSublocationOverviewModelTests
    {
        [Theory]
        [InlineData(OrderTypeEnum.AssociatedServiceMerger, SelectSublocationsOverviewModel.MergerAdvice)]
        [InlineData(OrderTypeEnum.AssociatedServiceSplit, SelectSublocationsOverviewModel.SplitAdvice)]
        public static void Ctor_Sets_TitleAndAdvice_For_MergerOrSplit(
        OrderTypeEnum orderType,
        string expectedAdvice)
        {
            var order = new Order
            {
                OrderingParty = new EntityFramework.Organisations.Models.Organisation
                {
                    Name = "ICB Test",
                },
                OrderType = orderType,
            };

            var model = new SelectSublocationsOverviewModel(
                isConfirm: true,
                order,
                new List<SublocationModel>(),
                addOrChangeSublocationsLink: "/add",
                backLink: "/back");

            model.Title.Should().Be(SelectSublocationsOverviewModel.MergerOrSplitTitle);
            model.Advice.Should().Be(expectedAdvice);
            model.ProcessType.Should().Be("order");
            model.Caption.Should().Be(order.CallOffId.ToString());
        }
}
