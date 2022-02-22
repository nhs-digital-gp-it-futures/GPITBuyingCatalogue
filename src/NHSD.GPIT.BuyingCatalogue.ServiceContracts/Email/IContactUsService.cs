﻿using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email
{
    public interface IContactUsService
    {
        Task SubmitQuery(
            bool isTechnicalQuery,
            string fullName,
            string emailAddress,
            string message);
    }
}
