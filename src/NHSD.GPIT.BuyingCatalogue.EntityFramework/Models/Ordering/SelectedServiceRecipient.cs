#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class SelectedServiceRecipient
    {
        public int OrderId { get; set; }

        public string OdsCode { get; set; }

        public virtual ServiceRecipient OdsCodeNavigation { get; set; }

        public virtual Order Order { get; set; }
    }
}
