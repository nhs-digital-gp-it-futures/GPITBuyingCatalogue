namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public enum CataloguePriceCalculationType
    {
        // Cumulative is where for each band, you times the quantity covered by that band by the price
        // e.g if a tier is 1-10 items, and you have 10 items, you times the price by 10
        Cumulative = 1,

        // SingleFixed returns one price depending on which band you land in, eg, if a tier is 1-10 items, and you have 1, you pay one price, if you have 10, you pay the same price
        SingleFixed = 2,
    }
}
