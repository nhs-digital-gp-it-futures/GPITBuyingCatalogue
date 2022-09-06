using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;

namespace NHSD.GPIT.BuyingCatalogue.Services.Users
{
    public sealed class CreateUserService : ICreateUserService
    {
        private readonly UserManager<AspNetUser> userManager;
        private readonly IPasswordService passwordService;
        private readonly IPasswordResetCallback passwordResetCallback;
        private readonly IGovNotifyEmailService govNotifyEmailService;
        private readonly RegistrationSettings settings;

        public CreateUserService(
            UserManager<AspNetUser> userManager,
            IPasswordService passwordService,
            IPasswordResetCallback passwordResetCallback,
            IGovNotifyEmailService govNotifyEmailService,
            RegistrationSettings settings)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
            this.passwordResetCallback = passwordResetCallback ?? throw new ArgumentNullException(nameof(passwordResetCallback));
            this.govNotifyEmailService = govNotifyEmailService ?? throw new ArgumentNullException(nameof(govNotifyEmailService));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<AspNetUser> Create(
            int primaryOrganisationId,
            string firstName,
            string lastName,
            string emailAddress,
            string organisationFunction)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
                throw new ArgumentException($"{nameof(emailAddress)} must be provided.", nameof(emailAddress));

            var aspNetUser = new AspNetUser
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = emailAddress,
                Email = emailAddress,
                PrimaryOrganisationId = primaryOrganisationId,
            };

            await userManager.CreateAsync(aspNetUser);
            await userManager.AddToRoleAsync(aspNetUser, organisationFunction);

            var token = await passwordService.GeneratePasswordResetTokenAsync(aspNetUser.Email);

            await SendInitialEmailAsync(token);

            return aspNetUser;
        }

        private Task SendInitialEmailAsync(PasswordResetToken token)
        {
            if (token is null)
                throw new ArgumentNullException(nameof(token));

            var user = token.User;

            var personalisation = new Dictionary<string, dynamic>
            {
                ["password_set_link"] = passwordResetCallback.GetPasswordResetCallback(token),
            };

            return govNotifyEmailService.SendEmailAsync(
                user.Email,
                settings.EmailTemplateId,
                personalisation);
        }
    }
}
