using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models;

public interface ISupplierDetailsModel
{
    public string Summary { get; }

    public string Name { get; }

    public string Url { get; }

    public List<SupplierContactViewModel> Contacts { get; init; }

    public bool HasContacts();
}
