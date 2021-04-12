using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity
{
    public interface IPasswordResetCallback
    {
        Uri GetPasswordResetCallback(PasswordResetToken token);
    }
}
