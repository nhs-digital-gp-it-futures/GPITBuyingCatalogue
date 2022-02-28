using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;

namespace NHSD.GPIT.BuyingCatalogue.Services.Session
{
    public class SupplierContactSessionService : ISupplierContactSessionService
    {
        private readonly ISessionService sessionService;

        public SupplierContactSessionService(ISessionService sessionService)
        {
            this.sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        public SupplierContact GetSupplierContact(CallOffId callOffId, int supplierId)
        {
            return sessionService.GetObject<SupplierContact>(SessionKey(callOffId, supplierId));
        }

        public void SetSupplierContact(CallOffId callOffId, int supplierId, SupplierContact contact)
        {
            sessionService.SetObject(SessionKey(callOffId, supplierId), contact);
        }

        private static string SessionKey(CallOffId callOffId, int supplierId) => $"{callOffId}-{supplierId}-newContact";
    }
}
