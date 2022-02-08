using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.Services.CreateBuyer
{
    public sealed class CreateBuyerService : ICreateBuyerService
    {
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly IPasswordService passwordService;
        private readonly IPasswordResetCallback passwordResetCallback;
        private readonly IGovNotifyEmailService govNotifyEmailService;
        private readonly RegistrationSettings settings;

        public CreateBuyerService(
            BuyingCatalogueDbContext dbContext,
            IPasswordService passwordService,
            IPasswordResetCallback passwordResetCallback,
            IGovNotifyEmailService govNotifyEmailService,
            RegistrationSettings settings)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
            this.passwordResetCallback = passwordResetCallback ?? throw new ArgumentNullException(nameof(passwordResetCallback));
            this.govNotifyEmailService = govNotifyEmailService ?? throw new ArgumentNullException(nameof(govNotifyEmailService));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<AspNetUser> Create(int primaryOrganisationId, string firstName, string lastName, string phoneNumber, string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
                throw new ArgumentException($"{nameof(emailAddress)} must be provided.", nameof(emailAddress));

            var aspNetUser = new AspNetUser
            {
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                UserName = emailAddress,
                NormalizedUserName = emailAddress.ToUpperInvariant(),
                Email = emailAddress,
                NormalizedEmail = emailAddress.ToUpperInvariant(),
                PrimaryOrganisationId = primaryOrganisationId,
                OrganisationFunction = OrganisationFunction.Buyer.DisplayName,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };

            dbContext.AspNetUsers.Add(aspNetUser);

            await dbContext.SaveChangesAsync();

            var token = await passwordService.GeneratePasswordResetTokenAsync(aspNetUser.Email);

            await SendInitialEmailAsync(token);

            return aspNetUser;
        }

        public Task<bool> UserExistsWithEmail(string emailAddress) => Task.FromResult(dbContext.AspNetUsers.Any(u => u.NormalizedUserName == emailAddress.ToUpperInvariant()));

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
