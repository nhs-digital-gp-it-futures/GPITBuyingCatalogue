using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ServiceLevelAgreements
{
    [Collection(nameof(AdminCollection))]
    public sealed class DeleteSLAContact : AuthorityTestBase
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
        public void DeleteSLAContact_ClickDelete_ExpectedResult()
        {
            using var context = GetEndToEndDbContext();
            var serviceLevelAgreement = context.ServiceLevelAgreements.Include(p => p.Contacts).First(p => p.SolutionId == SolutionId);
            var serviceContact = new SlaContact()
            {
                SolutionId = new CatalogueItemId(99998, "002"),
                Channel = "This is a Channel 2",
                ContactInformation = "This is Contact Information 2",
                TimeFrom = DateTime.UtcNow,
                TimeUntil = DateTime.UtcNow.AddHours(5),
            };

            serviceLevelAgreement.Contacts.Add(serviceContact);
            context.SaveChanges();

            var parameters = new Dictionary<string, string>
            {
                { nameof(SolutionId), SolutionId.ToString() },
                { nameof(ContactId), serviceContact.Id.ToString() },
            };

            NavigateToUrl(
                  typeof(ServiceLevelAgreementsController),
                  nameof(ServiceLevelAgreementsController.DeleteContact),
                  parameters);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();
        }
    }
}
