using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public sealed class PublicCloud
    {
        [StringLength(1000)]
        [Url]
        public string Link { get; set; }

        public string RequiresHscn { get; set; }

        [StringLength(500)]
        public string Summary { get; set; }

        public TaskProgress Status()
        {
            if (string.IsNullOrEmpty(Summary))
                return TaskProgress.NotStarted;

            return TaskProgress.Completed;
        }
    }
}
