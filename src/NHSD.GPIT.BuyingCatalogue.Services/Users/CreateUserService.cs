using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;

namespace NHSD.GPIT.BuyingCatalogue.Services.Users
{
    public sealed class CreateUserService : ICreateUserService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public CreateUserService(
            BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<AspNetUser> Create(
            int primaryOrganisationId,
            string firstName,
            string lastName,
            string emailAddress,
            string organisationFunction,
            bool isDisabled = false)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
                throw new ArgumentException($"{nameof(emailAddress)} must be provided.", nameof(emailAddress));

            var selectedRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.Name == organisationFunction);

            var aspNetUser = new AspNetUser
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = emailAddress,
                Email = emailAddress,
                PrimaryOrganisationId = primaryOrganisationId,
                Disabled = isDisabled,
                AspNetUserRoles = new List<AspNetUserRole> { new() { RoleId = selectedRole.Id, }, },
            };

            dbContext.AspNetUsers.Add(aspNetUser);

            return aspNetUser;
        }
    }
}
