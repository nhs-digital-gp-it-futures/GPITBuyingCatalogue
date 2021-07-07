﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users
{
    public interface IUsersService
    {
        Task<AspNetUser> GetUser(string userId);

        Task<List<AspNetUser>> GetAllUsersForOrganisation(Guid organisationId);

        Task EnableOrDisableUser(string userId, bool disabled);
    }
}
