﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.Services.Email;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Email
{
    public static class ContactUsServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ContactUsService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static Task SubmitQuery_NullOrEmptyFullName_ThrowsException(
            string fullName,
            bool isTechnicalQuery,
            string emailAddress,
            string message,
            ContactUsService service) => Assert.ThrowsAsync<ArgumentNullException>(() => service.SubmitQuery(isTechnicalQuery, fullName, emailAddress, message));

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static Task SubmitQuery_NullOrEmptyEmailAddress_ThrowsException(
            string emailAddress,
            bool isTechnicalQuery,
            string fullName,
            string message,
            ContactUsService service) => Assert.ThrowsAsync<ArgumentNullException>(() => service.SubmitQuery(isTechnicalQuery, fullName, emailAddress, message));

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static Task SubmitQuery_NullOrEmptyMessage_ThrowsException(
            string message,
            bool isTechnicalQuery,
            string fullName,
            string emailAddress,
            ContactUsService service) => Assert.ThrowsAsync<ArgumentNullException>(() => service.SubmitQuery(isTechnicalQuery, fullName, emailAddress, message));

        [Theory]
        [CommonAutoData]
        public static async Task SubmitQuery_TechnicalRequest_UsesCorrectEmail(
            string fullName,
            string emailAddress,
            string message,
            [Frozen] ContactUsSettings settings,
            [Frozen] Mock<IGovNotifyEmailService> govNotifyEmailService,
            ContactUsService service)
        {
            await service.SubmitQuery(true, fullName, emailAddress, message);

            govNotifyEmailService.Verify(s => s.SendEmailAsync(settings.TechnicalFaultRecipient.Address, It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>()));
            govNotifyEmailService.Verify(s => s.SendEmailAsync(emailAddress, It.IsAny<string>(), null));
        }

        [Theory]
        [CommonAutoData]
        public static async Task SubmitQuery_GeneralEnquiry_UsesCorrectEmail(
            string fullName,
            string emailAddress,
            string message,
            [Frozen] ContactUsSettings settings,
            [Frozen] Mock<IGovNotifyEmailService> govNotifyEmailService,
            ContactUsService service)
        {
            await service.SubmitQuery(false, fullName, emailAddress, message);

            govNotifyEmailService.Verify(s => s.SendEmailAsync(settings.GeneralQueriesRecipient.Address, It.IsAny<string>(), It.IsAny<Dictionary<string, dynamic>>()));
            govNotifyEmailService.Verify(s => s.SendEmailAsync(emailAddress, It.IsAny<string>(), null));
        }
    }
}
