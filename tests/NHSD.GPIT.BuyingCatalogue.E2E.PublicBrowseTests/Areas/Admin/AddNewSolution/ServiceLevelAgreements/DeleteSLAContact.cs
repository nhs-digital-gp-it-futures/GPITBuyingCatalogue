using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ServiceLevelAgreements
{
    public sealed class DeleteSLAContact : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int ContactId = 2;
        private static readonly CatalogueItemId SolutionId = new(99998, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(ContactId), ContactId.ToString() },
        };

        public DeleteSLAContact(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ServiceLevelAgreementsController),
                  nameof(ServiceLevelAgreementsController.DeleteContact),
                  Parameters)
        {
        }

        [Fact]
        public void DeleteSLAContact_CorrectlyDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(SLAContactObjects.CancelLink).Should().BeTrue();
        }

        [Fact]
        public void DeleteSLAContact_ClickGoBack_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditContact))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void DeleteSLAContact_ClickCancel_ExpectedResult()
        {
            CommonActions.ClickLinkElement(SLAContactObjects.CancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditContact))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task DeleteSLAContact_ClickDelete_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();

            await using var context = GetEndToEndDbContext();

            var contact = await context.SlaContacts.SingleOrDefaultAsync(slac => slac.Id == ContactId);

            contact.Should().BeNull();
        }
    }
}
