namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public class PublicationStatus : EnumerationBase
    {
        public static readonly PublicationStatus Draft = new(1, nameof(Draft));
        public static readonly PublicationStatus Unpublished = new(2, nameof(Unpublished));
        public static readonly PublicationStatus Published = new(3, nameof(Published));
        public static readonly PublicationStatus Withdrawn = new(4, nameof(Withdrawn));

        public PublicationStatus(int id, string name)
            : base(id, name)
        {
        }
    }
}
