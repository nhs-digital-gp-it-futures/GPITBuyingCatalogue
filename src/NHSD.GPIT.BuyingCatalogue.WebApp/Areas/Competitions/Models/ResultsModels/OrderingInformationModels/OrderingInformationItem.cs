using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels.OrderingInformationModels;

public class OrderingInformationItem
{
    public OrderingInformationItem(
        CatalogueItem catalogueItem,
        IPrice price,
        int quantity)
    {
        CatalogueItemType = catalogueItem.CatalogueItemType;
        CatalogueItemName = catalogueItem.Name;
        SupplierName = catalogueItem.Supplier.LegalName;
        Price = price;
        Quantity = quantity;
    }

    public CatalogueItemType CatalogueItemType { get; set; }

    public string CatalogueItemName { get; set; }

    public string SupplierName { get; set; }

    public IPrice Price { get; set; }

    public int Quantity { get; set; }
}
