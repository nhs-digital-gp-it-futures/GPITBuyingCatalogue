namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models
{
    public sealed class RelatedOrganisation
    {
        public int OrganisationId { get; set; }

        public int RelatedOrganisationId { get; set; }

        public Organisation Organisation { get; set; }

        public Organisation RelatedOrganisationNavigation { get; set; }
    }
}
