using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public class OnPremise
    {
        [StringLength(1000)]
        [Url]
        public string Link { get; set; }

        [StringLength(1000)]
        public string HostingModel { get; set; }

        public string RequiresHscn { get; set; }

        [StringLength(500)]
        public string Summary { get; set; }

        public TaskProgress Status()
        {
            if (string.IsNullOrEmpty(Summary) || string.IsNullOrEmpty(HostingModel))
                return TaskProgress.NotStarted;

            return TaskProgress.Completed;
        }
    }
}
